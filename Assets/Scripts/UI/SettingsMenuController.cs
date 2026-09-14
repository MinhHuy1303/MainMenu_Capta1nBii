using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MainMenuUI
{
    /// <summary>
    /// Quản lý toàn bộ logic bảng Cài Đặt: Đồ họa, Điều khiển, Tốc độ trả lái, Âm thanh.
    /// Tự động lưu/đọc lựa chọn người dùng bằng PlayerPrefs.
    /// </summary>
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("1. Chất lượng đồ họa — [0]=Thấp [1]=Trung Bình [2]=Cao")]
        public Button[] qualityButtons;

        [Header("2. Điều khiển — [0]=Vô lăng [1]=Nghiêng (Tilt)")]
        public Button[] controlButtons;

        [Header("3. Tốc độ trả lái (Slider giá trị 0-1)")]
        public Slider steeringReturnSlider;
        public TMP_Text steeringReturnValueLabel; // hiển thị % bên cạnh thanh trượt

        [Header("4. Âm thanh")]
        public Toggle soundToggle;
        public Image soundToggleBackground;
        public GameObject soundOnWaves;   // hiện khi Bật
        public GameObject soundOffSlash;  // hiện khi Tắt (gạch chéo)
        public TMP_Text soundStateLabel;  // chữ "BẬT" / "TẮT"

        [Header("Màu trạng thái dùng chung")]
        public Color activeColor = new Color(0.18f, 0.80f, 0.44f);   // xanh lá - đang chọn / bật
        public Color inactiveColor = new Color(0.10f, 0.10f, 0.10f); // đen - chưa chọn / tắt

        const string KeyQuality = "QualityLevel";
        const string KeyControlMode = "ControlMode";       // 0 = Vô lăng, 1 = Nghiêng
        const string KeySteeringReturn = "SteeringReturnSpeed";
        const string KeySoundOn = "SoundOn";

        void Awake()
        {
            for (int i = 0; i < qualityButtons.Length; i++)
            {
                int index = i; // biến cục bộ để tránh lỗi closure trong vòng lặp
                qualityButtons[i].onClick.AddListener(() => SetQuality(index));
            }

            for (int i = 0; i < controlButtons.Length; i++)
            {
                int index = i;
                controlButtons[i].onClick.AddListener(() => SetControlMode(index));
            }

            if (steeringReturnSlider != null)
                steeringReturnSlider.onValueChanged.AddListener(SetSteeringReturnSpeed);

            if (soundToggle != null)
                soundToggle.onValueChanged.AddListener(SetSoundOn);
        }

        void OnEnable()
        {
            // Luôn đưa bảng Cài Đặt lên trên cùng Canvas khi mở, dù được gọi từ Menu chính
            // hay từ Pause Menu (Pause Menu là sibling cuối nên vẽ đè lên nếu không làm bước này).
            if (transform.parent != null)
                transform.parent.SetAsLastSibling();

            // Mỗi lần bảng Cài Đặt được mở lại, nạp đúng trạng thái đã lưu.
            LoadAllSettings();
        }

        void LoadAllSettings()
        {
            int savedQuality = PlayerPrefs.GetInt(KeyQuality, QualitySettings.GetQualityLevel());
            ApplyQuality(savedQuality, save: false);

            int savedControl = PlayerPrefs.GetInt(KeyControlMode, 0);
            ApplyControlMode(savedControl, save: false);

            float defaultSteering = steeringReturnSlider != null ? steeringReturnSlider.value : 0.5f;
            float savedSteering = PlayerPrefs.GetFloat(KeySteeringReturn, defaultSteering);
            if (steeringReturnSlider != null)
                steeringReturnSlider.SetValueWithoutNotify(savedSteering);
            UpdateSteeringReturnLabel(savedSteering);

            bool savedSound = PlayerPrefs.GetInt(KeySoundOn, 1) == 1;
            if (soundToggle != null)
                soundToggle.SetIsOnWithoutNotify(savedSound);
            ApplySoundVisual(savedSound);
            AudioListener.volume = savedSound ? 1f : 0f;
        }

        // ---------------- 1. Đồ họa ----------------
        public void SetQuality(int index) => ApplyQuality(index, save: true);

        void ApplyQuality(int index, bool save)
        {
            if (qualityButtons == null || index < 0 || index >= qualityButtons.Length) return;
            QualitySettings.SetQualityLevel(index, true);
            HighlightGroup(qualityButtons, index);
            if (save) PlayerPrefs.SetInt(KeyQuality, index);
        }

        // ---------------- 2. Điều khiển ----------------
        public void SetControlMode(int index) => ApplyControlMode(index, save: true);

        void ApplyControlMode(int index, bool save)
        {
            if (controlButtons == null || index < 0 || index >= controlButtons.Length) return;
            HighlightGroup(controlButtons, index);
            if (save) PlayerPrefs.SetInt(KeyControlMode, index);
            // Hệ thống điều khiển xe đọc PlayerPrefs["ControlMode"] (0 = Vô lăng, 1 = Nghiêng) để chọn scheme phù hợp.
        }

        // ---------------- 3. Tốc độ trả lái ----------------
        public void SetSteeringReturnSpeed(float value)
        {
            PlayerPrefs.SetFloat(KeySteeringReturn, value);
            // Script điều khiển xe đọc PlayerPrefs["SteeringReturnSpeed"] làm hệ số hồi vô lăng.
            UpdateSteeringReturnLabel(value);
        }

        void UpdateSteeringReturnLabel(float value)
        {
            if (steeringReturnValueLabel != null)
                steeringReturnValueLabel.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        // ---------------- 4. Âm thanh ----------------
        public void SetSoundOn(bool isOn)
        {
            AudioListener.volume = isOn ? 1f : 0f;
            PlayerPrefs.SetInt(KeySoundOn, isOn ? 1 : 0);
            ApplySoundVisual(isOn);
        }

        void ApplySoundVisual(bool isOn)
        {
            if (soundToggleBackground != null)
                soundToggleBackground.color = isOn ? activeColor : inactiveColor;
            if (soundOnWaves != null) soundOnWaves.SetActive(isOn);
            if (soundOffSlash != null) soundOffSlash.SetActive(!isOn);
            if (soundStateLabel != null) soundStateLabel.text = isOn ? "BẬT" : "TẮT";
        }

        // ---------------- Dùng chung ----------------
        void HighlightGroup(Button[] buttons, int selectedIndex)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                var img = buttons[i].targetGraphic as Image;
                if (img != null) img.color = (i == selectedIndex) ? activeColor : inactiveColor;
            }
        }
    }
}
