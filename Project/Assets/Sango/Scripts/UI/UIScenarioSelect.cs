using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 剧本选择界面
    /// </summary>
    public class UIScenarioSelect : UGUIWindow
    {
        public int curSelectIndex;
        public UIScenarioItem uIScenarioItem;
        public Text scenarioDescText;
        public Text scenarioInfoText;
        public RawImage scenarioPosterImg;

        List<UIScenarioItem> selectedItems = new List<UIScenarioItem>();

        public GameObject cityObject;
        public RectTransform mapBounds;
        List<GameObject> cityList = new List<GameObject>();

        public GameObject sureButton;

        public override void OnShow()
        {
            curSelectIndex = -1;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            sureButton.SetActive(false);
#else
            sureButton.SetActive(true);
#endif
            int count = ShortScenario.all_scenario_info_list.Count;
            for (int i = 0; i < count; i++)
            {
                ShortScenario scenario = ShortScenario.all_scenario_info_list[i];
                UIScenarioItem item;
                if (i < selectedItems.Count)
                {
                    item = selectedItems[i];
                }
                else
                {
                    item = GameObject.Instantiate(uIScenarioItem.gameObject, uIScenarioItem.transform.parent).GetComponent<UIScenarioItem>();
                    selectedItems.Add(item);
                }
                item.gameObject.SetActive(true);
                int selIndex = i;
                item.targetIndex = selIndex;
                item.SetName(scenario.GetIDName()).SetSelected(i == curSelectIndex);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                item.BindCall(() => { OnNext(); });
#else
                item.BindCall(() => { OnSelectScenario(selIndex); });
#endif


            }

            for (int i = count; i < selectedItems.Count; i++)
            {
                selectedItems[i].gameObject.SetActive(false);
            }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            ShowScenario(curSelectIndex);
#else
            ShowScenario(0);
#endif
        }

        public void Clear()
        {

        }

        public void OnSelectScenario(int index)
        {
            if (curSelectIndex != index)
            {
                if(curSelectIndex >= 0 &&  curSelectIndex < selectedItems.Count)
                    selectedItems[curSelectIndex].SetSelected(false);
            }
            selectedItems[index].SetSelected(true);
            ShowScenario(index);
        }

        public void ShowScenario(int index)
        {
            if (index == curSelectIndex)
                return;

            curSelectIndex = index;
            if (index < 0)
            {
                scenarioInfoText.text = "";
                scenarioDescText.text = "";
                for (int j = 0; j < cityList.Count; j++)
                {
                    cityList[j].SetActive(false);
                }
                return;
            }

            ShortScenario scenario = ShortScenario.all_scenario_info_list[curSelectIndex];
            ScenarioInfo scenarioInfo = scenario.Info;
            scenarioInfoText.text = scenario.GetDateName();
            scenarioDescText.text = scenarioInfo.description;
            int i = 0;
            foreach (var city in scenario.citySet.Values)
            {

                if (city.BuildingType > 1) continue;
                if (city.Id == 0) continue;

                GameObject cityObj;
                if (i >= cityList.Count)
                {
                    cityObj = GameObject.Instantiate(cityObject, cityObject.transform.parent);
                    cityList.Add(cityObj);
                    cityObj.name = city.Id.ToString();
                }
                else
                {
                    cityObj = cityList[i];
                    cityObj.name = city.Id.ToString();
                }

                i++;
                RectTransform rectTransform = cityObj.GetComponent<RectTransform>();
                float x = city.x * mapBounds.sizeDelta.x / scenario.Map.Width - mapBounds.sizeDelta.x / 2;
                float y = mapBounds.sizeDelta.y / 2 - city.y * mapBounds.sizeDelta.y / scenario.Map.Height;
                rectTransform.anchoredPosition = new Vector2((int)(x + 0.5f), (int)(y + 0.5f));

                Image bgImg = cityObj.transform.GetChild(0).GetComponent<Image>();
                if (city.BelongForce > 0)
                {
                    ShortForce shortForce = scenario.forceSet[city.BelongForce];
                    Flag flag = scenario.CommonData.Flags[shortForce.Flag];
                    bgImg.color = flag.color;
                }
                else
                {
                    bgImg.color = Color.white;
                }

                cityObj.SetActive(true);
            }

            for (int j = i; j < cityList.Count; j++)
            {
                cityList[j].SetActive(false);
            }

        }

        public void OnReturn()
        {
            Clear();
            Window.Instance.Open("window_start");
            Window.Instance.Close("window_scenario_select");
        }

        public void OnNext()
        {
            if (curSelectIndex == -1) return;

            Clear();
            ShortScenario scenario = ShortScenario.all_scenario_info_list[curSelectIndex];
            ShortScenario.CurSelected = scenario;

            Scenario.CurSelected = new Scenario(Scenario.all_scenario_list[curSelectIndex].FilePath);

            Window.Instance.Open("window_scenario_force_select");
            Window.Instance.Close("window_scenario_select");
        }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        private void Update()
        {
            for (int i = 0; i < selectedItems.Count; i++)
            {
                UIScenarioItem uIScenarioItem = selectedItems[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(uIScenarioItem.root, Input.mousePosition, Game.Instance.UICamera))
                {
                    ShowScenario(uIScenarioItem.targetIndex);
                    return;
                }
            }

            ShowScenario(-1);

        }
#endif
    }
}
