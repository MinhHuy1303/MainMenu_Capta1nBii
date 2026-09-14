using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    /// <summary>
    /// Gắn vào LobbyPanel: khi bấm "VÀO GAME", nạp Scene của bài thi đã chọn ở
    /// SelectLevelPanel (lưu trong PlayerPrefs). Nếu Scene đó chưa được thêm vào
    /// Build Settings (bài thi chưa làm xong), hiển thị thông báo tạm thay vì lỗi.
    /// </summary>
    public class LobbyLevelLauncher : MonoBehaviour
    {
        [SerializeField] private ComingSoonPopup toast;

        public void PlaySelectedLevel()
        {
            string sceneName = PlayerPrefs.GetString(LevelSelectionManager.SelectedLevelSceneKey, "");

            if (string.IsNullOrEmpty(sceneName) || SceneUtility.GetBuildIndexByScenePath(sceneName) < 0)
            {
                if (toast != null) toast.Show("Bài thi đang được phát triển");
                else Debug.LogWarning("[LobbyLevelLauncher] Scene '" + sceneName + "' chưa có trong Build Settings.");
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
