using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UISelectItem : MonoBehaviour
    {
        public Image select;
        public GameObject selNode;

        public bool IsSelected()
        {
            return select.gameObject.activeSelf;
        }

        public UISelectItem SetSelected(bool b)
        {
            select.gameObject.SetActive(b);
            return this;
        }

        public UISelectItem SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }
        public UISelectItem SetVisible(bool b)
        {
            selNode.SetActive(b);
            return this;
        }

    }
}