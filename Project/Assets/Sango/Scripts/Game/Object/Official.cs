using System.Collections;
using System.IO;
using TKNewtonsoft.Json;

namespace Sango.Core
{
    /// <summary>
    /// 特性
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class Official : SangoObject
    {
        public int troopsLimit;
        public int cost;
        public int level;
        public int meritNeeds;
    }
}
