using Sango.Game.Action;
using TKNewtonsoft.Json;
using Sango.Game.Render;
using System;
using System.Collections.Generic;

namespace Sango.Game
{
    public class Equipment : ItemType
    {
        /// <summary>
        /// 品级
        /// </summary>
        [JsonProperty] public int grade;

        /// <summary>
        /// 发现概率
        /// </summary>
        [JsonProperty] public int discoverProbability;

        /// <summary>
        /// 统率加成
        /// </summary>
        [JsonProperty] public int commandBonus;

        /// <summary>
        /// 武力加成
        /// </summary>
        [JsonProperty] public int strengthBonus;

        /// <summary>
        /// 智力加成
        /// </summary>
        [JsonProperty] public int intelligenceBonus;

        /// <summary>
        /// 政治加成
        /// </summary>
        [JsonProperty] public int politicsBonus;

        /// <summary>
        /// 魅力加成
        /// </summary>
        [JsonProperty] public int glamourBonus;

        /// <summary>
        /// 附加特技
        /// </summary>
        [JsonProperty]
        [JsonConverter(typeof(SangoObjectListIDConverter<Feature>))]
        public SangoObjectList<Feature> features = new SangoObjectList<Feature>();
        
        /// <summary>
        /// 附加效果
        /// </summary>
        public ActionBase action;

    }
}
