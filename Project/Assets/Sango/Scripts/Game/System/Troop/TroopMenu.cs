using Sango.UI;
using Sango.Render;
using System.Collections.Generic;
using UnityEngine;
using ContextMenu = Sango.UI.ContextMenu;
namespace Sango.Core.Player
{
    [GameSystem]
    public class TroopMenu : GameSystem
    {
        public Troop TargetTroop { get; set; }
        public void Start(Troop troop, Vector3 startPoint)
        {
            if (!troop.IsAlive) return;
            TargetTroop = troop;
            ContextMenuData.MenuData.Clear();
            GameEvent.OnTroopContextMenuShow?.Invoke(ContextMenuData.MenuData, troop);
            if (!ContextMenuData.MenuData.IsEmpty())
            {
                ContextMenu.Show(ContextMenuData.MenuData, startPoint);
                GameSystemManager.Instance.Push(this);
            }
        }

        /// <summary>
        /// 离开当前命令的时候触发
        /// </summary>
        public override void OnDestroy()
        {
            ContextMenu.CloseAll();
        }

        public override void OnExit()
        {
            OnDestroy();
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {
            switch (eventType)
            {
                case CommandEventType.Cancel:
                case CommandEventType.RClick:
                    {
                        ContextMenu.CloseAll();
                        break;
                    }

                case CommandEventType.Click:
                    {
                        break;
                    }
            }
        }
    }
}
