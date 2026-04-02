using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 部队兵力检查条件
    /// checkTarget: 检查目标 (self/target)
    /// value: 兵力值
    /// operator: 比较运算符 (eq/gt/lt/gte/lte)
    /// </summary>
    public class TroopStrengthCheck : Condition
    {
        /// <summary>
        /// 检查目标 (self: 自己, target: 目标)
        /// </summary>
        string checkTarget;
        
        /// <summary>
        /// 兵力值
        /// </summary>
        int value;
        
        /// <summary>
        /// 比较运算符 (eq: 等于, gt: 大于, lt: 小于, gte: 大于等于, lte: 小于等于)
        /// </summary>
        string @operator;

        /// <summary>
        /// 初始化部队兵力检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            checkTarget = p.Value<string>("checkTarget") ?? "self";
            value = p.Value<int>("value");
            @operator = p.Value<string>("operator") ?? "gte";
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <param name="objects">检查条件所需的对象</param>
        /// <returns>条件是否满足</returns>
        public override bool Check(params object[] objects)
        {
            Troop troop = null;
            
            if (checkTarget == "self" && objects.Length > 0)
            {
                troop = objects[0] as Troop;
            }
            else if (checkTarget == "target" && objects.Length > 1)
            {
                troop = objects[1] as Troop;
            }
            
            if (troop == null)
                return false;
            
            return CheckCondition(troop.troops);
        }

        /// <summary>
        /// 检查部队、目标和技能相关的条件
        /// </summary>
        /// <param name="troop">部队对象</param>
        /// <param name="target">目标部队</param>
        /// <param name="skill">技能实例</param>
        /// <returns>条件是否满足</returns>
        public override bool Check(Troop troop, Troop target, SkillInstance skill)
        {
            Troop checkTroop = null;
            
            if (checkTarget == "self")
            {
                checkTroop = troop;
            }
            else if (checkTarget == "target")
            {
                checkTroop = target;
            }
            
            if (checkTroop == null)
                return false;
            
            return CheckCondition(checkTroop.troops);
        }

        /// <summary>
        /// 检查技能实例、部队、法术单元格和攻击单元格列表相关的条件
        /// </summary>
        /// <param name="skillInstance">技能实例</param>
        /// <param name="troop">部队对象</param>
        /// <param name="spellCell">法术单元格</param>
        /// <param name="atkCellList">攻击单元格列表</param>
        /// <returns>条件是否满足</returns>
        public override bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
            Troop checkTroop = null;
            
            if (checkTarget == "self")
            {
                checkTroop = troop;
            }
            else if (checkTarget == "target" && spellCell != null)
            {
                checkTroop = spellCell.troop;
            }
            
            if (checkTroop == null)
                return false;
            
            return CheckCondition(checkTroop.troops);
        }

        /// <summary>
        /// 根据运算符检查条件是否满足
        /// </summary>
        /// <param name="actualValue">实际值</param>
        /// <returns>条件是否满足</returns>
        private bool CheckCondition(int actualValue)
        {
            switch (@operator)
            {
                case "eq":
                    return actualValue == value;
                case "gt":
                    return actualValue > value;
                case "lt":
                    return actualValue < value;
                case "gte":
                    return actualValue >= value;
                case "lte":
                    return actualValue <= value;
                default:
                    return false;
            }
        }
    }
}
