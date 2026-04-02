using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Player
{

    public class UIImageAnimation : MonoBehaviour
    {
        public UnityEngine.Sprite[] sprites;
        public Image image;
        public float speed;
        private int index = 0;
        private void OnEnable()
        {
            if (image == null)
                image = GetComponent<Image>();
            InvokeRepeating("UpdateRender", speed, speed);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void UpdateRender()
        {
            index++;
            if (index >= sprites.Length)
                index = 0;
            if (image != null)
            {
                UnityEngine.Sprite spr = sprites[index];
                image.enabled = (spr != null);
                image.sprite = spr;
            }
        }
    }
}
