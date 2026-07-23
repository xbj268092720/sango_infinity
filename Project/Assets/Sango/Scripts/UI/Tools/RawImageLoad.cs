using UnityEngine;
using UnityEngine.UI;

namespace Sango.UI.Tools
{
    public class RawImageLoad : MonoBehaviour
    {
        public string imgName;
        public void Start()
        {
            RawImage rawImage = GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = Resources.Load<Texture>(imgName);
            }
        }
    }
}
