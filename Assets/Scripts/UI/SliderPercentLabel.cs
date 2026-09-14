using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUI
{
    [RequireComponent(typeof(Slider))]
    public class SliderPercentLabel : MonoBehaviour
    {
        public Slider slider;
        public Text label;
        public string format = "{0}%";

        void Reset()
        {
            slider = GetComponent<Slider>();
        }

        void OnEnable()
        {
            if (slider == null) slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(UpdateLabel);
            UpdateLabel(slider.value);
        }

        void OnDisable()
        {
            if (slider != null) slider.onValueChanged.RemoveListener(UpdateLabel);
        }

        void UpdateLabel(float value)
        {
            if (label == null) return;
            label.text = string.Format(format, Mathf.RoundToInt(value * 100f));
        }
    }
}
