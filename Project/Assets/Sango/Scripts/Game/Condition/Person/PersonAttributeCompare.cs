using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 武将属性比较条件
    /// attributeType: 属性类型 (command/strength/intelligence/politics/glamour)
    /// compareTarget: 比较目标 (self/target)
    /// value: 比较值
    /// operator: 比较运算符 (eq/gt/lt/gte/lte)
    /// </summary>
    public class PersonAttributeCompare : Condition
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        string attributeType;
        
        /// <summary>
        /// 比较目标 (self: 自己, target: 目标)
        /// </summary>
        string compareTarget;
        
        /// <summary>
        /// 比较值
        /// </summary>
        int value;
        
        /// <summary>
        /// 比较运算符 (eq: 等于, gt: 大于, lt: 小于, gte: 大于等于, lte: 小于等于)
        /// </summary>
        string @operator;

        /// <summary>
        /// 初始化武将属性比较条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            attributeType = p.Value<string>("attributeType");
            compareTarget = p.Value<string>("compareTarget") ?? "self";
            value = p.Value<int>("value");
            @operator = p.Value<string>("operator") ?? "gte";
        }

        /// <summary>
        /// 获取武将的属性值
        /// </summary>
        /// <param name="person">武将对象</param>
        /// <returns>属性值</returns>
        private int GetAttributeValue(Person person)
        {
            if (person == null)
                return 0;
            
            switch (attributeType)
            {
                case "command":
                    return person.Command;
                case "strength":
                    return person.Strength;
                case "intelligence":
                    return person.Intelligence;
                case "politics":
                    return person.Politics;
                case "glamour":
                    return person.Glamour;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <param name="objects">检查条件所需的对象</param>
        /// <returns>条件是否满足</returns>
        public override bool Check(params object[] objects)
        {
            Person person = null;
            
            if (compareTarget == "self" && objects.Length > 0)
            {
                person = objects[0] as Person;
            }
            else if (compareTarget == "target" && objects.Length > 1)
            {
                person = objects[1] as Person;
            }
            
            if (person == null)
                return false;
            
            int actualValue = GetAttributeValue(person);
            return CheckCondition(actualValue);
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
            Person person = null;
            
            if (compareTarget == "self" && troop != null)
            {
                person = troop.Leader;
            }
            else if (compareTarget == "target" && target != null)
            {
                person = target.Leader;
            }
            
            if (person == null)
                return false;
            
            int actualValue = GetAttributeValue(person);
            return CheckCondition(actualValue);
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
            Person person = null;
            
            if (compareTarget == "self" && troop != null)
            {
                person = troop.Leader;
            }
            else if (compareTarget == "target" && spellCell != null && spellCell.troop != null)
            {
                person = spellCell.troop.Leader;
            }
            
            if (person == null)
                return false;
            
            int actualValue = GetAttributeValue(person);
            return CheckCondition(actualValue);
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
