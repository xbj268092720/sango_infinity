using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIButtonItem : MonoBehaviour
    {
        public Text title;
        public Button button;

        public UnityEngine.Sprite[] normal;
        public UnityEngine.Sprite[] pressed;

        public System.Action clickAction;

        public void OnClick()
        {
            clickAction?.Invoke();
        }

        public UIButtonItem SetTitle(string s)
        {
            if (title != null)
                title.text = s;
            return this;
        }

        public UIButtonItem BindAction(System.Action action)
        {
            clickAction = action;
            button.interactable = (clickAction != null);
            return this;
        }

        public UIButtonItem SetWidth(int width)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null)
                layoutElement.preferredWidth = width;
            return this;
        }

        public UIButtonItem SetStyle(int style)
        {
            if (style < normal.Length)
            {
                SpriteState spriteState = button.spriteState;
                spriteState.pressedSprite = pressed[style];
                button.spriteState = spriteState;
                button.image.sprite = normal[style];
            }
            return this;
        }
    }
}