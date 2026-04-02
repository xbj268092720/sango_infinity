using Sango.Core.Tools;
using TKNewtonsoft.Json.Linq;

namespace Sango.Core.Action
{
    /// <summary>
    /// 某兵种战法替换成新战法
    /// value: 技能id kinds: 兵种类型
    /// </summary>
    public class TroopAddSkill : ForceTroopActionBase
    {
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            base.Init(p, sangoObjects);
            GameEvent.OnTroopCalculateAttribute += OnTroopCalculateAttribute;
        }

        public override void Clear()
        {
            GameEvent.OnTroopCalculateAttribute -= OnTroopCalculateAttribute;
        }

        void OnTroopCalculateAttribute(Troop troop, Scenario scenario)
        {
            if (Force != troop.BelongForce) return;
            Skill skill = scenario.GetObject<Skill>(value);
            if (kinds == null)
            {
                troop.landSkills.Add(SkillInstance.Create(troop, skill));
                troop.waterSkills.Add(SkillInstance.Create(troop, skill));
            }
            else
            {
                if (kinds.Contains(troop.LandTroopType.kind))
                    troop.landSkills.Add(SkillInstance.Create(troop, skill));
                if (kinds.Contains(troop.WaterTroopType.kind))
                    troop.waterSkills.Add(SkillInstance.Create(troop, skill));
            }
        }
    }
}
