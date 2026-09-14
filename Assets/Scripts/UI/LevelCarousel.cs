using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Điều khiển snap-to-center cho ScrollRect ngang chứa các Card bài thi.
    /// Yêu cầu các Card có cùng bề rộng và cách đều nhau (cardStep = width + spacing),
    /// và Content có lề trái/phải = (bề rộng Viewport - bề rộng Card) / 2 để card đầu/cuối
    /// canh giữa được khi cuộn hết cỡ.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class LevelCarousel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private float cardStep = 340f;
        [SerializeField] private float snapDuration = 0.25f;

        public Action<int> onFocusChanged;

        private int cardCount;
        private int focusedIndex = -1;
        private bool dragging;
        private Coroutine snapRoutine;

        public void Init(int count)
        {
            cardCount = Mathf.Max(1, count);
            FixPadding();
        }

        /// <summary>
        /// Tính lại lề trái/phải của Content dựa trên bề rộng Viewport THỰC TẾ tại runtime,
        /// để card đầu/cuối luôn canh giữa được dù màn hình có tỉ lệ khác 16:9 (tránh co cụm/lệch).
        /// </summary>
        private void FixPadding()
        {
            if (content == null) return;
            HorizontalLayoutGroup hlg = content.GetComponent<HorizontalLayoutGroup>();
            LayoutElement firstCard = content.GetComponentInChildren<LayoutElement>();
            RectTransform viewport = content.parent as RectTransform;
            if (hlg == null || firstCard == null || viewport == null) return;

            float viewportWidth = viewport.rect.width;
            float cardWidth = firstCard.preferredWidth;
            int pad = Mathf.RoundToInt(Mathf.Max(0f, (viewportWidth - cardWidth) / 2f));
            hlg.padding = new RectOffset(pad, pad, hlg.padding.top, hlg.padding.bottom);
        }

        private void Update()
        {
            if (dragging || snapRoutine != null || content == null) return;

            int nearest = NearestIndex();
            if (nearest != focusedIndex)
            {
                focusedIndex = nearest;
                onFocusChanged?.Invoke(focusedIndex);
            }
        }

        private int NearestIndex()
        {
            if (cardStep <= 0f) return 0;
            return Mathf.Clamp(Mathf.RoundToInt(-content.anchoredPosition.x / cardStep), 0, cardCount - 1);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragging = true;
            if (snapRoutine != null) { StopCoroutine(snapRoutine); snapRoutine = null; }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            dragging = false;
            SnapTo(NearestIndex());
        }

        public void SnapTo(int index)
        {
            index = Mathf.Clamp(index, 0, cardCount - 1);
            focusedIndex = index;
            onFocusChanged?.Invoke(index);

            if (snapRoutine != null) StopCoroutine(snapRoutine);
            snapRoutine = StartCoroutine(SnapRoutine(-index * cardStep));
        }

        public void SnapToImmediate(int index)
        {
            index = Mathf.Clamp(index, 0, cardCount - 1);
            focusedIndex = index;
            if (snapRoutine != null) { StopCoroutine(snapRoutine); snapRoutine = null; }
            if (scrollRect != null) scrollRect.velocity = Vector2.zero;
            if (content != null) content.anchoredPosition = new Vector2(-index * cardStep, content.anchoredPosition.y);
            onFocusChanged?.Invoke(index);
        }

        private IEnumerator SnapRoutine(float targetX)
        {
            if (scrollRect != null) scrollRect.velocity = Vector2.zero;
            float startX = content.anchoredPosition.x;
            float t = 0f;
            while (t < snapDuration)
            {
                t += Time.unscaledDeltaTime;
                if (scrollRect != null) scrollRect.velocity = Vector2.zero;
                float x = Mathf.SmoothStep(startX, targetX, t / snapDuration);
                content.anchoredPosition = new Vector2(x, content.anchoredPosition.y);
                yield return null;
            }
            content.anchoredPosition = new Vector2(targetX, content.anchoredPosition.y);
            snapRoutine = null;
        }
    }
}
