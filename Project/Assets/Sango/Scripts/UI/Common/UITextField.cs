using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UITextField : MonoBehaviour
    {
        public Text label;
        public Image image;
        public Text titleLabel;

        public string text { get { return label.text; } set { label.text = value; } }

        public UITextField SetTitle(string title)
        {
            if(titleLabel != null)
                titleLabel.text = title;
            return this;
        }

        public UITextField SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }

        public UITextField SetText(string lab)
        {
            label.text = lab;
            return this;
        }

        public UITextField SetColor(Color c)
        {
            label.color = c;
            return this;
        }

        public UITextField SetAlignment(TextAnchor c)
        {
            label.alignment = c;
            return this;
        }
    }
}