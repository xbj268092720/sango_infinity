using System.Collections.Generic;
using System.IO;
using TKNewtonsoft.Json;
using Sango.Mod;

namespace Sango.Game
{
    public class SkillConfigManager : Singleton<SkillConfigManager>
    {
        private SangoObjectMap<Skill> _skillConfigs = new SangoObjectMap<Skill>();
        private SangoObjectMap<Buff> _buffConfigs = new SangoObjectMap<Buff>();

        public void Init()
        {
            Sango.Log.Print("SkillConfigManager.Init()");
            LoadSkillConfigs();
            LoadBuffConfigs();
        }

        public SangoObjectMap<Skill> LoadSkillConfigs()
        {
            if (_skillConfigs != null && _skillConfigs.Count > 0)
                return _skillConfigs;

            _skillConfigs = new SangoObjectMap<Skill>();
            string skillConfigFileName = "Data/Skill/Skills.json";
            ModManager.Instance.LoadFile(skillConfigFileName, file =>
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new SangoObjectMaptConverter<Skill>());
                SangoObjectMap<Skill> skills = JsonConvert.DeserializeObject<SangoObjectMap<Skill>>(File.ReadAllText(file), jsonSerializerSettings);
                if (skills != null)
                {
                    skills.ForEach(skill =>
                {
                    _skillConfigs.Add(skill);
                });
                }
            });

            return _skillConfigs;
        }

        public SangoObjectMap<Buff> LoadBuffConfigs()
        {
            if (_buffConfigs != null && _buffConfigs.Count > 0)
                return _buffConfigs;

            _buffConfigs = new SangoObjectMap<Buff>();
            string buffConfigFileName = "Data/Skill/Buffs.json";
            ModManager.Instance.LoadFile(buffConfigFileName, file =>
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new SangoObjectMaptConverter<Buff>());
                SangoObjectMap<Buff> buffs = JsonConvert.DeserializeObject<SangoObjectMap<Buff>>(File.ReadAllText(file), jsonSerializerSettings);
                if (buffs != null)
                {
                    buffs.ForEach(buff =>
                {
                    _buffConfigs.Add(buff);
                });
                }
            });

            return _buffConfigs;
        }

        public Skill GetSkill(int id)
        {
            return _skillConfigs.Get(id);
        }

        public Buff GetBuff(int id)
        {
            return _buffConfigs.Get(id);
        }

        public void AddSkill(Skill skill)
        {
            if (skill != null)
            {
                _skillConfigs.Add(skill);
            }
        }

        public void AddBuff(Buff buff)
        {
            if (buff != null)
            {
                _buffConfigs.Add(buff);
            }
        }

        public void LoadModSkillConfigs(Mod.Mod mod)
        {
            string skillConfigPath = mod.GetFullPath("Data/Skill/Skills.json");
            if (File.Exists(skillConfigPath))
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new SangoObjectMaptConverter<Skill>());
                SangoObjectMap<Skill> modSkills = JsonConvert.DeserializeObject<SangoObjectMap<Skill>>(File.ReadAllText(skillConfigPath), jsonSerializerSettings);
                if (modSkills != null)
                {
                    modSkills.ForEach(skill =>
                    {
                        _skillConfigs.Add(skill);
                    });
                }
            }

            string buffConfigPath = mod.GetFullPath("Data/Skill/Buffs.json");
            if (File.Exists(buffConfigPath))
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new SangoObjectMaptConverter<Buff>());
                SangoObjectMap<Buff> modBuffs = JsonConvert.DeserializeObject<SangoObjectMap<Buff>>(File.ReadAllText(buffConfigPath), jsonSerializerSettings);
                if (modBuffs != null)
                {
                    modBuffs.ForEach(buff =>
                    {
                        _buffConfigs.Add(buff);
                    });
                }
            }
        }
    }
}
