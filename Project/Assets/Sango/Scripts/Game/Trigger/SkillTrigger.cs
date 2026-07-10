using Sango.Core.Tools;
using System.Collections.Generic;

namespace Sango.Core
{
    public abstract class SkillTrigger : Trigger
    {
        protected Cell atk_cell;
        protected SkillInstance skill;
        protected OverrideData<int> damageOverride;
        public override SkillInstance ActionSkill => skill;
        public override SkillInstance TargetSkill => null;
        public override Person ActionPerson => skill.master.Leader;
        public override Person TargetPerson => atk_cell.troop?.Leader;
        public override Troop ActionTroop => skill.master;
        public override Troop TargetTroop => atk_cell.troop;
        public override Cell ActionCell => skill.master.cell;
        public override Cell TargetCell => atk_cell;
        public override City ActionCity => skill.master.BelongCity;
        public override City TargetCity => atk_cell.troop?.BelongCity ?? atk_cell.building?.BelongCity;
        public override Corps ActionCorps => skill.master.BelongCorps;
        public override Corps TargetCorps => atk_cell.troop?.BelongCorps ?? atk_cell.building?.BelongCorps;
        public override Force ActionForce => skill.master.BelongForce;
        public override Force TargetForce => atk_cell.troop?.BelongForce ?? atk_cell.building?.BelongForce;

        public override Fire ActiveFire => skill.master.cell.fire;
        public override Fire TargetFire => atk_cell.fire;
        public override object ActionObject => skill;
        public override object TargetObject => atk_cell;
    }
}
