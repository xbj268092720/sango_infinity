namespace Sango.Core
{
    public class TroopDestroyTroop : TroopMissionBehaviour
    {
        public override MissionType MissionType { get { return MissionType.TroopDestroyTroop; } }

        public override bool IsMissionComplete
        {
            get
            {
                return (TargetTroop == null || !TargetTroop.IsAlive || !TargetTroop.IsEnemy(Troop));
            }
        }

        public override void Prepare(Troop troop, Scenario scenario)
        {
            if (Troop != troop) Troop = troop;
            if (TargetTroop == null || TargetTroop.Id != troop.missionTarget) TargetTroop = scenario.troopsSet.Get(Troop.missionTarget);

            // 任务完成后,如果城池被友军拿取则回到创建城池,否则将进入己方目标城池
            if (IsMissionComplete || (!troop.IsPlayer && troop.IsWithOutFood() == 2 && GameRandom.Chance(60)))
            {
                Troop.SetMission(MissionType.TroopReturnCity, Troop.BelongCity.Id);
                Troop.NeedPrepareMission();
            }
            else
            {
                // 获取目标城市周围的敌人
                priorityActionData = TroopAIUtility.PriorityAction(Troop, TargetTroop.cell, scenario, SkillAttackPriority);
            }
        }

        // 技能攻击评分
        public int SkillAttackPriority(Troop troop, SkillInstance skill, Cell target, Cell movetoCell, Cell spellCell)
        {
            int socer = TroopAIUtility.SkillStatusPriority(troop, skill, target, movetoCell, spellCell);
            if (socer > 0)
            {
                if (!target.IsEmpty() && (target.troop != null))
                {
                    if (target.troop == TargetTroop)
                    {
                        socer += 50000;
                        if (movetoCell == troop.cell)
                            socer += 100000;
                    }
                    else
                    {
                        socer = 5;
                    }
                }
                else
                {
                    if (movetoCell == troop.cell && !troop.TroopType.isRange)
                        socer += 50000;
                }
            }
            return socer;
        }

        public override bool DoAI(Troop troop, Scenario scenario)
        {
            // 任务完成后,如果城池被友军拿取则回到创建城池,否则将进入己方目标城池
            if (IsMissionComplete)
            {
                Troop.NeedPrepareMission();
                return false;
            }

            if (priorityActionData != null)
            {
                if (!priorityActionData.moveFinish && !troop.MoveTo(priorityActionData.movetoCell))
                    return false;
                if (!priorityActionData.moveFinish)
                    priorityActionData.moveFinish = true;
                if (!troop.SpellSkill(priorityActionData.skill, priorityActionData.spellCell))
                    return false;
                return true;
            }
            else
            {
                // 向目标前进
                return troop.TryCloseTo(TargetTroop.cell);
            }
        }
    }
}
