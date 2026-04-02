using Sango.Loader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIItemTypeSliderRect : MonoBehaviour
    {
        public delegate void OnItemTypeShow(ItemType itemType, UIItemTypeSlider uIItemTypeSlider);
        
        public UIItemTypeSlider itemTypeSlider;
        public RectTransform contentNode;
        public ScrollRect scrollRect;
        public OnItemTypeShow onItemTypeShow;
        List<UIItemTypeSlider> uIItemTypeSliders = new List<UIItemTypeSlider>();
        public void Init()
        {
            for (int i = 0; i < Scenario.Cur.CommonData.ItemTypeList.Count; i++)
            {
                ItemType t = Scenario.Cur.CommonData.ItemTypeList[i];
                UIItemTypeSlider item;
                if(i < uIItemTypeSliders.Count)
                {
                    item = uIItemTypeSliders[i];
                }
                else
                {
                    item = GameObject.Instantiate(itemTypeSlider.gameObject, contentNode).GetComponent<UIItemTypeSlider>();
                    item.gameObject.SetActive(true);
                    uIItemTypeSliders.Add(item);
                }
                item.SetItemType(t);
                onItemTypeShow?.Invoke(t, item);
            }
        }
    }
}