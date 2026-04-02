using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIToggleField : MonoBehaviour
    {
        public Text title;
        public Toggle toggle;
        public bool value;
        public System.Action<bool> onValueChange;

        public void Set(string title, bool value, System.Action<bool> onValueChange)
        {
            this.title.text = title;
            toggle.isOn = value;
            this.onValueChange = onValueChange;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        void OnToggleValueChanged(bool v)
        {
            if (v != value)
            {
                value = v;
                onValueChange?.Invoke(value);
            }
        }
    }
}