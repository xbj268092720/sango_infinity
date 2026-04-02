
using Sango.Render;
using System.Collections.Generic;

namespace Sango.Core.Player
{
    /// <summary>
    /// 部队委任-建造建筑
    /// </summary>
    [GameSystem]
    public class TroopInteractiveBuildingFix : TroopInteractiveBase
    {
        List<Cell> MovePath { get; set; }

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        protected override bool Check(Troop troop, Cell actionCell)
        {
            if (actionCell.building == null || actionCell.building.IsCityBase() || actionCell.building.BelongForce != troop.BelongForce) return false;
            if( actionCell.building.isUpgrading || actionCell.building.durability >= actionCell.building.DurabilityLimit) return false;

            content = string.Format("即前往将对{0}进行修补。\n确定吗？", actionCell.building.Name);
            return true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            MovePath = GameSystem.GetSystem<TroopSystem>().movePath;
            if (MovePath.Count <= 1)
            {
                OnMoveDone();
                return;
            }
            Cell start = TargetTroop.cell;
            for (int i = 1; i < MovePath.Count; i++)
            {
                bool isLast = i == MovePath.Count - 1;
                Cell dest = MovePath[i];
                TroopMoveEvent @event = RenderEvent.Instance.Create<TroopMoveEvent>();
                @event.Init(TargetTroop, start, dest, isLast, isLast ? OnMoveDone : null);
                RenderEvent.Instance.Add(@event);
                start = dest;
            }

        }

        public override void OnDestroy()
        {

        }

        public void OnMoveDone()
        {
            TargetTroop.ActionOver = true;
            TargetTroop.Render?.UpdateRender();

            if (ActionCell.building != null && ActionCell.building.IsSameForce(TargetTroop) && ActionCell.building.IsCityBase())
                TargetTroop.EnterCity(ActionCell.building as City);

            Done();
        }
    }
}
