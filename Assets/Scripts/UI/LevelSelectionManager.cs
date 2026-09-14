using System.Collections;
using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class LevelOption
    {
        public string levelName;
        public string sceneName;
        public Sprite previewSprite;
        public bool isLocked;
    }

    /// <summary>
    /// Quản lý bảng "CHỌN BÀI THI HẠNG B": danh sách bài thi, trạng thái khóa/mở,
    /// và điều hướng sang Sảnh chờ (LobbyPanel) khi chọn được 1 bài thi hợp lệ.
    /// Bài thi được chọn lưu vào PlayerPrefs để LobbyPanel/BtnPlay đọc lại khi bấm "VÀO GAME".
    /// </summary>
    public class LevelSelectionManager : MonoBehaviour
    {
        public const string SelectedLevelIndexKey = "SelectedLevelIndex";
        public const string SelectedLevelSceneKey = "SelectedLevelScene";
        public const string SelectedLevelNameKey = "SelectedLevelName";

        [Header("Panels")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject[] mainMenuLayersToHide;

        [Header("Carousel")]
        [SerializeField] private LevelCarousel carousel;
        [SerializeField] private LevelCardUI[] cards;

        [Header("Data")]
        [SerializeField] private LevelOption[] levels;
        [SerializeField] private int defaultFocusIndex = 1;

        [Header("Phản hồi")]
        [SerializeField] private ComingSoonPopup toast;
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeMagnitude = 6f;
        [SerializeField] private float goToLobbyDelay = 0.2f;

        private void Awake()
        {
            for (int i = 0; i < cards.Length && i < levels.Length; i++)
            {
                cards[i].Setup(i, levels[i].levelName, levels[i].previewSprite);
                cards[i].ApplyState(levels[i].isLocked, false);
            }

            if (carousel != null)
            {
                carousel.Init(levels.Length);
                carousel.onFocusChanged = OnFocusChanged;
            }
        }

        public void Open()
        {
            if (panelRoot != null) panelRoot.SetActive(true);
            SetMainMenuLayersVisible(false);

            int startIndex = PlayerPrefs.GetInt(SelectedLevelIndexKey, defaultFocusIndex);
            startIndex = Mathf.Clamp(startIndex, 0, levels.Length - 1);

            if (carousel != null) carousel.SnapToImmediate(startIndex);
            else OnFocusChanged(startIndex);
        }

        public void Close()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            SetMainMenuLayersVisible(true);
        }

        private void SetMainMenuLayersVisible(bool visible)
        {
            if (mainMenuLayersToHide == null) return;
            for (int i = 0; i < mainMenuLayersToHide.Length; i++)
                if (mainMenuLayersToHide[i] != null) mainMenuLayersToHide[i].SetActive(visible);
        }

        private void OnFocusChanged(int index)
        {
            for (int i = 0; i < cards.Length && i < levels.Length; i++)
                cards[i].ApplyState(levels[i].isLocked, i == index);
        }

        public void OnCardClicked(int index)
        {
            if (index < 0 || index >= levels.Length) return;
            LevelOption level = levels[index];

            if (level.isLocked)
            {
                StartCoroutine(ShakeCard(index));
                if (toast != null) toast.Show("Bài thi chưa được mở khóa!");
                return;
            }

            if (carousel != null) carousel.SnapTo(index);

            PlayerPrefs.SetInt(SelectedLevelIndexKey, index);
            PlayerPrefs.SetString(SelectedLevelSceneKey, level.sceneName);
            PlayerPrefs.SetString(SelectedLevelNameKey, level.levelName);
            PlayerPrefs.Save();

            StartCoroutine(GoToLobbyAfterDelay(goToLobbyDelay));
        }

        private IEnumerator GoToLobbyAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            // Nếu người chơi đã bấm "Quay Lại" trước khi hết delay thì panelRoot đã bị tắt rồi —
            // hủy chuyển màn để tránh Lobby bật lên ngoài ý muốn sau khi đã back ra.
            if (panelRoot != null && !panelRoot.activeSelf) yield break;

            Close();
            if (lobbyPanel != null) lobbyPanel.SetActive(true);
        }

        private IEnumerator ShakeCard(int index)
        {
            if (index < 0 || index >= cards.Length) yield break;

            RectTransform rt = cards[index].transform as RectTransform;
            if (rt == null) yield break;

            float t = 0f;
            while (t < shakeDuration)
            {
                t += Time.unscaledDeltaTime;
                float angle = Mathf.Sin(t * 50f) * shakeMagnitude * (1f - t / shakeDuration);
                rt.localRotation = Quaternion.Euler(0f, 0f, angle);
                yield return null;
            }
            rt.localRotation = Quaternion.identity;
        }
    }
}
