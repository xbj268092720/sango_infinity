using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sango.Mod;

using Sango.Core; namespace Sango.UI
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
        List<Mod.Mod> enabledMods = new List<Mod.Mod>();

        protected override void Awake()
        {
            itemCount = uIModItems.Length;
            uIObjectListItemsRect = new RectTransform[uIModItems.Length];
            for (int i = 0; i < uIModItems.Length; i++)
                uIObjectListItemsRect[i] = uIModItems[i].GetComponent<RectTransform>();
        }

        public override void OnOpen()
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
                for(int i = 0; i < enabledModNames.Length;i++)
                {
                    string modName = enabledModNames[i];
                    Mod.Mod mod = allMods.Find(x => x.Id == modName);
                    if (mod != null)
                    {
                        enabledMods.Add(mod);
                    }
                }
            }

            SortMod();
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
                if (!enabledMods.Contains(mod))
                {
                    enabledMods.Add(mod);
                }
            }
            else
            {
                enabledMods.Remove(mod);
            }
            SortMod();
            UpdateItemStartIndex(startIndex);
        }

        void SortMod()
        {
            List<Mod.Mod> mods = new List<Mod.Mod> (enabledMods);
            for(int i = 0; i < allMods.Count; i++)
            {
                Mod.Mod dest = allMods[i];
                if(!enabledMods.Contains (dest))
                    mods.Add(dest);
            }
            allMods = mods;
        }


        public void OnModUp(int index)
        {
            Mod.Mod mod = allMods[index];
            int i = 0;
            for(int j = 1; j < enabledMods.Count; j++)
            {
                if (enabledMods[j] == mod)
                {
                    Mod.Mod temp = enabledMods[i];
                    enabledMods[i] = mod;
                    enabledMods[j] = temp;
                }
                else
                    i++;
            }
            SortMod();
            UpdateItemStartIndex(startIndex);
        }

        public void OnModDown(int index)
        {
            Mod.Mod mod = allMods[index];
            int i = 1;
            for (int j = 0; j < enabledMods.Count - 1; j++)
            {
                if (enabledMods[j] == mod)
                {
                    Mod.Mod temp = enabledMods[i];
                    enabledMods[i] = mod;
                    enabledMods[j] = temp;
                }
                else
                    i++;
            }

            SortMod();
            UpdateItemStartIndex(startIndex);
        }

        public void ShowModInfo(int index)
        {
            if (index < 0 || index >= allMods.Count)
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

            Mod.Mod mod = allMods[index];
            modInfoText.text = $"{mod.Name} v{mod.Version}";
            modDescriptionText.text = mod.Description;
            
            if (modPosterImg != null && !string.IsNullOrEmpty(mod.Poster))
            {
                    modPosterImg.enabled = true;
                string posterPath = mod.GetFullPath(mod.Poster);
                modPosterImg.texture = Loader.ObjectLoader.LoadObject<Texture>(posterPath, false, false);
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
            //// 重新初始化MOD
            //ModManager.Instance.InitMods(enabledMods.ToArray());
            // 显示提示，需要重启游戏才能生效
            GameDialog.Open("修改MOD后需要重启游戏才能生效，点击确定重启游戏。", () =>
            {
                // 重启游戏
                Application.Quit();
            });
        }

        public void OnScrollBarValueChange(float value)
        {
            startIndex = (int)UnityEngine.Mathf.Lerp(0, totalCount - itemCount, value);
            UpdateItemStartIndex(startIndex);
        }

        public void UpdateItemStartIndex(int startIndex)
        {
            int enabledCount = enabledMods.Count;
            for (int i = 0; i < itemCount; i++)
            {
                UIModItem listItem = uIModItems[i];
                int destIndex = i + startIndex;
                listItem.targetIndex = destIndex;
                if (destIndex < totalCount)
                {
                    Mod.Mod mod = allMods[destIndex];
                    listItem.SetSelected(selectedIndex == destIndex).SetEnabled(destIndex < enabledCount).SetName(mod.Name).SetVersion(mod.Version).enableToggle.gameObject.SetActive(true);
                    listItem.BindToggleCall(OnToggleMod);
                    listItem.onUpCall = OnModUp;
                    listItem.onDownCall = OnModDown;
                }
                else
                {
                    listItem.SetSelected(false).SetEnabled(false).SetName("").SetVersion("").enableToggle.gameObject.SetActive(false);
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
                if (listItem == currentSelectItem && RectTransformUtility.RectangleContainsScreenPoint(itemRect, Input.mousePosition, Sango.Core.Game.Instance.UICamera))
                {
                    OnItemSelected(item);
                    break;
                }
            }
        }

        public void OnObjectListItemPointEnter(UIModItem item)
        {
            item.SetOver(true);
            ShowModInfo(item.targetIndex);
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
