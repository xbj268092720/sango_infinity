using TKNewtonsoft.Json;
using TKNewtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sango.Game
{
    public abstract class SkillEffect
    {
        public SkillInstance master;
        public virtual void Init(JObject p, SkillInstance master) { this.master = master; }
        public abstract void Action(SkillInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList);
        public virtual void Clear() {}

        public delegate SkillEffect SkillEffectCreator();

        public static Dictionary<string, SkillEffectCreator> CreateMap = new Dictionary<string, SkillEffectCreator>();
        public static void Register(string name, SkillEffectCreator action)
        {
            CreateMap[name] = action;
        }
        public static SkillEffect CraeteHandle<T>() where T : SkillEffect, new()
        {
            return new T();
        }
        public static SkillEffect Create(string name)
        {
            SkillEffectCreator creator;
            if (CreateMap.TryGetValue(name, out creator))
                return creator();
            return null;
        }

        public static void Init()
        {

            Register("SetFire", CraeteHandle<SetFire>);
            Register("AddBuff", CraeteHandle<AddBuff>);
            Register("RemoveBuffById", CraeteHandle<RemoveBuffById>);
            Register("RemoveBuffByKind", CraeteHandle<RemoveBuffByKind>);
            Register("PutoutFire", CraeteHandle<PutoutFire>);
            Register("RecureWoundedTroops", CraeteHandle<RecureWoundedTroops>);

        }





    }
}
