using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 条件基类，所有游戏中的条件判断都继承自此类
    /// </summary>
    public interface IConditionDatabase
    {
        SkillInstance ActiveSkill { get; }
        SkillInstance TargetSkill { get; }
        Person ActivePerson { get; }
        Person TargetPerson { get; }
        Troop ActiveTroop { get; }
        Troop TargetTroop { get; }
        Cell ActiveCell { get; }
        Cell TargetCell { get; }
        City ActiveCity { get; }
        City TargetCity { get; }
        Corps ActiveCorps { get; }
        Corps TargetCorps { get; }
        Force ActiveForce { get; }
        Force TargetForce { get; }
        object ActiveObject { get; }
        object TargetObject { get; }
    }

}
