using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIToggleItem : MonoBehaviour
    {
        public Text title;
        public Toggle toggle;

        public UIToggleItem SetTitle(string s)
        {
            if(title != null)
                title.text = s;
            return this;
        }

        public UIToggleItem SetWidth(int width)
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