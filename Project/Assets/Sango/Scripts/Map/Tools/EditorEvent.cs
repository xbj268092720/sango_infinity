using Sango.Core.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Tools
{
    /// <summary>
    /// 地图编辑器
    /// </summary>
    public class EditorEvent : EventBase
    {
        public static EventDelegate<EditorMenuItemData, UIMapEditor> OnEditorTopMenuInit;


    }
}
