using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// MOD项UI
    /// </summary>
    public class UIModItem : MonoBehaviour
    {
        public Text modNameText;
        public Text modVersionText;
        public Toggle enableToggle;
        public Image selectedImage;
        public Transform root;
        public Image pressImg;
        public Image overImg;

        public int targetIndex;
        private System.Action onClickCall;
        private System.Action<bool> onToggleCall;

        public UIModItem SetName(string name)
        {
            if (modNameText != null)
            {
                modNameText.text = name;
            }
            return this;
        }

        public UIModItem SetVersion(string version)
        {
            if (modVersionText != null)
            {
                modVersionText.text = version;
            }
            return this;
        }

        public UIModItem SetEnabled(bool enabled)
        {
            if (enableToggle != null)
            {
                enableToggle.isOn = enabled;
            }
            return this;
        }

        public UIModItem SetSelected(bool selected)
        {
            if (selectedImage != null)
            {
                selectedImage.gameObject.SetActive(selected);
            }
            return this;
        }

        public void BindCall(System.Action call)
        {
            onClickCall = call;
        }

        public void BindToggleCall(System.Action<bool> call)
        {
            onToggleCall = call;
            if (enableToggle != null)
            {
                enableToggle.onValueChanged.RemoveAllListeners();
                enableToggle.onValueChanged.AddListener((value) => { onToggleCall?.Invoke(value); });
            }
        }

        public void OnClick()
        {
            onClickCall?.Invoke();
        }
        public void SetPressd(bool b)
        {
            pressImg.enabled = b;
        }

        public void SetOver(bool b)
        {
            overImg.enabled = b;
        }
    }
}
