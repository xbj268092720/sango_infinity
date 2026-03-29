using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 天气检查条件
    /// weatherType: 天气类型
    /// checkTarget: 检查目标 (self/target)
    /// </summary>
    public class WeatherCheck : Condition
    {
        /// <summary>
        /// 天气类型
        /// </summary>
        int weatherType;
        
        /// <summary>
        /// 检查目标 (self: 自己, target: 目标)
        /// </summary>
        string checkTarget;

        /// <summary>
        /// 初始化天气检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            weatherType = p.Value<int>("weatherType");
            checkTarget = p.Value<string>("checkTarget") ?? "self";
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <param name="objects">检查条件所需的对象</param>
        /// <returns>条件是否满足</returns>
        public override bool Check(params object[] objects)
        {
            if (objects.Length < 2)
                return false;
            
            Troop troop = objects[0] as Troop;
            Troop target = objects[1] as Troop;
            
            if (checkTarget == "self" && troop != null && troop.cell != null)
            {
                return troop.cell.weatherType == weatherType;
            }
            else if (checkTarget == "target" && target != null && target.cell != null)
            {
                return target.cell.weatherType == weatherType;
            }
            
            return false;
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
            if (checkTarget == "self" && troop != null && troop.cell != null)
            {
                return troop.cell.weatherType == weatherType;
            }
            else if (checkTarget == "target" && target != null && target.cell != null)
            {
                return target.cell.weatherType == weatherType;
            }
            
            return false;
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
            if (checkTarget == "self" && troop != null && troop.cell != null)
            {
                return troop.cell.weatherType == weatherType;
            }
            else if (checkTarget == "target" && spellCell != null)
            {
                return spellCell.weatherType == weatherType;
            }
            
            return false;
        }
    }
}
