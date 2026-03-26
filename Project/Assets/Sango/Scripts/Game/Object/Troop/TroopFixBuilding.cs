using System;

namespace Sango.Game
{
    public class TroopFixBuilding : TroopMissionBehaviour
    {
        Cell FinalCell { get; set; }
        public override MissionType MissionType { get { return MissionType.TroopFixBuilding; } }
        public override bool IsMissionComplete
        {
            get
            {
                return (TargetBuilding == null || !TargetBuilding.IsAlive || TargetBuilding.durability >= TargetBuilding.DurabilityLimit);
            }
        }

        public override void Prepare(Troop troop, Scenario scenario)
        {
            if (Troop != troop) Troop = troop;
            if (TargetBuilding == null || TargetBuilding.Id != troop.missionTarget) TargetBuilding = scenario.GetObject<Building>(Troop.missionTarget);
            if (TargetCity == null) TargetCity = troop.BelongCity;
            // 任务完成后,待命
            if (IsMissionComplete)
            {
                Troop.ClearMission();
                return;
            }

            if (FinalCell == null || FinalCell.IsEmpty() || (!FinalCell.IsEmpty() && FinalCell.troop != Troop))
            {
                for (int i = 0; i < 6; i++)
                {
                    Cell cell = Troop.cell.Neighbors[i];
                    if (cell == TargetBuilding.CenterCell)
                    {
                        FinalCell = cell;
                        return;
                    }
                }
                if (troop.MoveRange.Count == 0)
                {
                    scenario.Map.GetMoveRange(troop, troop.MoveRange);
#if SANGO_DEBUG_AI
                    GameAIDebug.Instance.ShowMoveRange(troop.MoveRange, troop);
#endif
                }
                for (int i = 0; i < 6; i++)
                {
                    Cell cell = TargetCell.Neighbors[i];
                    if (cell.IsEmpty() && troop.MoveRange.Contains(cell))
                    {
                        FinalCell = cell;
                        return;
                    }
                }

                FinalCell = null;
            }
        }

        public override bool DoAI(Troop troop, Scenario scenario)
        {
            if (IsMissionComplete)
            {
                Troop.ClearMission();
                return true;
            }

            if (FinalCell != null)
            {
                if (!troop.MoveTo(FinalCell))
                    return false;

                if (!troop.BuildBuilding(TargetBuilding.CenterCell, null))
                    return false;
                return true;

            }

            return troop.TryCloseTo(TargetBuilding.CenterCell);
        }
    }
}
