using UnityEngine;
using UnityEngine.UI;

using Sango.Core; namespace Sango.UI
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