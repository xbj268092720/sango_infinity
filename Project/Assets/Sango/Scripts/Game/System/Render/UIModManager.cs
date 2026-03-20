using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sango.Mod;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// MOD管理界面
    /// </summary>
    public class UIModManager : UGUIWindow
    {
        public Text modInfoText;
        public Text modDescriptionText;
        public RawImage modPosterImg;
        public Button backButton;
        public Button applyButton;


        public RectTransform sliderRect;
        public Scrollbar scrollbar;
        public UIModItem[] uIModItems;
        UIModItem currentSelectItem;
        RectTransform[] uIObjectListItemsRect;
        protected int startIndex = 0;
        protected int itemCount = 0;
        protected int totalCount = 0;
        protected int selectedIndex = 0;


        List<Mod.Mod> allMods = new List<Mod.Mod>();
        List<string> enabledMods = new List<string>();

        protected override void Awake()
        {
            itemCount = uIModItems.Length;
            uIObjectListItemsRect = new RectTransform[uIModItems.Length];
            for (int i = 0; i < uIModItems.Length; i++)
                uIObjectListItemsRect[i] = uIModItems[i].GetComponent<RectTransform>();
        }

        public override void OnShow()
        {
            LoadModList();
            selectedIndex = 0;
            ShowModInfo(-1);
            BindEvents();

            totalCount = allMods.Count;
            if (totalCount < itemCount)
            {
                sliderRect.gameObject.SetActive(false);
            }
            else
            {
                sliderRect.gameObject.SetActive(true);
                scrollbar.size = (float)itemCount / (float)totalCount;
                scrollbar.SetValueWithoutNotify(0);
            }
            startIndex = 0;
            OnScrollBarValueChange(0);
        }

        private void LoadModList()
        {
            allMods.Clear();
            enabledMods.Clear();

            // 获取所有MOD
            foreach (var mod in ModManager.Instance.mModMap.Values)
            {
                allMods.Add(mod);
            }

            // 获取已启用的MOD
            string[] enabledModNames = ModManager.Instance.LoadModList();
            if (enabledModNames != null)
            {
                enabledMods.AddRange(enabledModNames);
            }
        }

        private void BindEvents()
        {
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnReturn);
            }

            if (applyButton != null)
            {
                applyButton.onClick.RemoveAllListeners();
                applyButton.onClick.AddListener(OnApply);
            }
        }

        public void OnSelectMod(int index)
        {
            ShowModInfo(index);
        }

        public void OnToggleMod(int index, bool isEnabled)
        {
            Mod.Mod mod = allMods[index];
            if (isEnabled)
            {
                if (!enabledMods.Contains(mod.Id))
                {
                    enabledMods.Add(mod.Id);
                }
            }
            else
            {
                enabledMods.Remove(mod.Id);
            }
        }

        public void ShowModInfo(int index)
        {
            if (index == selectedIndex)
                return;

            selectedIndex = index;
            if (index < 0)
            {
                modInfoText.text = "";
                modDescriptionText.text = "";
                if (modPosterImg != null)
                {
                    modPosterImg.texture = null;
                    modPosterImg.enabled = false;
                }
                return;
            }

            Mod.Mod mod = allMods[selectedIndex];
            modInfoText.text = $"{mod.Name} v{mod.Version}";
            modDescriptionText.text = mod.Description;
            
            if (modPosterImg != null && !string.IsNullOrEmpty(mod.Poster))
            {
                    modPosterImg.enabled = true;
                string posterPath = mod.GetFullPath(mod.Poster);
                modPosterImg.texture = Loader.ObjectLoader.LoadObject<Texture>(posterPath, true, false);
            }
        }

        public void OnReturn()
        {
            Window.Instance.Open("window_start");
            Window.Instance.Close("window_mod_manager");
        }

        public void OnApply()
        {
            // 保存MOD列表
            ModManager.Instance.SaveModList(enabledMods.ToArray());
            // 重新初始化MOD
            ModManager.Instance.InitMods(enabledMods.ToArray());
            // 显示提示，需要重启游戏才能生效
            UIDialog.Open("修改MOD后需要重启游戏才能生效，点击确定重启游戏。", () =>
            {
                // 重启游戏
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });
        }

        public void OnScrollBarValueChange(float value)
        {
            startIndex = (int)UnityEngine.Mathf.Lerp(0, totalCount - itemCount, value);
            UpdateItemStartIndex(startIndex);
        }

        public void UpdateItemStartIndex(int startIndex)
        {
            for (int i = 0; i < itemCount; i++)
            {
                UIModItem listItem = uIModItems[i];
                int destIndex = i + startIndex;
                listItem.targetIndex = destIndex;
                if (destIndex < totalCount)
                {
                    Mod.Mod mod = allMods[destIndex];
                    listItem.SetSelected(selectedIndex == destIndex).SetName(mod.Name).SetVersion(mod.Version);
                }
                else
                {
                    listItem.SetSelected(false).SetName("").SetVersion("");
                }
            }
        }

        public void UpShow()
        {
            if (startIndex > 0)
                startIndex--;
            UpdateItemStartIndex(startIndex);
            scrollbar.SetValueWithoutNotify((float)startIndex / (totalCount - itemCount));
        }

        public void DownShow()
        {
            if (startIndex < totalCount - itemCount)
                startIndex++;

            UpdateItemStartIndex(startIndex);
            scrollbar.SetValueWithoutNotify((float)startIndex / (totalCount - itemCount));
        }

        public void OnObjectListItemPressDown(UIModItem item)
        {
            item.SetPressd(true);
            currentSelectItem = item;
        }

        public void OnObjectListItemPressUp(UIModItem item)
        {
            item.SetPressd(false);
            for (int i = 0; i < itemCount; i++)
            {
                RectTransform itemRect = uIObjectListItemsRect[i];
                UIModItem listItem = uIModItems[i];
                if (listItem == currentSelectItem && RectTransformUtility.RectangleContainsScreenPoint(itemRect, Input.mousePosition, Sango.Game.Game.Instance.UICamera))
                {
                    OnItemSelected(item);
                    break;
                }
            }
        }

        public void OnObjectListItemPointEnter(UIModItem item)
        {
            item.SetOver(true);
        }
        public void OnObjectListItemPointExit(UIModItem item)
        {
            item.SetOver(false);
        }

        public virtual void OnItemSelected(UIModItem item)
        {
            if (item.targetIndex >= totalCount)
                return;

            int destIndex = item.targetIndex;
            if (destIndex == selectedIndex)
                return;

            for (int i = 0; i < itemCount; i++)
            {
                UIModItem listItem = uIModItems[i];
                if (listItem.targetIndex == selectedIndex)
                {
                    listItem.SetSelected(false);
                    break;
                }
            }
            selectedIndex = destIndex;
            item.SetSelected(true);
            OnSelectMod(selectedIndex);
        }
    }
}
