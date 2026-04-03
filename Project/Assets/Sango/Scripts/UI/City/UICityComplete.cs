using Sango.Core.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Sango.Core;
using Sango.Manager;

namespace Sango.UI
{
    /// <summary>
    /// 攻陷城池
    /// </summary>
    public class UICityComplete : UGUIWindow
    {
        public Text cityName;
        public Animation animation;

        public override void OnShow(params object[] ps)
        {
            base.OnShow();
            cityName.text = (ps[0] as string);
            animation.Play();
            AudioManager.Instance.PlaySfx("Assets/Sound/2265.ogg");
            Invoke("Hide", animation.clip.length);
        }
    }
}
