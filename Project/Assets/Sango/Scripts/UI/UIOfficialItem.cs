using Sango.Core.Player;
using UnityEngine;
using Sango.Core;
using UnityEngine.UI;

namespace Sango.UI
{
    public class UIOfficialItem : MonoBehaviour
    {
        public int index;
        public delegate void OnSelect(UIOfficialItem item);
        public OnSelect onSelected;
        public OnSelect onDoubleSelected;
        public RectTransform contentRect;
        public Image selectImg;
        public Image overImg;
        public Image pressImg;
        public Image disableImg;
        public UITextItem[] uITextItems;
        float lastPressTime;
        readonly float doubleClickTimer = 0.3f;
        int clickCount = 0;

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
            if (!b)
            {
                float off = Time.realtimeSinceStartup - lastPressTime;
                if (off <= doubleClickTimer)
                {
                    onDoubleSelected?.Invoke(this);
                }
                else
                {
                    onSelected?.Invoke(this);
                }
                lastPressTime = Time.realtimeSinceStartup;
            }
        }

        public void SetPerson(Person person, Official official)
        {
            if (person == null)
            {
                uITextItems[0].SetText("");
                uITextItems[1].SetText("");
                uITextItems[2].SetText("");
                uITextItems[3].SetText("");
                uITextItems[4].SetText("");
                uITextItems[5].SetText("");
                if (uITextItems.Length > 6)
                {
                    uITextItems[6]?.SetText("");
                }
            }
            else
            {
                uITextItems[0].SetText(official.Name);
                uITextItems[1].SetText(official.level.ToString());
                uITextItems[2].SetText(person.Official.meritNeeds.ToString());
                uITextItems[3].SetText(official.troopsLimit.ToString());
                uITextItems[4].SetText(official.cost.ToString());
                uITextItems[5].SetText(official.effect_desc);
                if (uITextItems.Length > 6)
                {
                    uITextItems[6]?.SetText(person.ColorName);
                }

            }
        }

        public void SetDisable(bool b)
        {
            if (disableImg != null)
                disableImg.enabled = b;
        }
    }
}
