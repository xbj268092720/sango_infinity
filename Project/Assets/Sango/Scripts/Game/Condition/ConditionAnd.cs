using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 与条件类，用于组合多个条件，只有当所有条件都满足时才返回true
    /// </summary>
    public class ConditionAnd : Condition
    {
        /// <summary>
        /// 左侧条件
        /// </summary>
        Condition L;

        /// <summary>
        /// 右侧条件
        /// </summary>
        Condition R;

        /// <summary>
        /// 检查所有条件是否都满足
        /// </summary>
        /// <param name="objects">检查条件所需的对象</param>
        /// <returns>所有条件是否都满足</returns>
        public override bool Check(params object[] objects)
        {
            if (L != null && !L.Check(objects)) return false;
            if (R != null && !R.Check(objects)) return false;
            return true;
        }

        /// <summary>
        /// 检查部队、目标和技能相关的所有条件是否都满足
        /// </summary>
        /// <param name="troop">部队对象</param>
        /// <param name="target">目标部队</param>
        /// <param name="skill">技能实例</param>
        /// <returns>所有条件是否都满足</returns>
        public override bool Check(Troop troop, Troop target, SkillInstance skill)
        {
            if (L != null && !L.Check(troop, target, skill)) return false;
            if (R != null && !R.Check(troop, target, skill)) return false;
            return true;
        }

        /// <summary>
        /// 检查技能实例、部队、法术单元格和攻击单元格列表相关的所有条件是否都满足
        /// </summary>
        /// <param name="skillInstance">技能实例</param>
        /// <param name="troop">部队对象</param>
        /// <param name="spellCell">法术单元格</param>
        /// <param name="atkCellList">攻击单元格列表</param>
        /// <returns>所有条件是否都满足</returns>
        public override bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
            if (L != null && !L.Check(skillInstance, troop, spellCell, atkCellList)) return false;
            if (R != null && !R.Check(skillInstance, troop, spellCell, atkCellList)) return false;
            return true;
        }

        /// <summary>
        /// 初始化与条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            JObject Lobj = p.Value<JObject>("L");
            if (Lobj != null)
            {
                L = Condition.Create(Lobj.Value<string>("class"));
                if (L != null)
                    L.Init(Lobj, sangoObjects);
            }

            JObject Robj = p.Value<JObject>("R");
            if (Robj != null)
            {
                R = Condition.Create(Robj.Value<string>("class"));
                if (R != null)
                    R.Init(Robj, sangoObjects);
            }
        }
    }
}
