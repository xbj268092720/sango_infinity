using System.Collections;
using System.Collections.Generic;
using System.IO;
using TKNewtonsoft.Json;

namespace Sango.Core
{
    /// <summary>
    /// 特性
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Official : SangoObject
    {
        [JsonProperty]
        public int troopsLimit;

        [JsonProperty]
        public int cost;

        [JsonProperty]
        public int level;

        [JsonProperty]
        public int meritNeeds;

        [JsonProperty]
        public string effect_desc;

        [JsonProperty]
        public int commandNeed;

        [JsonProperty]
        public int strengthNeed;

        [JsonProperty]
        public int intelligenceNeed;

        [JsonProperty]
        public int politics_need;

        [JsonProperty]
        public int glamour_need;

        [JsonProperty]
        public int level_need;
        /// <summary>
        /// 下一级官职
        /// </summary>
        Official[] _next_lvevl_officials;

        public Official[] NextOfficials
        {
            get
            {
                if (meritNeeds > 0 && _next_lvevl_officials == null)
                {
                    // 第一次访问, 缓存
                    List<Official> lvevl_officials = new List<Official>();
                    Scenario.Cur.CommonData.Officials.ForEach(o =>
                    {
                        if (o.level == level - 1)
                        {
                            lvevl_officials.Add(o);
                        }
                    });
                    _next_lvevl_officials = lvevl_officials.ToArray();
                }
                return _next_lvevl_officials;
            }
        }

        public bool CheckPerson(Person person)
        {
            if(person.Command < commandNeed) return false;
            if(person.Strength < strengthNeed) return false;
            if(person.Intelligence < intelligenceNeed) return false;
            if(person.Politics < politics_need) return false;
            if(person.Glamour < glamour_need) return false;
            if(person.Level.Id < level_need) return false;
            return true ;
        }
    }
}
