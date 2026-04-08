using TKNewtonsoft.Json;
namespace Sango.Core
{
    /// <summary>
    /// 州
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Province : SangoObject
    {

        [JsonProperty]
        public string desc;

        [JsonConverter(typeof(Id2ObjConverter<Region>))]
        [JsonProperty]
        public Region Region { get; set; }

        [JsonConverter(typeof(SangoObjectListIDConverter<Province>))]
        [JsonProperty]
        public SangoObjectList<Province> neighbors = new SangoObjectList<Province>();

    }
}
