using Sango;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Sango.Tools
{
    public class UITopMenuPanel : MonoBehaviour
    {
        public RectTransform lineObj;
        public UISubTopMenuItem itemObj;

        public UITopMenuPanel subPanel;
        public UIMapEditor rootUI;

        public UISubTopMenuItem currentSelected;

        /// <summary>
        /// 当前面板显示的菜单数据列表
        /// </summary>
        public List<EditorMenuItemData> menuItems { get; private set; }

        CreatePool<RectTransform> linePool;
        CreatePool<UISubTopMenuItem> itemPool;

        private void Awake()
        {
            linePool = new CreatePool<RectTransform>(lineObj);
            itemPool = new CreatePool<UISubTopMenuItem>(itemObj);
            menuItems = new List<EditorMenuItemData>();
        }

        public int Show(List<EditorMenuItemData> menuItemDatas)
        {
            menuItems = menuItemDatas;
            int size = 0;
            for (int i = 0; i < menuItemDatas.Count; i++)
            {
                EditorMenuItemData data = menuItemDatas[i];
                if (data.isSeparator)
                {
                    RectTransform rectTransform = linePool.Create();
                    rectTransform.SetAsLastSibling();
                }
                else
                {
                    UISubTopMenuItem uISubTopMenu = itemPool.Create();
                    if (uISubTopMenu != null)
                    {
                        uISubTopMenu.rootPanel = this;
                        uISubTopMenu.rootUI = rootUI;
                        size = Mathf.Max(size, uISubTopMenu.Show(data));
                        uISubTopMenu.transform.SetAsLastSibling();

                    }
                }
            }
            return size;
        }

        private void OnEnable()
        {
            linePool.Reset();
            itemPool.Reset();
        }

        private void OnDestroy()
        {
            linePool.Clear();
            itemPool.Clear();
        }
    }
}