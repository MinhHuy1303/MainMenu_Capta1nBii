using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUI
{
    /// <summary>
    /// Quản lý mở/đóng bảng "Hướng Dẫn" (Guide popup) ở Main Menu.
    /// </summary>
    public class GuideManager : MonoBehaviour
    {
        [Header("Tham chiếu UI")]
        public GameObject guidePanel; // GuidePopup - panel overlay cần bật/tắt
        public Button openButton;     // Nút "Hướng Dẫn" ở thanh dưới
        public Button closeButton;    // Nút [X] trong Header

        [Header("Tùy chọn")]
        public bool closeOnEscape = true; // Escape (PC) / nút Back (Android) cũng đóng bảng

        void Awake()
        {
            if (openButton != null) openButton.onClick.AddListener(Open);
            if (closeButton != null) closeButton.onClick.AddListener(Close);
        }

        void Update()
        {
            if (!closeOnEscape || guidePanel == null || !guidePanel.activeSelf) return;

            // Input.GetKeyDown(KeyCode.Escape) cũng nhận nút Back trên Android.
            if (Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        public void Open()
        {
            if (guidePanel != null) guidePanel.SetActive(true);
        }

        public void Close()
        {
            if (guidePanel != null) guidePanel.SetActive(false);
        }
    }
}
