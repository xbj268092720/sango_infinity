using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UISortButton : MonoBehaviour
    {
        public UISortButtonGroup sortGroup;
        public Button button;
        public Text nameLab;
        public Image icon;
        public Image up;
        public Image down;
        public delegate void OnClick(bool up);
        public OnClick onClick;

        public void Start()
        {
            if (sortGroup != null)
            {
                sortGroup.Add(this);
            }
        }
        public UISortButton SetName(string name)
        {
            nameLab.text = name;
            return this;
        }
        public UISortButton SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }

        public UISortButton Clear()
        {
            down.enabled = false;
            up.enabled = false;
            return this;
        }

        public void OnButtonClick()
        {
            if (sortGroup != null)
            {
                sortGroup.Select(this);
            }

            if ((up.enabled == false && down.enabled == false) || down.enabled == false)
            {
                down.enabled = true;
                up.enabled = false;
                onClick?.Invoke(false);
            }
            else if (up.enabled == false)
            {
                up.enabled = true;
                down.enabled = false;
                onClick?.Invoke(true);
            }
        }

    }
}