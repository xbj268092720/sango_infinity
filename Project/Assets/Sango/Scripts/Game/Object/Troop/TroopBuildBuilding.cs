using System;

namespace Sango.Game
{
    public class TroopBuildBuilding : TroopMissionBehaviour
    {
        Cell FinalCell { get; set; }
        public override MissionType MissionType { get { return MissionType.TroopBuildBuilding; } }
        public override bool IsMissionComplete
        {
            get
            {
                return (TargetCell == null || (TargetCell.building != null && TargetCell.building.isComplate));
            }
        }

        public override void Prepare(Troop troop, Scenario scenario)
        {
            if (Troop != troop) Troop = troop;
            if (TargetBuildingType == null || TargetBuildingType.Id != troop.missionTarget) TargetBuildingType = scenario.GetObject<BuildingType>(Troop.missionTarget);
            if (TargetCity == null) TargetCity = troop.BelongCity;
            if (TargetCell == null) TargetCell = troop.missionTargetCell;

            // 任务完成后,如果城池被友军拿取则回到创建城池,否则将进入己方目标城池
            if (IsMissionComplete)
            {
                if (Troop.BelongCity.IsSameForce(Troop))
                {
                    Troop.missionType = (int)MissionType.TroopReturnCity;
                    Troop.missionTarget = Troop.BelongCity.Id;
                }
                else
                {
                    Troop.missionType = (int)MissionType.TroopOccupyCity;
                    Troop.missionTarget = TargetCity.Id;
                }
                Troop.NeedPrepareMission();
            }

            if (FinalCell == null || FinalCell.IsEmpty() || (!FinalCell.IsEmpty() && FinalCell.troop != Troop))
            {
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
                Troop.NeedPrepareMission();
                return false;
            }

            if (FinalCell != null)
            {
                if (!troop.MoveTo(FinalCell))
                    return false;

                if (!troop.BuildBuilding(TargetCell, TargetBuildingType))
                    return false;
                return true;

            }

            return troop.TryCloseTo(TargetCell);
        }
    }
}
