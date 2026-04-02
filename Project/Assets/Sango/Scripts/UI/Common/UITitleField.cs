using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UITitleField : MonoBehaviour
    {
        public Text title;

        public void Set(string title)
        {
            this.title.text = title;
        }
    }
}