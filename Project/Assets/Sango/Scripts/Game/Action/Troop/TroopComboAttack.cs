using Sango.Core.Tools;
using TKNewtonsoft.Json.Linq;

namespace Sango.Core.Action
{
    /// <summary>
    /// 某兵种类型战法的增减伤害  
    /// value： 增加值(百分比) , Execute为绝对值
    /// </summary>
    public class TroopComboAttack : TroopActionBase
    {
        int skillType;
        int count;
        int probability;
        int currentCount;
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            base.Init(p, sangoObjects);
            skillType = p.Value<int>("skillType");
            count = p.Value<int>("count");
            probability = p.Value<int>("probability");
            currentCount = 0;
        }

        public override void Clear()
        {

        }

        public override void Execute(Trigger trigger)
        {
            if(trigger == null) return;
            if (trigger.ActionTroop != Troop) return;
            if (trigger.ActionSkill == null) return;
            if(!trigger.ActionSkill.IsStrategy() && trigger.ActionSkill.IsNormal())
            if (currentCount < count)
            {
                currentCount++;
                if(GameRandom.Chance(probability))
                {

                }
                
            }
        }
    }
}
