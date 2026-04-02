using System.Collections.Generic;
using UnityEngine;
using static Sango.Window;

namespace Sango.Game.Render.UI
{
    public class ContextMenu
    {
        static string[] windowNames = new string[] {
            "window_contextMenu",
            "window_contextMenu_command",
            "window_contextMenu_object",
            "window_contextMenu_system",
        };

        static string currentWindowName;

        public static UIContextMenu Show(ContextMenuData itemData, Vector2 position)
        {
           return Show(itemData.headList, position, ContextMenuType.Common);
        }

        public static UIContextMenu Show(ContextMenuData itemData, Vector2 position, ContextMenuType contentMenuType)
        {
            return Show(itemData.headList, position, contentMenuType);
        }

        public static UIContextMenu Show(List<ContextMenuItem> menuItems, Vector2 position)
        {
            return Show(menuItems, position, ContextMenuType.Common);
        }

        public static UIContextMenu Show(List<ContextMenuItem> menuItems, Vector2 position, ContextMenuType contentMenuType)
        {
            if (menuItems.Count == 0)
                return null;

            currentWindowName = windowNames[(int)contentMenuType];
            WindowInterface windowInterface = Window.Instance.Open(currentWindowName);
            if (windowInterface != null)
            {
                UIContextMenu uIContextMenu = windowInterface.ugui_instance as UIContextMenu;
                if (uIContextMenu != null)
                {
                    Vector2 anchorPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(uIContextMenu.GetComponent<RectTransform>(),
                        position, Game.Instance.UICamera, out anchorPos);

                    ContextMenuItem item = menuItems[0];
                    uIContextMenu.Close(item.depth);
                    menuItems.Sort((a, b) => a.order.CompareTo(b.order));
                    uIContextMenu.Show(anchorPos, item.depth, menuItems);
                    return uIContextMenu;
                }
            }
            return null;

        }

        public static void SetVisible(bool b)
        {
            if (string.IsNullOrEmpty(currentWindowName)) return;

            WindowInterface windowInterface = Window.Instance.GetWindow(currentWindowName);
            if (windowInterface != null)
            {
                windowInterface.SetVisible(b);
            }
        }

        public static bool IsVisible()
        {
            if(string.IsNullOrEmpty(currentWindowName)) return false;

            WindowInterface windowInterface = Window.Instance.GetWindow(currentWindowName);
            if (windowInterface == null) return false;
            return windowInterface.IsVisible();
        }


        public static void Add(ContextMenuData itemData)
        {
            //ContenDatas[itemData.depth].Add(itemData);
        }

        public static bool Close()
        {
            WindowInterface windowInterface = Window.Instance.GetWindow(currentWindowName);
            if (windowInterface != null)
            {
                UIContextMenu uIContextMenu = windowInterface.ugui_instance as UIContextMenu;
                if (uIContextMenu.Close())
                {
                    Window.Instance.Close(currentWindowName);
                    return true;
                }
            }
            return false;
        }

        public static void CloseAll()
        {
            if (string.IsNullOrEmpty(currentWindowName)) return;
            WindowInterface windowInterface = Window.Instance.GetWindow(currentWindowName);
            if (windowInterface != null)
            {
                UIContextMenu uIContextMenu = windowInterface.ugui_instance as UIContextMenu;
                uIContextMenu.Close(0);
                Window.Instance.Close(currentWindowName);
            }
        }

    }
}
