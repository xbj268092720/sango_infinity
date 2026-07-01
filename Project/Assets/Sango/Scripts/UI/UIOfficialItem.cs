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


    }
}
