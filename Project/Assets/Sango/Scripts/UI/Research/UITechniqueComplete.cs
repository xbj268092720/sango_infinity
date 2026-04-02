using Sango.Core.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sango.Core; namespace Sango.UI
{
    /// <summary>
    /// 剧本选择界面
    /// </summary>
    public class UITechniqueComplete : UGUIWindow
    {
        public UITechniqueItem techniqueItem;
        public Animation animation;

        public override void OnShow(params object[] ps)
        {
            base.OnShow();
            techniqueItem.SetTechnique(ps[0] as Technique);
            animation.Play();
            Invoke("Hide", animation.clip.length);
        }
    }
}
