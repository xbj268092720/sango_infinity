using TKNewtonsoft.Json.Linq;
using Sango.Core.Tools;
using System.Collections.Generic;

namespace Sango.Core
{
    public class SkillIsStrategySkill : Condition
    {
        // 比较结果: 0不是 1是
        int result;

        public override bool Check(params object[] objects)
        {
            if (objects.Length < 3) return false;
            for (int i = 0; i < objects.Length; i++)
            {
                SkillInstance skill = objects[i] as SkillInstance;
                if (skill != null)
                {
                    return (skill.IsStrategy()) == (result == 1);
                }
            }
            return false;
        }

        public override bool Check(Troop troop, Troop target, SkillInstance skill)
        {
            if (skill != null)
            {
                return (skill.IsStrategy()) == (result == 1);
            }
            return false;
        }

        public override bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
            if (skillInstance != null)
            {
                return (skillInstance.IsStrategy()) == (result == 1);
            }
            return false;
        }

        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            result = p.Value<int>("result");
        }
    }
}
