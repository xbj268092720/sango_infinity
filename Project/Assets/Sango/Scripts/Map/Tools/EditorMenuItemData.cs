using Sango.Core.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools
{
    public class EditorMenuItemData
    {
        public string displayName;
        public string actionId;
        public string shortcut; // 快捷键显示文本
        public bool isSeparator = false;
        public bool isToggle = false;
        public bool isToggleOn = false;
        public bool isValid = true; // 菜单项是否可用，false时置灰
        public string icon; // 可选的图标
        public List<EditorMenuItemData> menuItems = new List<EditorMenuItemData>();
        public System.Action action;
        public Action<bool> onToggleChanged; // Toggle状态改变时的回调
        public Action<bool> onValidChanged; // 有效性状态改变时的回调

        /// <summary>
        /// 添加带Toggle属性的菜单项
        /// </summary>
        /// <param name="name">菜单名称，支持路径格式如"文件/新建"</param>
        /// <param name="action">点击回调</param>
        /// <param name="isToggle">是否为Toggle类型</param>
        /// <param name="isToggleOn">Toggle是否选中</param>
        /// <param name="onToggleChanged">Toggle状态改变时的回调</param>
        public void Add(string name, System.Action action, bool isToggle, bool isToggleOn, Action<bool> onToggleChanged = null)
        {
            // 处理分隔线
            if (name == "-")
            {
                menuItems.Add(new EditorMenuItemData() { displayName = "-", isSeparator = true });
                return;
            }
            string[] subName = name.Split('/');
            Add(subName, action, isToggle, isToggleOn, onToggleChanged);
        }

        private void Add(string[] name, System.Action action, bool isToggle, bool isToggleOn, Action<bool> onToggleChanged)
        {
            string mainName = name[0];
            EditorMenuItemData data = menuItems.Find(x => x.displayName == mainName);
            if (data == null)
            {
                data = new EditorMenuItemData()
                {
                    displayName = mainName,
                    actionId = (menuItems.Count + 1).ToString(),
                    isToggle = isToggle,
                    isToggleOn = isToggleOn,
                    onToggleChanged = onToggleChanged
                };
                menuItems.Add(data);
            }
            if (name.Length > 1)
            {
                string[] co = new string[name.Length - 1];
                System.Array.Copy(name, 1, co, 0, co.Length);
                data.Add(co, action, isToggle, isToggleOn, onToggleChanged);
            }
            else
            {
                data.action = action;
                data.isToggle = isToggle;
                data.isToggleOn = isToggleOn;
                data.onToggleChanged = onToggleChanged;
            }
        }

        /// <summary>
        /// 切换Toggle状态
        /// </summary>
        public void Toggle()
        {
            if (!isToggle) return;
            isToggleOn = !isToggleOn;
            onToggleChanged?.Invoke(isToggleOn);
        }

        /// <summary>
        /// 设置Toggle状态
        /// </summary>
        /// <param name="isOn">新的Toggle状态</param>
        /// <param name="notify">是否触发回调</param>
        public void SetToggle(bool isOn, bool notify = true)
        {
            if (!isToggle) return;
            isToggleOn = isOn;
            if (notify)
            {
                onToggleChanged?.Invoke(isToggleOn);
            }
        }

        /// <summary>
        /// 设置菜单项有效性
        /// </summary>
        /// <param name="isValid">是否可用</param>
        /// <param name="notify">是否触发回调</param>
        public void SetValid(bool isValid, bool notify = true)
        {
            if (this.isValid == isValid) return;
            this.isValid = isValid;
            if (notify)
            {
                onValidChanged?.Invoke(isValid);
            }
        }

        /// <summary>
        /// 根据路径查找菜单项
        /// </summary>
        /// <param name="path">路径，格式如"文件/新建"</param>
        /// <returns>找到的菜单项，未找到返回null</returns>
        public EditorMenuItemData Find(string path)
        {
            string[] parts = path.Split('/');
            return FindRecursive(parts);
        }

        private EditorMenuItemData FindRecursive(string[] parts)
        {
            if (parts.Length == 0) return null;

            EditorMenuItemData result = menuItems.Find(x => x.displayName == parts[0]);
            if (result == null) return null;

            if (parts.Length == 1) return result;

            string[] remaining = new string[parts.Length - 1];
            System.Array.Copy(parts, 1, remaining, 0, remaining.Length);
            return result.FindRecursive(remaining);
        }

        /// <summary>
        /// 添加菜单项（普通类型）
        /// </summary>
        public void Add(string name, System.Action action)
        {

            string[] subName = name.Split('/');
            Add(subName, action);
        }

        private void Add(string[] name, System.Action action)
        {
            string mainName = name[0];

            // 处理分隔线
            if (mainName == "-")
            {
                menuItems.Add(new EditorMenuItemData() { displayName = "-", isSeparator = true });
                return;
            }

            EditorMenuItemData data = menuItems.Find(x => x.displayName == mainName);
            if (data == null)
            {
                data = new EditorMenuItemData()
                {
                    displayName = mainName,
                    actionId = (menuItems.Count + 1).ToString(),
                };
                menuItems.Add(data);
            }
            if (name.Length > 1)
            {
                string[] co = new string[name.Length - 1];
                System.Array.Copy(name, 1, co, 0, co.Length);
                data.Add(co, action);
            }
            else
            {
                data.action = action;
            }
        }
    }
}
