using Sango.Core.Player;
using UnityEngine;
using Sango.Core;
using UnityEngine.UI;

namespace Sango.UI
{
    public class UIOfficialItem : MonoBehaviour
    {
        public int index;
        public delegate void OnSelect(UIObjectListItem item);
        public delegate void OnShow(UIObjectListItem item);
        public OnSelect onSelected;
        public OnShow onShow;
        public RectTransform contentRect;
        public Image selectImg;
        public Image overImg;
        public Image pressImg;
        public UITextItem[] uITextItems;

        void ScrollCellIndex(int idx)
        {
            index = idx;
        }
        public void OnClick()
        {
        }

        public void Clear()
        {
            
        }

        public void Set(string content)
        {
            
        }

        public void Add(string content, int width)
        {
            UITextItem item;
           
        }

        public void Set(int index, string content)
        {
            if (index < uITextItems.Length)
                uITextItems[index - 1].SetText(content);
        }

        public void SetSelected(bool b)
        {
            selectImg.enabled = b;
        }

        public bool IsSelected()
        {
            return selectImg.enabled;
        }

        public void SetOver(bool b)
        {
            overImg.enabled = b;
        }

        public void SetPressd(bool b)
        {
            pressImg.enabled = b;
        }

        public void SetPerson(Person person, Official official)
        {
            if(person == null)
            {
                uITextItems[0].SetText("");
                uITextItems[1].SetText("");
                uITextItems[2].SetText("");
                uITextItems[3].SetText("");
                uITextItems[4].SetText("");
                uITextItems[5].SetText("");
            }
            else
            {
                uITextItems[0].SetText(official.Name);
                uITextItems[1].SetText(person.Official.meritNeeds.ToString());
                uITextItems[2].SetText(official.troopsLimit.ToString());
                uITextItems[3].SetText(official.cost.ToString());
                uITextItems[4].SetText(official.effect_desc);
                uITextItems[5].SetText(person.ColorName);
            }
        }
    }
}
