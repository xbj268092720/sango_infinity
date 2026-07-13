using UnityEngine;
using UnityEngine.UI;

using Sango.Core;
namespace Sango.UI
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
        public Image progressImg;
        public Image bgImg;
        public UIButtonItem downloadBtn;
        public Mod.Mod mod;

        public int targetIndex;
        private System.Action onClickCall;
        private System.Action<int, bool> onToggleCall;
        public System.Action<int> onUpCall;
        public System.Action<int> onDownCall;
        public System.Action<UIModItem> onDownloadCall;

        public Button upButton;
        public Button downButton;
        public void OnClickDownload()
        {
            //onDownloadCall?.Invoke(this);
            if (mod != null)
            {
                if (!mod.IsDownloading())
                    mod.Download();
            }
        }

        public void SetMod(Mod.Mod m)
        {
            mod = m;
            if (mod != null)
            {
                enableToggle.gameObject.SetActive(mod.IsValidMod());
                downloadBtn.gameObject.SetActive(mod.CanUpgrage());
                SetName(mod.Name);
                if (mod.CanUpgrage())
                {
                    if (mod.IsValidMod())
                    {
                        string txt = $"当前版本:{mod.Version}, 线上版本:{mod.UrlVersion}.下载大小:{GameUtility.FormatFileSizeStr(mod.Size)}";

                    }
                    else
                    {
                        string txt = $"线上版本:{mod.UrlVersion}.下载大小:{GameUtility.FormatFileSizeStr(mod.Size)}";
                    }
                }
                else
                {
                    SetVersion(mod.Version);
                }
            }
            else
            {
                SetSelected(false);
                SetEnabled(false);
                SetName("");
                SetVersion("");
                enableToggle.gameObject.SetActive(false);
                downloadBtn.gameObject.SetActive(false);
            }
        }

        public void OnClickUp()
        {
            onUpCall?.Invoke(targetIndex);
        }
        public void OnClickDown()
        {
            onDownCall?.Invoke(targetIndex);
        }

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
            if (upButton != null)
            {
                upButton.gameObject.SetActive(false);
            }
            if (downButton != null)
            {
                downButton.gameObject.SetActive(false);
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

        public void BindToggleCall(System.Action<int, bool> call)
        {
            onToggleCall = call;
            if (enableToggle != null)
            {
                enableToggle.onValueChanged.RemoveAllListeners();
                enableToggle.onValueChanged.AddListener((value) => { onToggleCall?.Invoke(targetIndex, value); });
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


            if (enableToggle != null && enableToggle.isOn == true)
            {
                if (upButton != null)
                {
                    upButton.gameObject.SetActive(b);
                }
                if (downButton != null)
                {
                    downButton.gameObject.SetActive(b);
                }
            }
            else
            {
                if (upButton != null)
                {
                    upButton.gameObject.SetActive(false);
                }
                if (downButton != null)
                {
                    downButton.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (mod != null && mod.IsDownloading())
            {
                progressImg.enabled = true;
                progressImg.fillAmount = mod.loadProgress;
            }
            else
            {
                progressImg.enabled = false;
                progressImg.fillAmount = 0;
            }
        }
    }
}
