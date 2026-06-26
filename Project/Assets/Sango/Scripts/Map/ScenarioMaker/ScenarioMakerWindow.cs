using Sango.Core;
using Sango.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Sango.ScenarioMaker
{
    /// <summary>
    /// 剧本编辑器主窗口
    /// 提供信息、武将、势力、军团、城池五个页签的数据编辑
    /// </summary>
    public class ScenarioMakerWindow : EditorWindow
    {
        /// <summary>
        /// 编辑器页签类型
        /// </summary>
        private enum EditTab
        {
            Info,
            Person,
            Force,
            Corps,
            City
        }

        /// <summary>
        /// 当前页签
        /// </summary>
        private EditTab currentTab = EditTab.Info;

        /// <summary>
        /// 列表滚动位置
        /// </summary>
        private Vector2 listScroll = Vector2.zero;

        /// <summary>
        /// 详情滚动位置
        /// </summary>
        private Vector2 detailScroll = Vector2.zero;

        /// <summary>
        /// 剧本列表滚动位置
        /// 用于在没有剧本时展示 ShortScenario.all_scenario_info_list
        /// </summary>
        private Vector2 scenarioListScroll = Vector2.zero;

        /// <summary>
        /// 列表搜索过滤
        /// </summary>
        private string searchFilter = "";

        /// <summary>
        /// 各页签当前选中对象的ID
        /// </summary>
        private int selectedPersonId = 0;
        private int selectedForceId = 0;
        private int selectedCorpsId = 0;
        private int selectedCityId = 0;

        /// <summary>
        /// 初始化窗口
        /// </summary>
        public void Initialize()
        {
            windowRect = new UnityEngine.Rect(Screen.width / 2f - 400f, Screen.height / 2f - 300f, 800f, 600f);
            windowName = "剧本编辑器";
            windowId = 1001;
            visible = true;
        }

        /// <summary>
        /// 绑定外部剧本
        /// </summary>
        /// <param name="scenario">剧本实例</param>
        public void SetScenario(Scenario scenario)
        {
            ScenarioMaker.Instance.SetScenario(scenario);
        }

        /// <summary>
        /// 窗口绘制回调
        /// </summary>
        private void OnDrawWindow(int winId)
        {
            if (ScenarioMaker.Instance.Scenario == null)
            {
                DrawNoScenario();
                return;
            }

            DrawToolbar();
            DrawTabs();

            GUILayout.BeginHorizontal();
            DrawListPanel();
            DrawDetailPanel();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 没有剧本时的提示界面
        /// 展示所有可用的剧本文件，由玩家选择加载
        /// </summary>
        private void DrawNoScenario()
        {
            GUILayout.Label("当前没有加载剧本，请从列表中选择一个剧本进行编辑");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新建剧本"))
            {
                ScenarioMaker.Instance.NewScenario();
            }
            if (GUILayout.Button("加载其他剧本"))
            {
                string[] path = WindowDialog.OpenFileDialog(
                    "加载剧本",
                    $"{Sango.Path.ContentRootPath}/Scenario",
                    "剧本文件(*.json)\0*.json;\0\0");

                if (path != null && path.Length > 0)
                {
                    ScenarioMaker.Instance.LoadScenario(path[0]);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("可选剧本列表");
            scenarioListScroll = GUILayout.BeginScrollView(scenarioListScroll, GUILayout.Height(400f));

            List<ShortScenario> list = ShortScenario.all_scenario_info_list;
            if (list.Count == 0)
            {
                GUILayout.Label("暂无可用剧本文件");
            }
            else
            {
                foreach (ShortScenario shortScenario in list)
                {
                    if (shortScenario == null || shortScenario.Info == null)
                    {
                        continue;
                    }

                    string label = string.IsNullOrEmpty(shortScenario.ModName)
                        ? shortScenario.GetIDName()
                        : shortScenario.GetModIDName(shortScenario.ModName);

                    if (GUILayout.Button(label))
                    {
                        ScenarioMaker.Instance.LoadScenario(shortScenario.FilePath);
                        Sango.Log.Info($"加载剧本: {shortScenario.Name}");
                        break;
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 顶部工具栏
        /// </summary>
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("新建"))
            {
                ScenarioMaker.Instance.NewScenario();
            }
            if (GUILayout.Button("加载"))
            {
                string[] path = WindowDialog.OpenFileDialog(
                    "加载剧本",
                    $"{Sango.Path.ContentRootPath}/Scenario",
                    "剧本文件(*.json)\0*.json;\0\0");

                if (path != null && path.Length > 0)
                {
                    ScenarioMaker.Instance.LoadScenario(path[0]);
                }
            }
            if (GUILayout.Button("保存"))
            {
                string path = WindowDialog.SaveFileDialog(
                    "保存剧本",
                    $"{Sango.Path.ContentRootPath}/Scenario",
                    "剧本.json",
                    "剧本文件(*.json)\0*.json;\0\0");
                if (!string.IsNullOrEmpty(path))
                {
                    ScenarioMaker.Instance.SaveScenario(path);
                }
            }
            GUILayout.Label($"剧本: {ScenarioMaker.Instance.Scenario.Info?.name ?? ""}");
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 页签切换
        /// </summary>
        private void DrawTabs()
        {
            int tabIndex = (int)currentTab;
            string[] tabNames = { "信息", "武将", "势力", "军团", "城池" };
            int newTab = GUILayout.SelectionGrid(tabIndex, tabNames, tabNames.Length);
            if (newTab != tabIndex)
            {
                currentTab = (EditTab)newTab;
            }
        }

        /// <summary>
        /// 左侧列表面板
        /// </summary>
        private void DrawListPanel()
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(250f));
            GUILayout.Label("列表");
            searchFilter = GUILayout.TextField(searchFilter);

            listScroll = GUILayout.BeginScrollView(listScroll, GUILayout.Height(400f));

            Scenario scenario = ScenarioMaker.Instance.Scenario;
            switch (currentTab)
            {
                case EditTab.Person:
                    DrawSetList(scenario.personSet, ref selectedPersonId);
                    break;
                case EditTab.Force:
                    DrawSetList(scenario.forceSet, ref selectedForceId);
                    break;
                case EditTab.Corps:
                    DrawSetList(scenario.corpsSet, ref selectedCorpsId);
                    break;
                case EditTab.City:
                    DrawSetList(scenario.citySet, ref selectedCityId);
                    break;
            }

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("添加"))
            {
                AddCurrentItem();
            }
            if (GUILayout.Button("删除"))
            {
                RemoveCurrentItem();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制单个集合的列表
        /// </summary>
        private void DrawSetList<T>(SangoObjectSet<T> set, ref int selectedId) where T : SangoObject, new()
        {
            List<int> ids = new List<int>();
            List<string> names = new List<string>();
            set.ForEach((T obj) =>
            {
                if (obj == null)
                {
                    return;
                }
                string label = $"{obj.Id}:{obj.Name}";
                if (string.IsNullOrEmpty(searchFilter) || label.Contains(searchFilter))
                {
                    ids.Add(obj.Id);
                    names.Add(label);
                }
            });

            int index = ids.IndexOf(selectedId);
            if (index < 0)
            {
                index = 0;
            }
            int newIndex = GUILayout.SelectionGrid(index, names.ToArray(), 1);
            if (newIndex != index && newIndex >= 0 && newIndex < ids.Count)
            {
                selectedId = ids[newIndex];
            }
        }

        /// <summary>
        /// 右侧详情面板
        /// </summary>
        private void DrawDetailPanel()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            detailScroll = GUILayout.BeginScrollView(detailScroll);

            Scenario scenario = ScenarioMaker.Instance.Scenario;
            switch (currentTab)
            {
                case EditTab.Info:
                    ScenarioMakerPropertyDrawer.DrawObject(scenario.Info, scenario);
                    break;
                case EditTab.Person:
                    {
                        Person person = scenario.personSet.Get(selectedPersonId);
                        if (person != null)
                        {
                            ScenarioMakerPropertyDrawer.DrawObject(person, scenario);
                        }
                    }
                    break;
                case EditTab.Force:
                    {
                        Force force = scenario.forceSet.Get(selectedForceId);
                        if (force != null)
                        {
                            ScenarioMakerPropertyDrawer.DrawObject(force, scenario);
                        }
                    }
                    break;
                case EditTab.Corps:
                    {
                        Corps corps = scenario.corpsSet.Get(selectedCorpsId);
                        if (corps != null)
                        {
                            ScenarioMakerPropertyDrawer.DrawObject(corps, scenario);
                        }
                    }
                    break;
                case EditTab.City:
                    {
                        City city = scenario.citySet.Get(selectedCityId);
                        if (city != null)
                        {
                            ScenarioMakerPropertyDrawer.DrawObject(city, scenario);
                        }
                    }
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 根据当前页签添加新对象
        /// </summary>
        private void AddCurrentItem()
        {
            Scenario scenario = ScenarioMaker.Instance.Scenario;
            switch (currentTab)
            {
                case EditTab.Person:
                    {
                        Person person = new Person { Id = -1, Name = "新武将" };
                        scenario.Add(person);
                        selectedPersonId = person.Id;
                        Sango.Log.Info($"添加武将: {person.Name}");
                    }
                    break;
                case EditTab.Force:
                    {
                        Force force = new Force { Id = -1 };
                        scenario.Add(force);
                        selectedForceId = force.Id;
                        Sango.Log.Info($"添加势力: ID={force.Id}");
                    }
                    break;
                case EditTab.Corps:
                    {
                        Corps corps = new Corps { Id = -1, number = 1 };
                        scenario.Add(corps);
                        selectedCorpsId = corps.Id;
                        Sango.Log.Info($"添加军团: ID={corps.Id}");
                    }
                    break;
                case EditTab.City:
                    {
                        City city = new City { Id = -1, Name = "新城池" };
                        if (scenario.CommonData != null)
                        {
                            city.BuildingType = scenario.CommonData.BuildingTypes.Get(1);
                            city.CityLevelType = scenario.CommonData.CityLevelTypes.Get(1);
                        }
                        scenario.Add(city);
                        selectedCityId = city.Id;
                        Sango.Log.Info($"添加城池: {city.Name}");
                    }
                    break;
            }
        }

        /// <summary>
        /// 根据当前页签删除选中对象
        /// </summary>
        private void RemoveCurrentItem()
        {
            Scenario scenario = ScenarioMaker.Instance.Scenario;
            switch (currentTab)
            {
                case EditTab.Person:
                    {
                        Person person = scenario.personSet.Get(selectedPersonId);
                        if (person != null)
                        {
                            scenario.Remove(person);
                            selectedPersonId = 0;
                            Sango.Log.Info($"删除武将: {person.Name}");
                        }
                    }
                    break;
                case EditTab.Force:
                    {
                        Force force = scenario.forceSet.Get(selectedForceId);
                        if (force != null)
                        {
                            scenario.Remove(force);
                            selectedForceId = 0;
                            Sango.Log.Info($"删除势力: ID={force.Id}");
                        }
                    }
                    break;
                case EditTab.Corps:
                    {
                        Corps corps = scenario.corpsSet.Get(selectedCorpsId);
                        if (corps != null)
                        {
                            scenario.Remove(corps);
                            selectedCorpsId = 0;
                            Sango.Log.Info($"删除军团: ID={corps.Id}");
                        }
                    }
                    break;
                case EditTab.City:
                    {
                        City city = scenario.citySet.Get(selectedCityId);
                        if (city != null)
                        {
                            scenario.Remove(city);
                            selectedCityId = 0;
                            Sango.Log.Info($"删除城池: {city.Name}");
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 重写OnGUI以实现窗口渲染
        /// </summary>
        private new void OnGUI()
        {
            if (visible)
            {
                windowRect = GUILayout.Window(windowId, windowRect, OnDrawWindow, windowName);
                ConstrainWindowToScreen();
            }
        }
    }
}
