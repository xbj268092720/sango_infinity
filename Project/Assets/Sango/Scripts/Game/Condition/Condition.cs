using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 条件基类，所有游戏中的条件判断都继承自此类
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// 初始化条件
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public abstract void Init(JObject p, params SangoObject[] sangoObjects);

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <param name="objects">检查条件所需的对象</param>
        /// <returns>条件是否满足</returns>
        public abstract bool Check(params object[] objects);

        /// <summary>
        /// 检查部队、目标和技能相关的条件
        /// </summary>
        /// <param name="troop">部队对象</param>
        /// <param name="target">目标部队</param>
        /// <param name="skill">技能实例</param>
        /// <returns>条件是否满足</returns>
        public virtual bool Check(Troop troop, Troop target, SkillInstance skill) { return false; }

        /// <summary>
        /// 检查技能实例、部队、法术单元格和攻击单元格列表相关的条件
        /// </summary>
        /// <param name="skillInstance">技能实例</param>
        /// <param name="troop">部队对象</param>
        /// <param name="spellCell">法术单元格</param>
        /// <param name="atkCellList">攻击单元格列表</param>
        /// <returns>条件是否满足</returns>
        public virtual bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList) { return false; }

        /// <summary>
        /// 条件创建委托
        /// </summary>
        public delegate Condition ConditionCreator();

        /// <summary>
        /// 条件创建映射表
        /// </summary>
        public static Dictionary<string, ConditionCreator> CreateMap = new Dictionary<string, ConditionCreator>();

        /// <summary>
        /// 注册条件
        /// </summary>
        /// <param name="name">条件名称</param>
        /// <param name="action">条件创建方法</param>
        public static void Register(string name, ConditionCreator action)
        {
            CreateMap[name] = action;
        }

        /// <summary>
        /// 创建条件实例的通用方法
        /// </summary>
        /// <typeparam name="T">条件类型</typeparam>
        /// <returns>条件实例</returns>
        public static Condition CraeteHandle<T>() where T : Condition, new()
        {
            return new T();
        }

        /// <summary>
        /// 根据名称创建条件实例
        /// </summary>
        /// <param name="name">条件名称</param>
        /// <returns>条件实例</returns>
        public static Condition Create(string name)
        {
            ConditionCreator actionBaseCreator;
            if (CreateMap.TryGetValue(name, out actionBaseCreator))
                return actionBaseCreator();
            return null;
        }

        /// <summary>
        /// 初始化所有条件
        /// </summary>
        public static void Init()
        {
            // core
            Register("and", CraeteHandle<ConditionAnd>);
            Register("or", CraeteHandle<ConditionOr>);
            Register("andList", CraeteHandle<ConditionAndList>);
            Register("orList", CraeteHandle<ConditionOrList>);
            Register("DistanceCheck", CraeteHandle<DistanceCheck>);

            // Troop
            Register("TroopAttributeCompare", CraeteHandle<TroopAttributeCompare>);
            Register("TroopStatusCheck", CraeteHandle<TroopStatusCheck>);
            Register("TroopStrengthCheck", CraeteHandle<TroopStrengthCheck>);
            Register("TroopMoraleCheck", CraeteHandle<TroopMoraleCheck>);

            // Skill
            Register("SkillIsCritical", CraeteHandle<SkillIsCritical>);
            Register("SkillIsNormalSkill", CraeteHandle<SkillIsNormalSkill>);
            Register("SkillIsStrategySkill", CraeteHandle<SkillIsStrategySkill>);

            // Person
            Register("PersonAttributeCompare", CraeteHandle<PersonAttributeCompare>);
            Register("PersonLoyaltyCheck", CraeteHandle<PersonLoyaltyCheck>);
            Register("PersonLevelCheck", CraeteHandle<PersonLevelCheck>);

            // Terrain
            Register("TerrainCheck", CraeteHandle<TerrainCheck>);

            // Weather
            Register("WeatherCheck", CraeteHandle<WeatherCheck>);

            // Faction
            Register("FactionCheck", CraeteHandle<FactionCheck>);

            // Resource
            Register("ResourceCheck", CraeteHandle<ResourceCheck>);

            // City
            Register("CityAttributeCheck", CraeteHandle<CityAttributeCheck>);
            Register("CityPopulationCheck", CraeteHandle<CityPopulationCheck>);

            // Time
            Register("TimeCheck", CraeteHandle<TimeCheck>);
        }
    }
}
