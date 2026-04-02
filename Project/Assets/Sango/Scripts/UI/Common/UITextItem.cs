using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UITextItem : MonoBehaviour
    {
        public Text label;
        public Image image;
        public UITextItem SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }

        public UITextItem SetText(string lab)
        {
            label.text = lab;
            return this;
        }

        public UITextItem SetColor(Color c)
        {
            label.color = c;
            return this;
        }

        public UITextItem SetAlignment(TextAnchor c)
        {
            label.alignment = c;
            return this;
        }
    }
}