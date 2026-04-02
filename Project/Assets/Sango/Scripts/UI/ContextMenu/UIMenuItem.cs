using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    public class UIMenuItem : MonoBehaviour
    {
        public Text title;
        public Button button;

        public UIMenuItem SetTitle(string t)
        {
            title.text = t;
            return this;
        }

        public UIMenuItem SetValid(bool b)
        {
            title.color = b ? Color.white : Color.gray;
            button.interactable = b;
            return this;
        }

        public UIMenuItem SetParent(Transform p)
        {
            transform.SetParent(p, false);
            transform.SetAsLastSibling();
            return this;
        }

        public UIMenuItem SetListener(UnityAction call)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(call);
            return this;
        }
    }
}
