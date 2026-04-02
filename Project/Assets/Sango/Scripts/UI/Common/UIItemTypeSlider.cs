using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIItemTypeSlider : MonoBehaviour
    {
        public Button item;
        public Text name;
        public Slider numberSlider;
        public UITextField numberLabel;

        public ItemType itemType;
        public int maxNumber;

        public UIItemTypeSlider SetItemType(ItemType type)
        {
            itemType = type;
            name.text = type.Name;
            return this;
        }
        public UIItemTypeSlider SetValid(bool b)
        {
            item.interactable = b;
            numberSlider.interactable = b;
            return this;
        }

    }
}