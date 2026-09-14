using UnityEngine;

namespace MainMenuUI
{
    /// <summary>
    /// Buộc game chạy đúng độ phân giải thực của thiết bị ngay khi khởi động,
    /// tránh trường hợp Unity tự hạ độ phân giải render trên Android khiến hình ảnh 3D bị mờ.
    /// Gắn script này vào 1 GameObject duy nhất trong scene khởi động đầu tiên.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Boot : MonoBehaviour
    {
        void Awake()
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
    }
}
