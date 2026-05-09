using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Sango.Tools
{
    public class UITopMenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image bg;
        public Image icon;
        public Text label;
        public System.Action<UITopMenuItem> action;
        public EditorMenuItemData data;
        public UIMapEditor rootUI;

        public void Show(EditorMenuItemData data)
        {
            bg.color = Color.white;
            this.data = data;
            label.text = data.displayName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            rootUI.currentTopMenu = this;
            if (action != null)
            {
                action(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(rootUI.currentTopMenu != this)
            {
                bg.color = Color.white;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            bg.color = rootUI.bgColor;
            if (rootUI.currentTopMenu != null)
            {
                rootUI.currentTopMenu.bg.color = Color.white;
                rootUI.currentTopMenu = this;
                if (action != null)
                {
                    action(this);
                }
                return;
            }
        }
    }
}