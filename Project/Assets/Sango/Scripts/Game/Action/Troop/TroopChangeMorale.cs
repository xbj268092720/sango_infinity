using Sango.Core.Tools;
using TKNewtonsoft.Json.Linq;

namespace Sango.Core.Action
{
    /// <summary>
    /// 某兵种类型战法的增减伤害  
    /// value： 增加值(百分比) , Execute为绝对值
    /// </summary>
    public class TroopChangeMorale : TroopActionBase
    {
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            base.Init(p, sangoObjects);
            GameEvent.OnTroopChangeMorale += OnTroopChangeMorale;
        }

        public override void Clear()
        {
            GameEvent.OnTroopChangeMorale -= OnTroopChangeMorale;
        }

        void OnTroopChangeMorale(Troop troop, int morale, OverrideData<int> overrideData)
        {
            overrideData.Value = overrideData.Value * (100 + value) / 100;
        }
        public override void Execute(Trigger trigger)
        {
            if(trigger == null) return;

            if (trigger.ActionTroop != Troop) return;

            if (trigger.TargetTroop == null) return;

            trigger.TargetTroop.ChangeMorale(-value);
        }
    }
}
