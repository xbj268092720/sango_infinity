using Sango;
using Sango.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sango.Tools
{
    public class UIMapEditor : MonoBehaviour
    {
        public Color bgColor = new Color(0.6588235f, 0.7843137f, 0.7843137f);

        public EditorMenuItemData topMenuData = new EditorMenuItemData();
        public RectTransform topMenuRoot;

        public UITopMenuItem UITopMenu;
        CreatePool<UITopMenuItem> itemPool;
        public UITopMenuItem currentTopMenu;

        public UITopMenuPanel UITopMenuPanel;
        public CreatePool<UITopMenuPanel> panelPool;
        UITopMenuPanel currentPanel;

        int bUpdateBarInNextFrame = 0;

        private void Awake()
        {
            itemPool = new CreatePool<UITopMenuItem>(UITopMenu);
            panelPool = new CreatePool<UITopMenuPanel>(UITopMenuPanel);

            // 初始化顶部菜单
            InitTopMenuBar();
        }

        public void InitTopMenuBar()
        {
            topMenuData.menuItems.Clear();
            EditorEvent.OnEditorTopMenuInit?.Invoke(topMenuData, this);
            Show();
        }

        public void AddTopMenu(string name, Action action)
        {
            topMenuData.Add(name, action);
        }

        public void Show()
        {
            itemPool.Reset();
            for (int i = 0; i < topMenuData.menuItems.Count; i++)
            {
                EditorMenuItemData itemData = topMenuData.menuItems[i];
                UITopMenuItem item = itemPool.Create();
                item.Show(itemData);
                item.action = ShowMenuPanel;
                item.rootUI = this;
            }
            bUpdateBarInNextFrame = 3;
        }

        public void ShowMenuPanel(UITopMenuItem item)
        {
            panelPool.Reset();
            UITopMenuPanel panel = panelPool.Create();
            RectTransform targetRect = item.transform as RectTransform;
            RectTransform curRect = panel.transform as RectTransform;

            panel.rootUI = this;
            panel.currentSelected = null;

            int maxSize = panel.Show(item.data.menuItems);
            Vector2 size = curRect.sizeDelta;
            size.x = maxSize;
            curRect.sizeDelta = size;
            curRect.anchoredPosition = targetRect.anchoredPosition - new Vector2(0, targetRect.sizeDelta.y);
            currentPanel = panel;
        }

        public void Clear()
        {
            panelPool.Reset();
        }

        /// <summary>
        /// 关闭菜单
        /// </summary>
        public void CloseMenu()
        {
            panelPool.Reset();
            if (currentTopMenu != null)
            {
                currentTopMenu.bg.color = Color.white;
                currentTopMenu = null;
            }
        }

        private void Update()
        {
            if (bUpdateBarInNextFrame > 0)
            {
                bUpdateBarInNextFrame--;
                if (bUpdateBarInNextFrame == 0)
                {
                    topMenuRoot.gameObject.SetActive(false);
                    topMenuRoot.gameObject.SetActive(true);
                }
            }

            if (currentPanel != null && Input.GetMouseButtonDown(0))
            {
                UITopMenuPanel panel = currentPanel;
                while (panel != null)
                {
                    RectTransform rectTransform = panel.transform as RectTransform;
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, App.Instance.UICamera))
                        return;
                    panel = panel.subPanel;
                }

                currentPanel = null;
                panelPool.Reset();
            }
        }
    }
}