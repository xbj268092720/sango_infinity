using Sango.Core.Tools;
using TKNewtonsoft.Json.Linq;

namespace Sango.Core.Action
{
    /// <summary>
    /// 某兵种类型战法的成功率增加-前 (必中)
    /// value:增加值（百分比）  100则为必中,在计算成功率之前直接(跳过后面的成功率计算)
    /// kinds： 兵种类型  
    /// checkLand： 0:只检查kinds 1:只对landType检查kinds，2只对waterType检查kinds 
    /// isNormal：  0都可以 2非 1是 
    /// condition： 额外条件 支持参数(troop,troop,skill)
    /// </summary>
    public class TroopSkillCalculateSuccess : TroopTroopActionBase
    {
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            base.Init(p, sangoObjects);
            if (value >= 100)
                GameEvent.OnTroopBeforeCalculateSkillSuccess += OnTroopBeforeCalculateSkillSuccess;
            else

                GameEvent.OnTroopAfterCalculateSkillSuccess += OnTroopBeforeCalculateSkillSuccess;
        }

        public override void Clear()
        {
            if (value >= 100)
                GameEvent.OnTroopBeforeCalculateSkillSuccess -= OnTroopBeforeCalculateSkillSuccess;
            else
                GameEvent.OnTroopAfterCalculateSkillSuccess -= OnTroopBeforeCalculateSkillSuccess;

        }

        void OnTroopBeforeCalculateSkillSuccess(Troop troop, SkillInstance skill, Cell cell, OverrideData<int> overrideData)
        {
            if (Force != null && troop.BelongForce != Force) return;
            if (Troop != null && Troop != troop) return;

            if (!CheckIsNormalSkill(skill, isNormal))
                return;

            if (checkLand == 1 && troop.IsInWater)
                return;
            if (checkLand == 2 && !troop.IsInWater)
                return;

            if (checkLand == 0 && kinds != null && !kinds.Contains(troop.LandTroopType.kind) && !kinds.Contains(troop.WaterTroopType.kind))
                return;

            if (checkLand == 1 && kinds != null && !kinds.Contains(troop.LandTroopType.kind))
                return;

            if (checkLand == 2 && kinds != null && !kinds.Contains(troop.WaterTroopType.kind))
                return;

            if (condition != null && !condition.Check(new TroopActionConditionDatabase(skill, cell)))
                return;

            overrideData.Value += value;
        }
    }
}
