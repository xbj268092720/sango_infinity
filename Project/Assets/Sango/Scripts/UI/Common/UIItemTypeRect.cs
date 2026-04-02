using Sango.Loader;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIItemTypeRect : MonoBehaviour
    {
        public delegate void OnItemTypeShow(ItemType itemType, UIItemType uIItemType);

        public UIItemType uiItemType;
        public RectTransform contentNode;
        public ScrollRect scrollRect;
        public OnItemTypeShow onItemTypeShow;

        List<UIItemType> uIItemTypes = new List<UIItemType>();

        public void Init()
        {
            for (int i = 0; i < Scenario.Cur.CommonData.ItemTypeList.Count; i++)
            {
                ItemType t = Scenario.Cur.CommonData.ItemTypeList[i];
                UIItemType item;
                if (i < uIItemTypes.Count)
                {
                    item = uIItemTypes[i];
                }
                else
                {
                    item = GameObject.Instantiate(uiItemType.gameObject, contentNode).GetComponent<UIItemType>();
                    item.gameObject.SetActive(true);
                    uIItemTypes.Add(item);
                }
                item.SetItemType(t);
                onItemTypeShow?.Invoke(t, item);
            }
        }
    }
}