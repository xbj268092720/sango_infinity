using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Sango.Tools
{
    public class UISubTopMenuItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public Image bg;
        public Image icon;
        public Image next;
        public Text label;
        public Text key;
        public EditorMenuItemData menuData;
        public UITopMenuPanel rootPanel;
        public UIMapEditor rootUI;

        // 视觉配置
        private Color _toggleOnColor = new Color(0.8f, 0.9f, 1f);
        private Color _toggleOffColor = new Color(1f, 1f, 1f);
        private Color _disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f); // 置灰颜色
        private Color _normalTextColor = new Color(0.0f, 0.0f, 0.0f);
        private Color _disabledTextColor = new Color(0.5f, 0.5f, 0.5f);

        public void OnPointerClick(PointerEventData eventData)
        {
            // 无效菜单项不响应点击
            if (!menuData.isValid)
            {
                return;
            }

            // 如果是Toggle类型的菜单项
            if (menuData.isToggle)
            {
                // 切换Toggle状态
                menuData.Toggle();
                
                // 更新图标显示
                UpdateToggleIcon();
                
                // 如果有回调则执行
                if (menuData.action != null)
                {
                    menuData.action();
                }
            }
            else
            {
                // 普通菜单项
                if(menuData.action != null)
                {
                    menuData.action();
                }
            }
            rootUI.CloseMenu();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // 无效菜单项不响应悬停
            if (!menuData.isValid)
            {
                return;
            }

            if (rootPanel == null || rootUI == null) return;
            UITopMenuPanel panel = rootPanel.subPanel;
            while (panel != null)
            {
                rootUI.panelPool.Recycle(panel);
                UITopMenuPanel lastPanel = panel;
                panel = panel.subPanel;
                if (panel != null)
                    lastPanel.subPanel = null;
            }
            rootPanel.subPanel = null;


            if (rootPanel.currentSelected != null)
                rootPanel.currentSelected.bg.color = Color.white;
            rootPanel.currentSelected = this;
            bg.color = rootUI.bgColor;

            if (menuData.menuItems.Count > 0)
            {
                UITopMenuPanel uITopMenuPanel = rootUI.panelPool.Create();
                rootPanel.subPanel = uITopMenuPanel;
                rootPanel.rootUI = rootUI;
                
                uITopMenuPanel.currentSelected = null;
                RectTransform targetRect = transform as RectTransform;
                RectTransform curRect = uITopMenuPanel.transform as RectTransform;
                RectTransform rootPanelRect = rootPanel.transform as RectTransform;
                uITopMenuPanel.rootUI = rootUI;

                int maxSize = uITopMenuPanel.Show(menuData.menuItems);
                Vector2 size = curRect.sizeDelta;
                size.x = maxSize;
                curRect.sizeDelta = size;
                curRect.anchoredPosition = rootPanelRect.anchoredPosition + targetRect.anchoredPosition + new Vector2(targetRect.sizeDelta.x, 0);
            }
        }

        public int Show(EditorMenuItemData data)
        {
            menuData = data;
            label.text = data.displayName;
            bg.color = Color.white;

            // 根据有效性更新视觉状态
            UpdateValidVisual();

            if (!string.IsNullOrEmpty(data.shortcut))
            {
                key.text = data.shortcut;
            }
            else
            {
                key.text = "";
            }

            if (data.isToggle)
            {
                icon.sprite = Sango.Loader.ObjectLoader.LoadObject<UnityEngine.Sprite>("Assets/UI/AtlasTexture/4846-7/4846-7_37.png");
                icon.enabled = true;
                UpdateToggleVisual();
            }
            else
            {
                bool hasIcon = !string.IsNullOrEmpty(data.icon);
                icon.enabled = hasIcon;
                if (hasIcon)
                {
                    icon.sprite = Sango.Loader.ObjectLoader.LoadObject<UnityEngine.Sprite>(data.icon);
                }
            }

            next.enabled = data.menuItems.Count > 0 && data.isValid;
            return 32 + (int)label.preferredWidth + 20 + (int)key.preferredWidth + 30;
        }

        /// <summary>
        /// 更新Toggle图标显示
        /// </summary>
        private void UpdateToggleIcon()
        {
            if (menuData == null || !menuData.isToggle) return;
            icon.enabled = menuData.isToggleOn;
        }

        /// <summary>
        /// 更新Toggle视觉状态
        /// </summary>
        private void UpdateToggleVisual()
        {
            if (menuData == null || !menuData.isToggle) return;
            
            // 更新图标显示
            icon.enabled = menuData.isToggleOn;
            
            // 可选：根据状态调整背景颜色
            // bg.color = menuData.isToggleOn ? _toggleOnColor : _toggleOffColor;
        }

        /// <summary>
        /// 更新有效性视觉状态（置灰效果）
        /// </summary>
        private void UpdateValidVisual()
        {
            if (menuData == null) return;

            if (menuData.isValid)
            {
                // 正常状态
                label.color = _normalTextColor;
                key.color = _normalTextColor;
                // bg.color = Color.white;
                // 图标恢复正常
                SetGraphicsAlpha(1f);
            }
            else
            {
                // 置灰状态
                label.color = _disabledTextColor;
                key.color = _disabledTextColor;
                bg.color = _disabledColor;
                // 图标置灰
                SetGraphicsAlpha(0.5f);
            }
        }

        /// <summary>
        /// 设置图形元素透明度
        /// </summary>
        private void SetGraphicsAlpha(float alpha)
        {
            Color color = label.color;
            color.a = alpha;
            label.color = color;

            color = key.color;
            color.a = alpha;
            key.color = color;

            color = bg.color;
            color.a = alpha;
            bg.color = color;

            if (icon.enabled)
            {
                color = icon.color;
                color.a = alpha;
                icon.color = color;
            }
        }

        /// <summary>
        /// 刷新Toggle状态（从外部调用，用于同步UI状态）
        /// </summary>
        public void RefreshToggleState()
        {
            UpdateToggleVisual();
        }

        /// <summary>
        /// 刷新有效性状态（从外部调用，用于同步UI状态）
        /// </summary>
        public void RefreshValidState()
        {
            UpdateValidVisual();
        }
    }
}