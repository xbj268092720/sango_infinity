using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 距离检查条件
    /// distance: 距离值
    /// operator: 比较运算符 (eq/gt/lt/gte/lte)
    /// distanceType: 距离类型 (manhattan/euclidean)
    /// </summary>
    public class DistanceCheck : Condition
    {
        /// <summary>
        /// 距离值
        /// </summary>
        int distance;
        
        /// <summary>
        /// 比较运算符 (eq: 等于, gt: 大于, lt: 小于, gte: 大于等于, lte: 小于等于)
        /// </summary>
        string @operator;
        
        /// <summary>
        /// 距离类型 (manhattan: 曼哈顿距离, euclidean: 欧几里得距离)
        /// </summary>
        string distanceType;

        /// <summary>
        /// 初始化距离检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            distance = p.Value<int>("distance");
            @operator = p.Value<string>("operator") ?? "eq";
            distanceType = p.Value<string>("distanceType") ?? "manhattan";
        }

        /// <summary>
        /// 计算两个单元格之间的距离
        /// </summary>
        /// <param name="cell1">第一个单元格</param>
        /// <param name="cell2">第二个单元格</param>
        /// <returns>计算的距离</returns>
        private int CalculateDistance(Cell cell1, Cell cell2)
        {
            if (cell1 == null || cell2 == null)
                return -1;
            
            if (distanceType == "manhattan")
            {
                return System.Math.Abs(cell1.x - cell2.x) + System.Math.Abs(cell1.y - cell2.y);
            }
            else if (distanceType == "euclidean")
            {
                int dx = cell1.x - cell2.x;
                int dy = cell1.y - cell2.y;
                return (int)System.Math.Sqrt(dx * dx + dy * dy);
            }
            
            return -1;
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
            
            if (troop == null || target == null || troop.cell == null || target.cell == null)
                return false;
            
            int actualDistance = CalculateDistance(troop.cell, target.cell);
            return CheckDistance(actualDistance);
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
            if (troop == null || target == null || troop.cell == null || target.cell == null)
                return false;
            
            int actualDistance = CalculateDistance(troop.cell, target.cell);
            return CheckDistance(actualDistance);
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
            if (troop == null || spellCell == null || troop.cell == null)
                return false;
            
            int actualDistance = CalculateDistance(troop.cell, spellCell);
            return CheckDistance(actualDistance);
        }

        /// <summary>
        /// 根据运算符检查距离是否满足条件
        /// </summary>
        /// <param name="actualDistance">实际距离</param>
        /// <returns>条件是否满足</returns>
        private bool CheckDistance(int actualDistance)
        {
            if (actualDistance < 0)
                return false;
            
            switch (@operator)
            {
                case "eq":
                    return actualDistance == distance;
                case "gt":
                    return actualDistance > distance;
                case "lt":
                    return actualDistance < distance;
                case "gte":
                    return actualDistance >= distance;
                case "lte":
                    return actualDistance <= distance;
                default:
                    return false;
            }
        }
    }
}
