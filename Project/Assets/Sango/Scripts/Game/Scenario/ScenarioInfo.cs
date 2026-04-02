using System.IO;
using TKNewtonsoft.Json;
using UnityEngine;

namespace Sango.Core
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ScenarioInfo
    {
        [JsonProperty]
        public int id;
        [JsonProperty]
        public string name;
        [JsonProperty]
        public string tag;
        [JsonProperty]
        public string description;
        [JsonProperty]
        public int year;
        [JsonProperty]
        public int month;
        [JsonProperty]
        public int day;
        [JsonProperty]
        public int curForceId;
        [JsonProperty]
        public string mapType;
        [JsonProperty]
        public int turnCount;
        [JsonProperty]
        public int priority;
        [JsonProperty]
        public bool isSave;
        [JsonProperty]
        public int[] playerForceList;
        [JsonProperty]
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 cameraPosition;
        [JsonProperty]
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 cameraRotation;
        [JsonProperty]
        public float cameraDistance;
        [JsonProperty]
        public long dateTime;
    }
}
