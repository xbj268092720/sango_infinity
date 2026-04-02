using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIScenarioSaveItem : MonoBehaviour
    {
        public Text id;
        public Text name;
        public Text time;
        public Text newText;
        public Image load;
        public Image save;
        public Button save_button;
        public Button load_button;
        public Button sureBtn;
        public RectTransform root;
        public int targetIndex;

        public UIScenarioSaveItem SetName(string n)
        {
            name.text = n;
            return this;
        }

        public UIScenarioSaveItem SetInactive(bool b)
        {
            load.gameObject.SetActive(!b);
            save.gameObject.SetActive(!b);
            return this;
        }

        public UIScenarioSaveItem SetId(int n)
        {
            if (n < 0)
            {
                id.text = "";
                return this;
            }

            id.text = n.ToString();
            return this;
        }

        public UIScenarioSaveItem SetNew(bool b)
        {
            newText.enabled = b;
            return this;
        }

        public UIScenarioSaveItem SetSaveTime(long t)
        {
            if (t < 0)
            {
                time.text = "";
                return this;
            }

            DateTime date = DateTime.FromFileTime(t);
            time.text = date.ToString("yyyy-MM-dd HH:mm");
            return this;
        }

        public UIScenarioSaveItem SetTime(long t)
        {
            if(t < 0)
            {
                time.text = "";
                return this;
            }

            DateTime date = DateTime.FromFileTime(t);
            time.text = date.ToString("yyyy-MM-dd HH:mm:ss");
            return this;
        }

        public UIScenarioSaveItem SetIsLoad(bool b)
        {
            load.enabled = b;
            save.enabled = !b;
            return this;
        }

        public UIScenarioSaveItem SetSelected(bool b)
        {
            sureBtn.gameObject.SetActive(b);
            return this;
        }
        public UIScenarioSaveItem BindSureCall(UnityAction call)
        {
            sureBtn.onClick.RemoveAllListeners();
            sureBtn.onClick.AddListener(call);
            return this;
        }

        public UIScenarioSaveItem BindCall(UnityAction call)
        {
            save_button.onClick.RemoveAllListeners();
            load_button.onClick.RemoveAllListeners();
            save_button.onClick.AddListener(call);
            load_button.onClick.AddListener(call);
            return this;
        }

        public UIScenarioSaveItem SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }

    }
}