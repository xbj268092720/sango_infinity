using UnityEngine;

namespace Sango.Game.Player
{

    public class ScaleWithScreenHeigh : MonoBehaviour
    {
        public bool scaleable = false;
        private void Start()
        {
            if(scaleable)
            {
                float s = Game.Instance.CanvasScalerFactor;
                if (s < 1)
                    transform.localScale = new Vector3(s, s, s);
            }
        }
    }
}
