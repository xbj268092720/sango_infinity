using Sango.Render;
using Sango.UI;
using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class TroopActionReturn : TroopActionBase
    {
        List<Cell> MovePath { get; set; }

        public TroopActionReturn()
        {
            customMenuName = "返回";
            customMenuOrder = 99999;
        }

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        protected override void OnTroopActionContextMenuShow(IContextMenuData menuData, Troop troop, Cell actionCell)
        {
            if (troop.BelongForce != null && troop.BelongForce.IsPlayer && troop.BelongForce == Scenario.Cur.CurRunForce)
            {
                TargetTroop = troop;
                ActionCell = actionCell;
                //if (actionCell.building != null && actionCell.building.IsSameForce(troop) && actionCell.building.IsCityBase())
                //    menuData.Add("进入", customMenuOrder, actionCell, OnClickMenuItem, IsValid);
                //else
#if UNITY_ANDROID
                menuData.Add(customMenuName, customMenuOrder, actionCell, OnClickMenuItem, IsValid);
#endif
            }
        }

        protected override void OnClickMenuItem(IContextMenuItem contextMenuItem)
        {

        }
    }
}
