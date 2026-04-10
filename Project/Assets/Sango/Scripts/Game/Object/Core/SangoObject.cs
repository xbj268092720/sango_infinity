using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TKNewtonsoft.Json;
using TKNewtonsoft.Json.Linq;
using Sango.Render;

namespace Sango.Core
{
    public enum SangoObjectType : int
    {
        Unknown = 0,
        Scenario,
        Person,
        Force,
        Troops,
        City,
        Building,
        Corps,
        Skill,
        InnerBuilding,
        Strategy

    }

    public class SangoObject
    {
        public virtual SangoObjectType ObjectType { get { return SangoObjectType.Unknown; } }

        private int _Id = -1;
        [JsonProperty(Order = -99)]
        public int Id { get { return _Id; } set { _Id = value; } }

        [JsonProperty(Order = -98)]
        public virtual string Name { get; set; }
        public virtual string Tag { get; set; }

        [JsonProperty]
        public virtual bool IsAlive { get; set; }

        [JsonProperty]
        public virtual bool ActionOver { get; set; }
        public virtual ObjectRender GetRender() { return null; }

        //[JsonExtensionData]
        //public Dictionary<string, JToken> AdditionalData { get; set; }
        [JsonExtensionData]
        private Dictionary<string, JToken> ExtensionData;

        public T GetExtensionData<T>(string key) where T : struct
        {
            if (ExtensionData == null) return default(T);

            if (ExtensionData.TryGetValue(key, out var value))
            {
                return value.Value<T>();
            }
            return default(T);
        }

        public void SetExtensionData<T>(string key, T value) where T : struct
        {
            if (ExtensionData == null)
                ExtensionData = new Dictionary<string, JToken>();
            ExtensionData[key] = new JValue(value);
        }

        public JToken GetExtensionData(string key)
        {
            if (ExtensionData == null) return null;
            if (ExtensionData.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        public void SetExtensionData<T>(string key, JToken value)
        {
            if (ExtensionData == null)
                ExtensionData = new Dictionary<string, JToken>();
            ExtensionData[key] = value;
        }
        public bool HasExtensionData(string key)
        {
            if (ExtensionData == null) return false;
            return ExtensionData.ContainsKey(key);
        }



        public SangoObject()
        {
            IsAlive = true;
        }
        public override string ToString()
        {
            return Name;
        }

        public virtual bool DoAI(Scenario scenario) { return true; }
        public virtual bool Run(Scenario scenario) { return true; }
        public virtual void OnScenarioPrepare(Scenario scenario) {; }
        public virtual void OnScenarioStart(Scenario scenario) {; }

        /// <summary>
        /// 在势力开始时候调用
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public virtual bool OnForceTurnStart(Scenario scenario) { return true; }

        /// <summary>
        /// 在势力结束时候调用
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public virtual bool OnForceTurnEnd(Scenario scenario) { return true; }

        /// <summary>
        /// 在回合开始时候调用
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public virtual bool OnTurnStart(Scenario scenario) { return true; }

        /// <summary>
        /// 在回合结束时候调用
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        public virtual bool OnTurnEnd(Scenario scenario) { return true; }


        public virtual bool OnDayStart(Scenario scenario) { return true; }
        public virtual bool OnDayEnd(Scenario scenario) { return true; }
        public virtual bool OnMonthStart(Scenario scenario) { return true; }
        public virtual bool OnMonthEnd(Scenario scenario) { return true; }
        public virtual bool OnYearStart(Scenario scenario) { return true; }
        public virtual bool OnYearEnd(Scenario scenario) { return true; }
        public virtual bool OnSeasonStart(Scenario scenario) { return true; }
        public virtual bool OnSeasonEnd(Scenario scenario) { return true; }
        public virtual void Init(Scenario scenario) { }
        public virtual void Clear() { }

        public bool IsEnemy(Force forceA, Force forceB)
        {
            if (forceA == null) return true;
            if (forceB == null) return true;
            return forceA != forceB && !forceA.IsAlliance(forceB);
        }

        public bool IsSameForce(Force forceA, Force forceB)
        {
            if (forceA == null) return false;
            if (forceB == null) return false;
            return forceA == forceB;
        }

        public bool IsAlliance(Force forceA, Force forceB)
        {
            if (forceA == null) return false;
            if (forceB == null) return false;
            return forceA.IsAlliance(forceB);
        }
        public static int Compare(SangoObject a, SangoObject b)
        {
            if (a != null && b != null)
            {
                return a.Id.CompareTo(b.Id);
            }

            if (a != null)
                return 1;

            if (b != null)
                return -1;

            return 0;
        }

    }


    public class SangoObjectExtensionData : SangoObject
    {

    }
}
