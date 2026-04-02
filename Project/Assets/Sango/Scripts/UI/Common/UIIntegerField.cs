using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIIntegerField : MonoBehaviour
    {
        public Text title;
        public InputField inputField;

        public int min;
        public int max;
        public int value;

        public System.Action<int> onValueChange;

        public void Set(string title, int value, int min, int max, System.Action<int> onValueChange)
        {
            this.title.text = title;
            if (min > max) { min = max; }
            inputField.SetTextWithoutNotify(value.ToString());
            inputField.characterValidation = InputField.CharacterValidation.Integer;
            inputField.onValueChanged.RemoveAllListeners();
            inputField.onValueChanged.AddListener(OnInputNumberChanged);
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
                    onValueChange?.Invoke(value);
                }
            }
        }
    }
}