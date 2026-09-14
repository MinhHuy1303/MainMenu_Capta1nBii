using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenuUI
{
    public class ButtonPunchEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float pressedScale = 0.95f;
        public float lerpSpeed = 18f;

        RectTransform rt;
        Vector3 targetScale = Vector3.one;

        void Awake()
        {
            rt = transform as RectTransform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = Vector3.one * pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = Vector3.one;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = Vector3.one;
        }

        void Update()
        {
            if (rt == null) return;
            rt.localScale = Vector3.Lerp(rt.localScale, targetScale, Time.unscaledDeltaTime * lerpSpeed);
        }
    }
}
