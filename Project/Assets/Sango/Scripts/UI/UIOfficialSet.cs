using Sango.Core.Player;
using UnityEngine;

using Sango.Core;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Sango.UI
{
    public class UIOfficialSet : UGUIWindow
    {
        public Text title;
        public List<ObjectSortTitle> sortItems;
        public UIObjectListItem[] uIObjectListItems;
        public Scrollbar scrollbar;
        public Scrollbar scrollbar_h;
        protected List<UISortButton> sortButtonPool = new List<UISortButton>();
        public UISortButton sortTitleItem;
        public GameObject selectSortBtn;
        public RectTransform sorltTitleTransform;
        public RectTransform maskRect;
        public RectTransform[] contentRect;
        protected int startIndex = 0;
        protected int itemWidth = 0;
        protected int itemCount = 0;
        
        public RectTransform btnRoot;
        public UIButtonItem[] buttons;

        public UIOfficialItem[] uiOfficialItems;
        public Scrollbar official_scrollbar;
        protected int officialIndex = 0;

        public GameObject officialSelectPanel;
        public UIOfficialItem[] uiSelectOfficialItems;
        public UIPersonItem uIPersonItem;


        RectTransform[] uIObjectListItemsRect;
        bool dragFlag = false;
        UIObjectListItem currentSelectItem;


        CityUpgradeOfficial cityUpgradeOfficial;

        protected override void Awake()
        {
            base.Awake();
            //uIObjectListItemsRect = new RectTransform[uIObjectListItems.Length];
            //for (int i = 0; i < uIObjectListItems.Length; i++)
            //{
            //    uIObjectListItemsRect[i] = uIObjectListItems[i].GetComponent<RectTransform>();
            //}
        }

        public override void OnOpen()
        {
            base.OnOpen();
            cityUpgradeOfficial = GameSystem.GetSystem<CityUpgradeOfficial>();
            title.text = cityUpgradeOfficial.customTitleName;
            sortItems = cityUpgradeOfficial.customTitleList;

            itemWidth = GetContentWidth();
            itemCount = uIObjectListItems.Length;

            bool show_scrollbar_h = maskRect.rect.width < itemWidth;
            scrollbar_h.gameObject.SetActive(show_scrollbar_h);
            if (show_scrollbar_h)
            {
                itemCount--;
                scrollbar_h.size = (float)maskRect.rect.width / (float)itemWidth;
                scrollbar_h.SetValueWithoutNotify(0);
                uIObjectListItems[uIObjectListItems.Length - 1].gameObject.SetActive(false);
            }
            else
            {
                uIObjectListItems[uIObjectListItems.Length - 1].gameObject.SetActive(true);
            }

            startIndex = 0;
            int dataCount = cityUpgradeOfficial.targetList.Count;
            if (dataCount < itemCount)
            {
                scrollbar.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                scrollbar.transform.parent.gameObject.SetActive(true);
                scrollbar.size = (float)itemCount / (float)dataCount;
                scrollbar.SetValueWithoutNotify(0);
            }

            officialIndex = 0;
            dataCount = cityUpgradeOfficial.upgradeList.Count;
            if (dataCount < uiOfficialItems.Length)
            {
                official_scrollbar.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                official_scrollbar.transform.parent.gameObject.SetActive(true);
                official_scrollbar.size = (float)itemCount / (float)dataCount;
                official_scrollbar.SetValueWithoutNotify(0);
            }

            // 重置状态和位置
            for (int j = 0; j < uIObjectListItems.Length; j++)
            {
                UIObjectListItem listItem = uIObjectListItems[j];
                Vector2 p = listItem.contentRect.anchoredPosition;
                p.x = 0;
                listItem.contentRect.anchoredPosition = p;
                listItem.SetOver(false);
            }

            UpdateSortContent();
            OnScrollBarValueChange(0);

            foreach (RectTransform r in contentRect)
            {
                Vector2 p = r.anchoredPosition;
                p.x = 0;
                r.anchoredPosition = p;
            }

            //bool hasButtons = objectSelectSystem.buttonDatas != null;
            //btnRoot.gameObject.SetActive(hasButtons);
            //if (hasButtons)
            //{
            //    for (int i = 0; i < buttons.Length; i++)
            //    {
            //        UIButtonItem buttonItem = buttons[i];
            //        if (i >= objectSelectSystem.buttonDatas.Count)
            //        {
            //            buttonItem.gameObject.SetActive(false);
            //            continue;
            //        }
            //        ObjectsDisplaySystem.ButtonData buttonData = objectSelectSystem.buttonDatas[i];
            //        buttonItem.gameObject.SetActive(true);
            //        buttonItem.SetTitle(buttonData.title).SetStyle(buttonData.style).BindAction(buttonData.action);
            //    }
            //}
        }

        public int GetContentWidth()
        {
            int w = 0;
            for (int i = 0; i < sortItems.Count; i++)
            {
                ObjectSortTitle sortTitle = sortItems[i];
                w += sortTitle.width;
            }
            return w + 24;
        }

        public void UpdateSortContent()
        {
            for (int j = 0; j < uIObjectListItems.Length; j++)
            {
                UIObjectListItem listItem = uIObjectListItems[j];
                listItem.Clear();
            }
            for (int i = 0; i < sortItems.Count; i++)
            {
                ObjectSortTitle sortTitle = sortItems[i];
                UISortButton uIPersonSortButton;

                if (i == 0)
                {
                    uIPersonSortButton = sortTitleItem;
                }
                else
                {
                    if (i - 1 < sortButtonPool.Count)
                        uIPersonSortButton = sortButtonPool[i - 1];
                    else
                        uIPersonSortButton = CreateSortButtonItem();
                }



                uIPersonSortButton.gameObject.SetActive(true);
                uIPersonSortButton.Clear().SetWidth(sortTitle.width).SetName(sortTitle.name);

                uIPersonSortButton.onClick = (up) =>
                {
                    cityUpgradeOfficial.targetList.Sort(sortTitle.Sort);
                    if (!up) cityUpgradeOfficial.targetList.Reverse();
                    scrollbar.SetValueWithoutNotify(0);
                    OnScrollBarValueChange(0);
                };

                if (i > 0)
                {
                    for (int j = 0; j < itemCount; j++)
                    {
                        UIObjectListItem listItem = uIObjectListItems[j];
                        listItem.Add("", sortTitle.width);
                    }
                }
                else
                {
                    for (int j = 0; j < itemCount; j++)
                    {
                        UIObjectListItem listItem = uIObjectListItems[j];
                        listItem.textItem.SetWidth(sortTitle.width);
                    }
                }
            }

            for (int i = sortItems.Count - 1; i < sortButtonPool.Count; i++)
                sortButtonPool[i].gameObject.SetActive(false);
        }

        protected UISortButton CreateSortButtonItem()
        {
            GameObject btn = GameObject.Instantiate(sortTitleItem.gameObject, sorltTitleTransform);
            UISortButton sortBtn = btn.GetComponent<UISortButton>();
            sortButtonPool.Add(sortBtn);
            return sortBtn;
        }

        public void UpdateItemStartIndex(int startIndex)
        {
            for (int i = 0; i < itemCount; i++)
            {
                UIObjectListItem listItem = uIObjectListItems[i];
                int destIndex = i + startIndex;
                listItem.index = destIndex;
                if (destIndex < cityUpgradeOfficial.targetList.Count)
                {
                    SangoObject sango = cityUpgradeOfficial.targetList[destIndex];
                    //bool isSelected = objectSelectSystem.selected.Contains(sango);
                    for (int j = 0; j < sortItems.Count; j++)
                    {
                        ObjectSortTitle sortTitle = sortItems[j];
                        listItem.Set(j, sortTitle.GetValueStr(sango));
                    }
                    listItem.SetSelected(false);
                }
                else
                {
                    listItem.SetSelected(false);
                }

            }
        }

        public void UpdateOfficialStartIndex(int startIndex)
        {
            for (int i = 0; i < uiOfficialItems.Length; i++)
            {
                UIOfficialItem listItem = uiOfficialItems[i];
                int destIndex = i + startIndex;
                listItem.index = destIndex;
                if (destIndex < cityUpgradeOfficial.upgradeList.Count)
                {
                    CityUpgradeOfficial.UpgradeOfficial data = cityUpgradeOfficial.upgradeList[destIndex];
                    listItem.SetPerson(data.person, data.official);
                }
                else
                {
                    listItem.SetPerson(null, null);
                }

            }
        }


        public void OnSure()
        {

        }

        public void OnCancel()
        {

        }

        public void OnPersonListItemPressDown(UIObjectListItem item)
        {
            item.SetPressd(true);
            currentSelectItem = item;
            dragFlag = !item.IsSelected();
        }

        public void OnPersonListItemPressUp(UIObjectListItem item)
        {
            item.SetPressd(false);
            //for (int i = 0; i < itemCount; i++)
            //{
            //    RectTransform itemRect = uIObjectListItemsRect[i];
            //    UIObjectListItem listItem = uIObjectListItems[i];
            //    if (listItem == currentSelectItem && RectTransformUtility.RectangleContainsScreenPoint(itemRect, Input.mousePosition, Sango.Core.Game.Instance.UICamera))
            //    {
            //        OnPersonListSelected(item);
            //        break;
            //    }
            //}
        }

        public void OnPersonListItemPointEnter(UIObjectListItem item)
        {
            item.SetOver(true);
        }
        public void OnPersonListItemPointExit(UIObjectListItem item)
        {
            item.SetOver(false);
        }

        public void OnPersonListSelected(UIObjectListItem item)
        {
            //if (item.index >= objectSelectSystem.Objects.Count)
            //    return;

            //if (!item.IsSelected() && objectSelectSystem.IsPersonLimit())
            //{
            //    int lastIndex = objectSelectSystem.RemoveFront();
            //    if (lastIndex >= 0)
            //    {
            //        //for (int i = 0; i < itemCount; i++)
            //        //{
            //        //    UIObjectListItem listItem = uIObjectListItems[i];
            //        //    int destIndex = i + startIndex;
            //        //    if (destIndex == lastIndex)
            //        //    {
            //        //        listItem.SetSelected(false);
            //        //        break;
            //        //    }
            //        //}
            //    }
            //    item.SetSelected(true);
            //    objectSelectSystem.Add(item.index);
            //    return;
            //}

            //item.SetSelected(!item.IsSelected());
            //if (item.IsSelected())
            //{
            //    objectSelectSystem.Add(item.index);
            //}
            //else
            //{
            //    objectSelectSystem.Remove(item.index);
            //}
        }

        public void OnDragPersonListSelected(UIObjectListItem item)
        {
            //if (dragFlag && objectSelectSystem.IsPersonLimit())
            //    return;

            //if (!dragFlag && objectSelectSystem.IsPersonEmpty())
            //    return;

            //if (item.index >= objectSelectSystem.Objects.Count)
            //    return;

            //if (item.IsSelected() && !dragFlag)
            //{
            //    item.SetSelected(false);
            //    objectSelectSystem.Remove(item.index);
            //}
            //else if (!item.IsSelected() && dragFlag)
            //{
            //    item.SetSelected(true);
            //    objectSelectSystem.Add(item.index);
            //}

            //for (int i = 0; i < itemCount; i++)
            //{
            //    RectTransform itemRect = uIObjectListItemsRect[i];
            //    UIObjectListItem listItem = uIObjectListItems[i];
            //    if (listItem != item && RectTransformUtility.RectangleContainsScreenPoint(itemRect, Input.mousePosition, Sango.Core.Game.Instance.UICamera))
            //    {
            //        if (listItem.IsSelected() && !dragFlag)
            //        {
            //            listItem.SetSelected(false);
            //            objectSelectSystem.Remove(listItem.index);
            //        }
            //        else if (!listItem.IsSelected() && dragFlag)
            //        {
            //            listItem.SetSelected(true);
            //            objectSelectSystem.Add(listItem.index);
            //        }
            //    }
            //}
        }

        public void OnScrollBarValueChange(float value)
        {
            startIndex = (int)UnityEngine.Mathf.Lerp(0, cityUpgradeOfficial.targetList.Count - itemCount, value);
            UpdateItemStartIndex(startIndex);
        }

        public void OnScrollBar_H_ValueChange(float value)
        {
            float dis = (float)itemWidth - (float)maskRect.rect.width;
            float pos = -(int)UnityEngine.Mathf.Lerp(0, dis, value);
            foreach (RectTransform r in contentRect)
            {
                Vector2 p = r.anchoredPosition;
                p.x = pos;
                r.anchoredPosition = p;
            }

            for (int j = 0; j < uIObjectListItems.Length; j++)
            {
                UIObjectListItem listItem = uIObjectListItems[j];
                Vector2 p = listItem.contentRect.anchoredPosition;
                p.x = pos;
                listItem.contentRect.anchoredPosition = p;
            }
        }

        public void OnOfficial_ScrollBarValueChange(float value)
        {
            officialIndex = (int)UnityEngine.Mathf.Lerp(0, cityUpgradeOfficial.upgradeList.Count - uiOfficialItems.Length, value);
            UpdateOfficialStartIndex(officialIndex);
        }
    }
}
