using UnityEngine;
using UIImage = UnityEngine.UI.Image;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    /// <summary>
    /// Một Card bài thi trong SelectLevelPanel: hiển thị ảnh xem trước, tên bài,
    /// và phản ánh trạng thái khóa/đang chọn do LevelSelectionManager điều khiển.
    /// </summary>
    public class LevelCardUI : MonoBehaviour
    {
        [SerializeField] private UIImage previewImage;
        [SerializeField] private GameObject lockOverlay;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private Outline focusOutline;
        [SerializeField] private Outline glowOutline;

        private static readonly Color FocusColor = new Color(0f, 0.902f, 0.463f, 1f); // #00E676
        private static readonly Color FocusGlowColor = new Color(0f, 0.902f, 0.463f, 0.55f);
        private static readonly Color TransparentColor = new Color(0f, 0.902f, 0.463f, 0f);
        private static readonly Color DefaultOutlineColor = new Color(0.82f, 0.92f, 1f, 0.35f);
        private static readonly Vector2 FocusOutlineDistance = new Vector2(3f, 3f);
        private static readonly Vector2 DefaultOutlineDistance = new Vector2(1.5f, 1.5f);
        private static readonly Vector2 FocusGlowDistance = new Vector2(7f, 7f);
        private static readonly float LockedPreviewAlpha = 0.4f;

        [HideInInspector] public int index;

        private Vector3 targetScale = Vector3.one;

        public void Setup(int idx, string levelName, Sprite preview)
        {
            index = idx;
            if (nameLabel != null) nameLabel.text = levelName;
            if (previewImage != null && preview != null) previewImage.sprite = preview;
        }

        public void ApplyState(bool locked, bool focused)
        {
            if (lockOverlay != null) lockOverlay.SetActive(locked);

            if (previewImage != null)
            {
                Color c = previewImage.color;
                c.a = locked ? LockedPreviewAlpha : 1f;
                previewImage.color = c;
            }

            bool highlighted = focused && !locked;
            targetScale = highlighted ? Vector3.one * 1.05f : Vector3.one;

            if (focusOutline != null)
            {
                focusOutline.effectColor = highlighted ? FocusColor : DefaultOutlineColor;
                focusOutline.effectDistance = highlighted ? FocusOutlineDistance : DefaultOutlineDistance;
            }

            if (glowOutline != null)
            {
                glowOutline.effectColor = highlighted ? FocusGlowColor : TransparentColor;
                glowOutline.effectDistance = highlighted ? FocusGlowDistance : Vector2.zero;
            }
        }

        private void Update()
        {
            if (transform.localScale != targetScale)
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * 12f);
        }
    }
}
