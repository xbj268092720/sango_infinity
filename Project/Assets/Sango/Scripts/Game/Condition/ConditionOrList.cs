using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Core
{
    /// <summary>
    /// 或集,任何一个满足则返回true
    /// </summary>
    public class ConditionOrList : Condition
    {
        List<Condition> list = new List<Condition>();

        public override bool Check(params object[] objects)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Condition c = list[i];
                if (c != null && c.Check(objects))
                    return true;
            }
            return false;
        }

        public override bool Check(Troop troop, Troop target, SkillInstance skill)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Condition c = list[i];
                if (c != null && c.Check(troop, target, skill))
                    return true;
            }
            return false;
        }

        public override bool Check(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Condition c = list[i];
                if (c != null && c.Check(skillInstance, troop, spellCell, atkCellList))
                    return true;
            }
            return false;
        }

        public override void Init(JObject p, params SangoObject[] sangoObjects)
        {
            JArray array = p.Value<JArray>("list");
            for (int i = 0; i < array.Count; i++)
            {
                JObject obj = array[i].Value<JObject>();
                Condition c = Condition.Create(obj.Value<string>("class"));
                if (c != null)
                {
                    c.Init(obj, sangoObjects);
                    list.Add(c);
                }
            }
        }
    }
}
