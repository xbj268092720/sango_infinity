using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Game.Render.UI
{
    public class ContextMenuData : IContextMenuData
    {
        public static ContextMenuData MenuData = new ContextMenuData();

        public List<ContextMenuItem> headList = new List<ContextMenuItem>();

        /// <summary>
        /// order越小越靠前,菜单链父级菜单以最小子级菜单order计算
        /// </summary>
        /// <param name="title"></param>
        /// <param name="order"></param>
        /// <param name="custom"></param>
        /// <param name="action"></param>
        public void Add(string title, int order, object custom, Action<IContextMenuItem> action, bool valide = true)
        {
            string[] menuPath = title.Split('/');
            List<ContextMenuItem> checkList = headList;
            ContextMenuItem checkItem;
            for (int i = 0; i < menuPath.Length; ++i)
            {
                string depthTitle = menuPath[i];
                checkItem = checkList.Find(x => x.title == depthTitle);
                if (checkItem == null)
                {
                    ContextMenuItem contextMenuItem = new()
                    {
                        title = depthTitle,
                        order = order,
                        depth = i,
                        valid = valide
                    };
                    if (i == menuPath.Length - 1)
                    {
                        contextMenuItem.CustomData = custom;
                        contextMenuItem.action = action;
                    }
                    checkList.Add(contextMenuItem);
                    checkItem = contextMenuItem;
                    checkList = checkItem.children;
                }
                else
                {
                    checkItem.valid = checkItem.valid | valide;
                    checkItem.order = Math.Min(order, checkItem.order);
                    checkList = checkItem.children;
                }
            }
        }

        public void Add(string title, int order, object custom)
        {
            Add(title, order, custom, null);
        }
        public void Add(string title, int order)
        {
            Add(title, order, null, null);
        }
        public void Add(string title)
        {
            Add(title, -1, null, null);
        }
        public void AddLine()
        {
            Add(null, -1, null, null);
        }
        public void Clear()
        {
            headList.Clear();
        }

        public bool IsEmpty()
        {
            return headList.Count == 0;
        }
    }
}
