using Sango.Loader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Sango.Game.Render.UI
{
    public class UIDropdownField : MonoBehaviour
    {
        public Text title;
        public Dropdown dropdown;
        public int value;
        public System.Action<int> onValueChange;

        public void Set(string title, int value, List<string> options, System.Action<int> onValueChange)
        {
            this.value = value;
            this.title.text = title;
            dropdown.ClearOptions();
            dropdown.AddOptions(options);
            dropdown.SetValueWithoutNotify(value);
            this.onValueChange = onValueChange;
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        void OnValueChanged(int v)
        {
            if (v != value)
            {
                value = v;
                onValueChange?.Invoke(v);
            }
        }
    }
}