using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIToggleGroupField : MonoBehaviour
    {
        public Text title;
        public UIToggleField toggleObj;
        List<UIToggleField> toggles = new List<UIToggleField>();
        public int value;
        public System.Action<int> onValueChange;

        public void Set(string title, int value, List<string> options, System.Action<int> onValueChange)
        {
            this.value = value;
            this.title.text = title;
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < options.Count; i++)
            {
                UIToggleField toggle;
                if (i < toggles.Count)
                {
                    toggle = toggles[i];
                }
                else
                {
                    GameObject go = GameObject.Instantiate(toggleObj.gameObject, toggleObj.gameObject.transform.parent);
                    toggle = go.GetComponentInChildren<UIToggleField>();
                    toggles.Add(toggle);
                }
                toggle.gameObject.SetActive(true);
                toggle.Set(options[i], value == i, OnValueChanged);
            }

            this.onValueChange = onValueChange;
        }

        void OnValueChanged(bool b)
        {
            if (b)
            {
                for (int i = 0; i < toggles.Count; i++)
                {
                    if (toggles[i].value)
                    {
                        onValueChange(i);
                        return;
                    }
                }
            }
        }
    }
}