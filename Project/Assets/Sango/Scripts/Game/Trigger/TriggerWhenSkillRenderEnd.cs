using Sango.Core.Tools;

namespace Sango.Core
{
    public class TriggerWhenSkillRenderEnd : SkillTrigger
    {
        public override void Init(TriggerCall call)
        {
            base.Init(call);
            GameEvent.OnSkillRenderEnd += OnSkillRenderEnd;
        }

        public override void Clear()
        {
            GameEvent.OnSkillRenderEnd -= OnSkillRenderEnd;
        }

        public void OnSkillRenderEnd(SkillInstance skill, Cell spellCell)
        {
            atk_cell = spellCell;
            this.skill = skill;
            triggerCall?.Invoke(this);
        }

    }
}
