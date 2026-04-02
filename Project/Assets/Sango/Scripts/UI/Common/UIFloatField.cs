using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIFloatField : MonoBehaviour
    {
        public Text title;
        public InputField inputField;

        public float min;
        public float max;
        public float value;

        public System.Action<float> onValueChange;

        public void Set(string title, float value, float min, float max, System.Action<float> onValueChange)
        {
            this.title.text = title;
            if (min > max) { min = max; }
            inputField.SetTextWithoutNotify(value.ToString());
            inputField.characterValidation = InputField.CharacterValidation.Decimal;
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(OnInputNumberChanged);
            this.onValueChange = onValueChange;
        }

        void OnInputNumberChanged(string number)
        {
            if (float.TryParse(number, out float v))
            {
                if (v > max) v = max;
                else if (v < min) v = min;
                if (v != value)
                {
                    value = v;
                    onValueChange?.Invoke(value);
                }
            }
        }
    }
}