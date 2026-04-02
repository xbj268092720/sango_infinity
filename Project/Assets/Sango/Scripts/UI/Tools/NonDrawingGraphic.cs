using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI
{
    /// <summary>
    /// ui点击屏蔽组件,ugui专用
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class NonDrawingGraphic : Graphic
    {

        public override void SetMaterialDirty()
        {
            return;
        }

        public override void SetVerticesDirty()
        {
            return;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
