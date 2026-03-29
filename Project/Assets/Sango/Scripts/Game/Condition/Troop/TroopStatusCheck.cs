using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 部队状态检查条件
    /// statusType: 状态类型
    /// checkTarget: 检查目标 (self/target)
    /// hasStatus: 是否拥有该状态 (true/false)
    /// </summary>
    public class TroopStatusCheck : Condition
    {
        /// <summary>
        /// 状态类型
        /// </summary>
        int statusType;
        
        /// <summary>
        /// 检查目标 (self: 自己, target: 目标)
        /// </summary>
        string checkTarget;
        
        /// <summary>
        /// 是否拥有该状态
        /// </summary>
        bool hasStatus;

        /// <summary>
        /// 初始化部队状态检查条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            statusType = p.Value<int>("statusType");
            checkTarget = p.Value<string>("checkTarget") ?? "self";
            hasStatus = p.Value<bool>("hasStatus");
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
            
            if (checkTarget == "self" && troop != null)
            {
                bool has = HasBuffByKind(troop, statusType);
                return has == hasStatus;
            }
            else if (checkTarget == "target" && target != null)
            {
                bool has = HasBuffByKind(target, statusType);
                return has == hasStatus;
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
            if (checkTarget == "self" && troop != null)
            {
                bool has = HasBuffByKind(troop, statusType);
                return has == hasStatus;
            }
            else if (checkTarget == "target" && target != null)
            {
                bool has = HasBuffByKind(target, statusType);
                return has == hasStatus;
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
            if (checkTarget == "self" && troop != null)
            {
                bool has = HasBuffByKind(troop, statusType);
                return has == hasStatus;
            }
            else if (checkTarget == "target" && spellCell != null && spellCell.troop != null)
            {
                bool has = HasBuffByKind(spellCell.troop, statusType);
                return has == hasStatus;
            }
            
            return false;
        }

        /// <summary>
        /// 检查部队是否拥有指定类型的buff
        /// </summary>
        /// <param name="troop">部队对象</param>
        /// <param name="kind">buff类型</param>
        /// <returns>是否拥有该类型的buff</returns>
        private bool HasBuffByKind(Troop troop, int kind)
        {
            if (troop == null || troop.buffManager == null)
                return false;
            
            foreach (var buff in troop.buffManager._buffs)
            {
                if (buff.Buff != null && buff.Buff.kind == kind)
                    return true;
            }
            return false;
        }
    }
}
