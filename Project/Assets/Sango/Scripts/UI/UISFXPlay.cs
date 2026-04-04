using Sango.Manager;
using UnityEngine.UI;

namespace Sango.UI
{
    public class UISFXPlay : UGUIWindow
    {
        public string sfxPath;
        private void Start()
        {
            Button button = GetComponent<Button>();
            if(button != null)
            {
                button.onClick.AddListener(PlaySFX);
            }
        }

        void PlaySFX()
        {
            if(!string.IsNullOrEmpty(sfxPath))
                AudioManager.Instance.PlaySfx(sfxPath);
        }
    }
}
