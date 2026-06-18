using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core.Action
{
    public abstract class TroopActionBase : ActionBase
    {
        public struct TroopActionConditionDatabase : IConditionDatabase
        {
            Cell atk_cell;
            SkillInstance self;
            public TroopActionConditionDatabase(SkillInstance self, Cell atk_cell)
            {
                this.atk_cell = atk_cell;
                this.self = self;
            }

            public SkillInstance ActionSkill => self;
            public SkillInstance TargetSkill => null;
            public Person ActionPerson => self.master.Leader;
            public Person TargetPerson => atk_cell.troop?.Leader;
            public Troop ActionTroop => self.master;
            public Troop TargetTroop => atk_cell.troop;
            public Cell ActionCell => self.master.cell;
            public Cell TargetCell => atk_cell;
            public City ActionCity => self.master.BelongCity;
            public City TargetCity => atk_cell.troop?.BelongCity ?? atk_cell.building?.BelongCity;
            public Corps ActionCorps => self.master.BelongCorps;
            public Corps TargetCorps => atk_cell.troop?.BelongCorps ?? atk_cell.building?.BelongCorps;
            public Force ActionForce => self.master.BelongForce;
            public Force TargetForce => atk_cell.troop?.BelongForce ?? atk_cell.building?.BelongForce;
            public BuildingBase ActiveBuildingBase => self.master.cell.building;
            public BuildingBase TargetBuildingBase => atk_cell.building;
            public Fire ActiveFire => self.master.cell.fire;
            public Fire TargetFire => atk_cell.fire;
            public object ActionObject => self;
            public object TargetObject => atk_cell;
        }



        protected Force Force { get; set; }
        protected Troop Troop { get; set; }
        protected JObject Params { get; set; }

        protected int value;
        protected List<int> kinds;

        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            Troop = sangoObjects[0] as Troop;
            if(Troop == null)
                Force = sangoObjects[0] as Force;
            Params = p;
            value = p.Value<int>("value");
            JArray kindsArray = p.Value<JArray>("kinds");
            if (kindsArray != null)
            {
                kinds = new List<int>(kindsArray.Count);
                for (int i = 0; i < kindsArray.Count; i++)
                    kinds.Add(kindsArray[i].ToObject<int>());
            }
        }

        /// <summary>
        /// 兵种类型(0全兵种全地形 -1陆地 -2水上)
        /// </summary>
        /// <param name="troop"></param>
        /// <param name="checkTroopTypeKind"></param>
        /// <returns></returns>
        public bool CheckTroopTypeKind(Troop troop, int checkTroopTypeKind)
        {
            if (checkTroopTypeKind == -1 && troop.IsInWater)
                return false;
            if (checkTroopTypeKind == -2 && !troop.IsInWater)
                return false;

            if (checkTroopTypeKind > 0 && ((troop.LandTroopType.kind == checkTroopTypeKind && troop.IsInWater) || (troop.WaterTroopType.kind == checkTroopTypeKind && !troop.IsInWater)))
                return false;
            return true;
        }

        /// <summary>
        /// 是否一般攻击 1一般攻击 2非一般攻击 0都可以
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="isNormalSkill"></param>
        /// <returns></returns>
        public bool CheckIsNormalSkill(SkillInstance skill, int isNormalSkill)
        {
            if ((isNormalSkill == 1 && skill.costEnergy > 0) || (isNormalSkill == 2 && skill.costEnergy == 0))
                return false;
            return true;
        }

        /// <summary>
        /// 是否是远程 1远程 2近战 0都可以 
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="isRangeSkill"></param>
        /// <returns></returns>
        public bool CheckIsRangeSkill(SkillInstance skill, int isRangeSkill)
        {
            if ((isRangeSkill == 1 && !skill.IsRange()) || (isRangeSkill == 2 && skill.IsRange()))
                return false;
            return true;
        }

    }
}
