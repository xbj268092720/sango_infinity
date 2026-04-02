using TKNewtonsoft.Json.Linq;
using Sango.Core.Tools;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 部队属性比较
    /// attType: 部队属性类型
    /// result: 比较结果 0等于 1大于 -1小于
    /// </summary>
    public class TroopAttributeCompare : Condition
    {
        // 部队属性类型
        string attType;

        // 比较结果: 0等于 1大于 -1小于
        int result;

        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            attType = p.Value<string>("attType");
            result = p.Value<int>("result");
        }

        public override bool Check(params object[] objects)
        {
            if (objects.Length < 3) return false;
            Troop atker = objects[0] as Troop;
            Troop target = objects[1] as Troop;
            if (target == null) return true;
            TroopCompareFunction.TroopCompare compareFunction = TroopCompareFunction.Get(attType);
            if (compareFunction == null)
            {
                return false;
            }

            return compareFunction(atker, target) == result;
        }

        public override bool Check(Troop troop, Troop target, SkillInstance skill)
        {
            if (target == null) return true;
            TroopCompareFunction.TroopCompare compareFunction = TroopCompareFunction.Get(attType);
            if (compareFunction == null)
            {
                return false;
            }
            return compareFunction(troop, target) == result;
        }

        public override bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
            if (spellCell.troop == null) return true;
            TroopCompareFunction.TroopCompare compareFunction = TroopCompareFunction.Get(attType);
            if (compareFunction == null)
            {
                return false;
            }
            return compareFunction(troop, spellCell.troop) == result;
        }
    }
}
