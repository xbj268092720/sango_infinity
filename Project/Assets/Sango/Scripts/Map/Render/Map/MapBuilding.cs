using UnityEngine;
using Sango.Core;
using Sango.Tools;
using System.Collections.Generic;

namespace Sango.Render
{
    /// <summary>
    /// 地图标注类
    /// </summary>
    public class MapBuilding : MapObject
    {
        public override bool canSave { get; set; } = false;
        public List<int> neighbors = new List<int>();
    }
}