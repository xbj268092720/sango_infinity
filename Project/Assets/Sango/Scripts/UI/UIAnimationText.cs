
using Sango.Loader;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIAnimationText : UGUIWindow
    {
        SangoObject target;
        public AnimationText aniText;
        public AnimationText.OnAnimationComplate onAnimationComplate;

        private void Awake()
        {
            aniText.aniByQueue = true;
            aniText.onAnimationComplate = OnAnimationComplate;
        }

        void OnAnimationComplate()
        {
            PoolManager.Recycle(gameObject);
            onAnimationComplate?.Invoke();
        }

        public void Show(int damage, int damageType = 0)
        {
            UITools.ShowInfo(aniText, damage, damageType);
        }

        private void Update()
        {
            if (target.IsAlive)
            {
                ObjectRender targetRender = target.GetRender();
                if (targetRender != null)
                {
                    transform.position = targetRender.GetPosition();
                }
            }
        }

        public static UIAnimationText Show(UIAnimationText src, SangoObject target, int value, int valueType)
        {
            if (src == null || !src.gameObject.activeInHierarchy)
            {
                GameObject aniTextInfo = PoolManager.Create(GameRenderHelper.AnimationTextInfoRes);
                if (aniTextInfo == null) return null;
                aniTextInfo.transform.SetParent(null);
                src = aniTextInfo.GetComponent<UIAnimationText>();
            }
            src.target = target;
            src.Show(value, valueType);
            return src;
        }
    }
}
