using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUI
{
    /// <summary>
    /// Quản lý cụm 3 nút chọn chất lượng đồ họa (Thấp / Trung Bình / Cao).
    /// - Đồng bộ với QualitySettings của Unity.
    /// - Tô màu nút đang chọn (xanh lá) khác với các nút còn lại (đen).
    /// - Lưu/đọc lựa chọn bằng PlayerPrefs để nhớ giữa các lần mở game.
    /// </summary>
    public class GraphicsSettingsManager : MonoBehaviour
    {
        [Header("Gán đúng thứ tự: [0]=Thấp, [1]=Trung Bình, [2]=Cao")]
        public Button[] qualityButtons;

        [Header("Màu nền nút")]
        public Color activeColor = new Color(0.18f, 0.80f, 0.44f);   // #2ECC71 - nút đang chọn
        public Color inactiveColor = new Color(0.10f, 0.10f, 0.10f); // đen/xám tối - nút chưa chọn

        // Khóa lưu PlayerPrefs, đặt const để tránh gõ sai chuỗi ở nhiều nơi
        const string PrefsKey = "QualityLevel";

        void Awake()
        {
            // Gán sự kiện click cho từng nút ngay trong code (không cần kéo-thả OnClick trong Inspector).
            // Dùng biến "index" cục bộ bên trong vòng lặp để tránh lỗi closure (nếu dùng thẳng "i"
            // thì cả 3 nút sẽ cùng lấy giá trị i cuối cùng sau khi vòng lặp kết thúc).
            for (int i = 0; i < qualityButtons.Length; i++)
            {
                int index = i;
                qualityButtons[i].onClick.AddListener(() => SetQuality(index));
            }
        }

        void Start()
        {
            // Khi mở lại menu: đọc mức đã lưu, nếu chưa từng lưu thì lấy mức hiện tại của QualitySettings.
            int savedIndex = PlayerPrefs.GetInt(PrefsKey, QualitySettings.GetQualityLevel());
            ApplyQuality(savedIndex, save: false); // không cần ghi lại PlayerPrefs khi chỉ là load ban đầu
        }

        /// <summary>
        /// Gọi hàm này khi người dùng bấm nút (đã tự động gán ở Awake).
        /// </summary>
        public void SetQuality(int index)
        {
            ApplyQuality(index, save: true);
        }

        /// <summary>
        /// Áp dụng mức chất lượng: set QualitySettings, tô màu nút, tùy chọn lưu PlayerPrefs.
        /// </summary>
        void ApplyQuality(int index, bool save)
        {
            if (index < 0 || index >= qualityButtons.Length) return;

            QualitySettings.SetQualityLevel(index, true);
            HighlightButton(index);

            if (save)
                PlayerPrefs.SetInt(PrefsKey, index);
        }

        /// <summary>
        /// Đổi màu nền: nút được chọn -> xanh lá (active), các nút còn lại -> đen (inactive).
        /// </summary>
        void HighlightButton(int selectedIndex)
        {
            for (int i = 0; i < qualityButtons.Length; i++)
            {
                var background = qualityButtons[i].targetGraphic as Image;
                if (background == null) continue;
                background.color = (i == selectedIndex) ? activeColor : inactiveColor;
            }
        }
    }
}
