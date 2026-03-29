using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 城市属性检查条件
    /// attributeType: 属性类型 (commerce/agriculture/popularSupport/security/energy/morale/troops/food/gold)
    /// checkTarget: 检查目标 (self/target)
    /// value: 比较值
    /// operator: 比较运算符 (eq/gt/lt/gte/lte)
    /// </summary>
    public class CityAttributeCheck : Condition
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        string attributeType;
        
        /// <summary>
        /// 检查目标 (self: 自己, target: 目标)
        /// </summary>
        string checkTarget;
        
        /// <summary>
        /// 比较值
        /// </summary>
        int value;
        
        /// <summary>
        /// 比较运算符 (eq: 等于, gt: 大于, lt: 小于, gte: 大于等于, lte: 小于等于)
        /// </summary>
        string @operator;

        /// <summary>
        /// 初始化城市属性检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            attributeType = p.Value<string>("attributeType");
            checkTarget = p.Value<string>("checkTarget") ?? "self";
            value = p.Value<int>("value");
            @operator = p.Value<string>("operator") ?? "gte";
        }

        /// <summary>
        /// 获取城市的属性值
        /// </summary>
        /// <param name="city">城市对象</param>
        /// <returns>属性值</returns>
        private int GetAttributeValue(City city)
        {
            if (city == null)
                return 0;
            
            switch (attributeType)
            {
                case "commerce":
                    return city.commerce;
                case "agriculture":
                    return city.agriculture;
                case "popularSupport":
                    return city.popularSupport;
                case "security":
                    return city.security;
                case "energy":
                    return city.energy;
                case "morale":
                    return city.morale;
                case "troops":
                    return city.troops;
                case "food":
                    return city.food;
                case "gold":
                    return city.gold;
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
            City city = null;
            
            if (checkTarget == "self" && objects.Length > 0)
            {
                city = objects[0] as City;
            }
            else if (checkTarget == "target" && objects.Length > 1)
            {
                city = objects[1] as City;
            }
            
            if (city == null)
                return false;
            
            int actualValue = GetAttributeValue(city);
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
            City city = null;
            
            if (checkTarget == "self" && troop != null)
            {
                city = troop.BelongCity;
            }
            else if (checkTarget == "target" && target != null)
            {
                city = target.BelongCity;
            }
            
            if (city == null)
                return false;
            
            int actualValue = GetAttributeValue(city);
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
            City city = null;
            
            if (checkTarget == "self" && troop != null)
            {
                city = troop.BelongCity;
            }
            else if (checkTarget == "target" && spellCell != null)
            {
                city = spellCell.BelongCity;
            }
            
            if (city == null)
                return false;
            
            int actualValue = GetAttributeValue(city);
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
