using Sango.Core;
using UnityEngine;

namespace Sango.Render
{
    public class TroopSpellSkillCriticalEvent : RenderEventBase
    {
        public Troop troop;
        public SkillInstance skill;
        public Cell spellCell;
        public int criticalFactor;
        private bool isAction = false;
        private float time = 0;
        private bool isCritWindowShown = false;
        private float critWindowTime = 0;

        public void Init(Troop troop, SkillInstance skill, Cell spellCell, int criticalFactor)
        {
            this.troop = troop;
            this.skill = skill;
            this.spellCell = spellCell;
            this.criticalFactor = criticalFactor;
            this.isAction = false;
            this.time = 0;
            IsDone = false;
        }
        public override void Enter(Scenario scenario)
        {
            isAction = false;
            time = 0;
            isCritWindowShown = false;
            critWindowTime = 0;

            // 显示技能效果
            if (IsVisible())
            {
                troop.Render.SetSmokeShow(true);
            }

            // 打开暴击窗口
            Sango.Window.Instance.Open("window_skill_crit", this);
        }

        public override void Exit(Scenario scenario)
        {
            Sango.Window.Instance.Close("window_skill_crit");
            if (IsVisible())
            {
                troop.Render.SetSmokeShow(false);
            }
        }

        public override bool IsVisible()
        {
            return troop.Render.IsVisible();
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            if (!IsVisible() || Input.GetMouseButtonDown(0))
            {
                Action();
                troop?.Render?.SetAniShow(0);
                IsDone = true;
                return IsDone;
            }
            
            // 处理暴击窗口显示
            if (!isCritWindowShown)
            {
                critWindowTime += deltaTime;
                if (critWindowTime >= 1f)
                {
                    isCritWindowShown = true;
                   
                    if (skill.costEnergy > 0)
                        troop.Render.ShowSkill(skill, false, true);
                }
                return IsDone;
            }
            
            IsDone = skill.UpdateRender(troop, spellCell, scenario, time, Action);
            time += deltaTime;
            return IsDone;
        }


        public void Action()
        {
            if (isAction) return;
            skill.Action(troop, spellCell, criticalFactor);
            isAction = true;
        }

    }
}
