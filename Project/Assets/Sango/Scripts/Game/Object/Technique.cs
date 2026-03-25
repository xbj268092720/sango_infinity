using TKNewtonsoft.Json;
using TKNewtonsoft.Json.Linq;
using Sango.Game.Action;
using Sango.Game.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.Game
{
    /// <summary>
    /// 州
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Technique : SangoObject
    {
        [JsonProperty] public string desc;
        [JsonProperty] public string kind;
        [JsonProperty] public int level;
        [JsonProperty] public int needAttr;
        [JsonProperty] public int goldCost;
        [JsonProperty] public int techPointCost;
        [JsonProperty] public int counter;
        /// <summary>
        /// 前置科技
        /// </summary>
        [JsonProperty] public int needTech;
        [JsonProperty] public JArray effects;
        [JsonProperty] public int col;
        [JsonProperty] public int row;
        [JsonConverter(typeof(Color32Converter))]
        [JsonProperty] public UnityEngine.Color32 tabColor;
        [JsonProperty] public int[] recommandFeatures;

        public bool CanResearch(Force force)
        {
            if (force == null) return false;
            if (force.HasTechnique(Id)) return false;
            if (needTech > 0)
            {
                if (!force.HasTechnique(needTech))
                    return false;
            }
            return true;
        }

        public bool IsValid(Force force)
        {
            return force.HasTechnique(Id);
        }

        public void InitActions(List<ActionBase> list, params SangoObject[] sangoObjects)
        {
            if (effects == null) return;
            for (int i = 0; i < effects.Count; i++)
            {
                JObject valus = effects[i] as JObject;
                ActionBase action = ActionBase.Create(valus.Value<string>("class"));
                if (action != null)
                {
                    action.Init(valus, sangoObjects);
                    list.Add(action);
                }
            }
        }

        public int[] GetCost(Person[] personList, City city)
        {
            if (personList == null || personList.Length == 0) return null;

            Scenario scenario = Scenario.Cur;
            ScenarioVariables variables = scenario.Variables;
            GameUtility.InitJobFeature(personList, city);
            int goldNeed = goldCost;
            int tpNeed = techPointCost;
            int turnCount = counter;

            int totalValue = 0;
            for (int i = 0; i < personList.Length; i++)
            {
                Person person = personList[i];
                if (person == null) continue;
                totalValue += person.GetAttribute(needAttr);
            }

            turnCount = GameUtility.Method_ResearchCounter(totalValue, turnCount);

            Tools.OverrideData<int> goldOverride = GameUtility.IntOverrideData.Set(goldNeed);
            Tools.OverrideData<int> tpOverride = new OverrideData<int>(tpNeed);
            Tools.OverrideData<int> turnCountOveride = new OverrideData<int>(turnCount);

            GameEvent.OnCityResearchCost?.Invoke(city, personList, this, goldOverride, tpOverride, turnCountOveride);
            goldNeed = goldOverride.Value;
            tpNeed = tpOverride.Value;
            turnCount = turnCountOveride.Value;

            city.ClearJobFeature();
            return new int[] { goldNeed, tpNeed, turnCount };
        }
    }
}
