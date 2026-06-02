using TKNewtonsoft.Json;
using Sango.Mod;
using System.Xml;

namespace Sango.Core
{
    public class GameData : Singleton<GameData>
    {
        /// <summary>
        /// 通用配置
        /// </summary>
        public ScenarioCommonData ScenarioCommonData { get; private set; }

        /// <summary>
        /// 模型配置
        /// </summary>
        public SangoObjectMap<ModelConfig> ModelConfigs { get; private set; }

        public void Init()
        {
            Sango.Log.Info("GameData.Init()");
            LoadCommonData();
            LoadModelConfig();
            SkillConfigManager.Instance.Init();
        }

        public ScenarioCommonData LoadNewCommonData()
        {
            ScenarioCommonData scenarioCommonData = new ScenarioCommonData();
            scenarioCommonData.Load(Path.ContentRootPath + "/Data/Common/Common.json");
            ModManager.Instance.EnumFiles("Data/Common/Common.json", dir =>
            {
                scenarioCommonData.Load(dir);
            });
            return scenarioCommonData;
        }

        internal ScenarioCommonData LoadCommonData()
        {
            if (ScenarioCommonData != null)
                return ScenarioCommonData;

            ScenarioCommonData = new ScenarioCommonData();
            ScenarioCommonData.Load(Path.ContentRootPath + "/Data/Common/Common.json");
            ModManager.Instance.EnumFiles("Data/Common/Common.json", dir =>
            {
                ScenarioCommonData.Load(dir);
            });
            return ScenarioCommonData;
        }

        public SangoObjectMap<ModelConfig> LoadModelConfig()
        {
            if (ModelConfigs != null)
                return ModelConfigs;

            ModelConfigs = new SangoObjectMap<ModelConfig>();
            string commonDataFileName = "Data/Model/ModelConfig.json";
            ModManager.Instance.LoadFile(commonDataFileName, file =>
            {
                //XmlDocument xmlDocument = new XmlDocument();
                //xmlDocument.Load(file);
                //ModelConfigs.Load(xmlDocument.LastChild);
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new SangoObjectMaptConverter<ModelConfig>());
                ModelConfigs = TKNewtonsoft.Json.JsonConvert.DeserializeObject<SangoObjectMap<ModelConfig>>(File.ReadAllText(file), jsonSerializerSettings);

            });
            
            //SimpleJSON.JSONArray node = new SimpleJSON.JSONArray();
            //ModelConfigs.Save(node);
            //node.SaveToFile("D:/modelConfig.json");
            //File.WriteAllText("D:/modelConfig1.json", node.ToJson());

            //SimpleJSON.JSONNode loaded = SimpleJSON.JSON.Parse(File.ReadAllText("D:/modelConfig1.json"));
            //File.WriteAllText("D:/modelConfig2.json", loaded.ToJson());

            return ModelConfigs;
        }
    }
}
