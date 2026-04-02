using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIIntegerSliderField : MonoBehaviour
    {
        public Text title;
        public Slider slider;
        public InputField inputField;

        public int min;
        public int max;
        public int value;

        public System.Action<int> onValueChange;

        public void Set(string title, int value, int min, int max, System.Action<int> onValueChange)
        {
            this.title.text = title;
            if (min >= max)
            {
                Sango.Log.Error("min >= max! set min = max - 1");
                min = max - 1;
            }
            this.slider.minValue = min;
            this.slider.maxValue = max;
            this.slider.SetValueWithoutNotify((float)(value - min) / (float)(max - min));
            inputField.SetTextWithoutNotify(value.ToString());
            inputField.characterValidation = InputField.CharacterValidation.Integer;
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(OnInputNumberChanged);
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(OnSliderNumberChanged);
            this.onValueChange = onValueChange;
        }

        void OnInputNumberChanged(string number)
        {
            if (int.TryParse(number, out int v))
            {
                if (v > max) v = max;
                else if (v < min) v = min;
                if (v != value)
                {
                    value = v;
                    slider.SetValueWithoutNotify((float)(value - min) / (max - min));
                    onValueChange?.Invoke(value);
                }
            }
        }

        void OnSliderNumberChanged(float number)
        {
            int v = min + (int)((float)(max - min) * number);
            if (v != value)
            {
                value = v;
                inputField.SetTextWithoutNotify(value.ToString());
                onValueChange?.Invoke(value);
            }
        }
    }
}