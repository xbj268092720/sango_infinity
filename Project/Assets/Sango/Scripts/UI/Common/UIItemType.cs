using Sango.Loader;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIItemType : MonoBehaviour
    {
        public UITextField name;
        public UITextField numberLabel;
        public ItemType itemType;
        public int maxNumber;
        public int useNumber;

        public UIItemType SetItemType(ItemType type)
        {
            itemType = type;
            name.text = type.Name;
            return this;
        }

        public UIItemType SetNumber(int num)
        {
            maxNumber = num;
            if (num > 0)
                numberLabel.text = num.ToString();
            else
                numberLabel.text = $"<color=#ff0000>{num}</color>";
            return this;
        }

        public UIItemType SetUsed(int num)
        {
            if (maxNumber <= 0)
                return this;

            int left = maxNumber - num;
            if (left > 0)
            {
                if (num == 0)
                    numberLabel.text = maxNumber.ToString();
                else
                    numberLabel.text = $"{maxNumber}→{left}";
            }
            else
            {
                if (maxNumber == 0)
                    numberLabel.text = $"<color=#ff0000>{maxNumber}</color>";
                else
                    numberLabel.text = $"{maxNumber}→<color=#ff0000>{left}</color>";
            }
            return this;
        }

    }
}