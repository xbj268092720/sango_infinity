using UnityEngine;
using UnityEngine.UI;

namespace Sango.UI
{
    public class UIChoiseItem : MonoBehaviour
    {
        public Button button;
        public Text lab;
        public int index;
        public System.Action<UIChoiseItem> OnClickCall;
        public void OnClick()
        {
            OnClickCall?.Invoke(this);
        }

        public UIChoiseItem SetText(string t)
        {
            lab.text = t;
            return this;
        }
        public UIChoiseItem SetClickCall(System.Action<UIChoiseItem> t)
        {
            OnClickCall = t;
            return this;
        }
        public UIChoiseItem SetValid(bool b)
        {
            button.interactable = b;
            return this;
        }
    }
}
