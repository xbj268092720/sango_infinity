using TKNewtonsoft.Json.Linq;
using Sango.Core.Tools;
using System.Collections.Generic;

namespace Sango.Core.Action
{
    /// <summary>
    /// 提升范围内部队的粮食消耗
    /// value:提升值(百分比) bound: 生效范围 targetType: 作用目标范围, 0己方 1敌人 2所有
    /// </summary>
    public class BuildingImproveTroopFoodCost : BuildingActionBase
    {
        /// <summary>
        /// 提升值(百分比)
        /// </summary>
        int value;

        /// <summary>
        /// 生效范围
        /// </summary>
        int bound;

        /// <summary>
        /// 作用目标范围, 0己方 1敌人 2所有
        /// </summary>
        int tartgetType;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="p">JSON参数对象</param>
        /// <param name="sangoObjects">相关的游戏对象</param>
        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            base.Init(p, sangoObjects);
            if (Building == null)
                return;

            GameEvent.OnTroopEnterCell += OnTroopEnterCell;

            value = p.Value<int>("value");
            bound = p.Value<int>("bound");
            tartgetType = p.Value<int>("tartgetType");
        }

        /// <summary>
        /// 清理建筑资金产量提升动作
        /// </summary>
        public override void Clear()
        {
            GameEvent.OnTroopEnterCell -= OnTroopEnterCell;
        }

        void OnTroopEnterCell(Troop troop, Cell dest, Cell last)
        {
            if (tartgetType == 0 && !troop.IsSameForce(Building))
                return;
            if (tartgetType == 1 && !troop.IsEnemy(Building))
                return;

            if (last != null && last.Distance(Building.CenterCell) <= bound)
            {
                troop.foodCostFactor -= (value / 100);
            }
            else if (dest != null && dest.Distance(Building.CenterCell) <= bound)
            {
                troop.foodCostFactor += (value / 100);
            }
        }
    }
}
