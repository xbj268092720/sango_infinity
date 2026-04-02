using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 势力检查条件
    /// forceId: 势力ID
    /// checkType: 检查类型 (self/ally/enemy/neutral)
    /// </summary>
    public class FactionCheck : Condition
    {
        /// <summary>
        /// 势力ID
        /// </summary>
        int forceId;
        
        /// <summary>
        /// 检查类型 (self: 自己, ally: 盟友, enemy: 敌人, neutral: 中立)
        /// </summary>
        string checkType;

        /// <summary>
        /// 初始化势力检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            forceId = p.Value<int>("forceId");
            checkType = p.Value<string>("checkType") ?? "self";
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
            
            if (checkType == "self" && troop != null)
            {
                return troop.BelongForce?.Id == forceId;
            }
            else if (target != null)
            {
                switch (checkType)
                {
                    case "ally":
                        return IsAlly(troop, target);
                    case "enemy":
                        return IsEnemy(troop, target);
                    case "neutral":
                        return IsNeutral(troop, target);
                    case "target":
                        return target.BelongForce?.Id == forceId;
                    default:
                        return false;
                }
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
            if (checkType == "self" && troop != null)
            {
                return troop.BelongForce?.Id == forceId;
            }
            else if (target != null)
            {
                switch (checkType)
                {
                    case "ally":
                        return IsAlly(troop, target);
                    case "enemy":
                        return IsEnemy(troop, target);
                    case "neutral":
                        return IsNeutral(troop, target);
                    case "target":
                        return target.BelongForce?.Id == forceId;
                    default:
                        return false;
                }
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
            if (checkType == "self" && troop != null)
            {
                return troop.BelongForce?.Id == forceId;
            }
            else if (spellCell != null && spellCell.troop != null)
            {
                switch (checkType)
                {
                    case "ally":
                        return IsAlly(troop, spellCell.troop);
                    case "enemy":
                        return IsEnemy(troop, spellCell.troop);
                    case "neutral":
                        return IsNeutral(troop, spellCell.troop);
                    case "target":
                        return spellCell.troop.BelongForce?.Id == forceId;
                    default:
                        return false;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 检查两个部队是否为盟友
        /// </summary>
        /// <param name="troop1">第一个部队</param>
        /// <param name="troop2">第二个部队</param>
        /// <returns>是否为盟友</returns>
        private bool IsAlly(Troop troop1, Troop troop2)
        {
            if (troop1 == null || troop2 == null)
                return false;
            
            // 简化实现：同一势力视为盟友
            return troop1.BelongForce?.Id == troop2.BelongForce?.Id;
        }

        /// <summary>
        /// 检查两个部队是否为敌人
        /// </summary>
        /// <param name="troop1">第一个部队</param>
        /// <param name="troop2">第二个部队</param>
        /// <returns>是否为敌人</returns>
        private bool IsEnemy(Troop troop1, Troop troop2)
        {
            if (troop1 == null || troop2 == null)
                return false;
            
            // 简化实现：不同势力视为敌人
            return troop1.BelongForce?.Id != troop2.BelongForce?.Id;
        }

        /// <summary>
        /// 检查两个部队是否为中立
        /// </summary>
        /// <param name="troop1">第一个部队</param>
        /// <param name="troop2">第二个部队</param>
        /// <returns>是否为中立</returns>
        private bool IsNeutral(Troop troop1, Troop troop2)
        {
            // 可以根据游戏规则实现中立关系的检查
            // 这里简化实现为始终返回false
            return false;
        }
    }
}
