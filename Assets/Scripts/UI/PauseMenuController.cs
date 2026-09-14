using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace MainMenuUI
{
    /// <summary>
    /// Quản lý Pause Menu dùng chung cho mọi Scene gameplay: mở/đóng bằng nút hoặc phím
    /// Escape (PC) / nút Back (Android), và điều khiển Time.timeScale khi tạm dừng.
    /// GameObject gốc chứa script này luôn active để còn nhận input mở menu;
    /// phần hiển thị (menuVisual) mới là phần được bật/tắt.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [Header("Tham chiếu UI")]
        public GameObject menuVisual;      // Overlay + PauseBox — phần thực sự hiện/ẩn
        public GameObject settingsPanel;   // Bảng Cài Đặt của Scene hiện tại (có thể để trống nếu Scene chưa có)

        [Header("Tùy chọn")]
        public bool toggleWithEscape = true; // Escape (PC) / nút Back (Android) mở hoặc đóng menu
        [SerializeField] string mainMenuSceneName = "SampleScene";

        bool IsOpen => menuVisual != null && menuVisual.activeSelf;

        void Update()
        {
            if (!toggleWithEscape) return;

            // Android back button phát sinh cùng sự kiện Escape trên thiết bị Keyboard ảo của Input System.
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (IsOpen) Resume();
                else Open();
            }
        }

        public void Open()
        {
            if (menuVisual != null) menuVisual.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            if (menuVisual != null) menuVisual.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OpenSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
            else
                Debug.LogWarning("[PauseMenuController] Chưa gán settingsPanel cho Scene này.", this);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
