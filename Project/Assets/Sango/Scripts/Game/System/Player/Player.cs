using System.Collections.Generic;

namespace Sango.Core.Player
{
    [GameSystem]
    public class Player : GameSystem
    {
        public ShortScenario[] all_saved_scenario_list = new ShortScenario[40];
        public ShortScenario[] all_auto_saved_scenario_list = new ShortScenario[10];
        private int autoSaveIndex = -1;
        private long autoSaveTime = 0;
        public bool autoSave = true;
        public int autoSaveTurnType = 0;
        int[] autoTurn = new int[] { 2, 3, 4, 6, 10 };
        public int currentTurnCount = 0;

        public override void Init()
        {
            InitSaveFile();
            InitAutoSaveFile();
            currentTurnCount = 0;
            GameEvent.OnForceTurnStart += OnForceTurnStart;
            GameEvent.OnScenarioVariablesSetting += OnScenarioVariablesSetting;
            GameEvent.OnScenarioInit += OnScenarioInit;
            GameEvent.OnGameSave += OnGameSave;
        }

        public override void Clear()
        {
            GameEvent.OnForceTurnStart -= OnForceTurnStart;
            GameEvent.OnScenarioVariablesSetting -= OnScenarioVariablesSetting;
            GameEvent.OnScenarioInit -= OnScenarioInit;
            GameEvent.OnGameSave -= OnGameSave;
        }

        /// <summary>
        /// 保存自定义数据
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="index"></param>
        void OnGameSave(Scenario scenario, int index, bool isAuto)
        {
            scenario.Variables.SetExtensionData("AutoSave", autoSave);
            scenario.Variables.SetExtensionData("AutoSaveType", autoSaveTurnType);
        }

        void OnScenarioInit(Scenario scenario)
        {
            if (scenario.Variables.HasExtensionData("AutoSave"))
                autoSave = scenario.Variables.GetExtensionData<bool>("AutoSave");
            if (scenario.Variables.HasExtensionData("AutoSaveType"))
                autoSaveTurnType = scenario.Variables.GetExtensionData<int>("AutoSaveType");
        }

        void OnScenarioVariablesSetting(IVariablesSetting variablesSetting, Scenario scenario)
        {
            variablesSetting.AddToggleItem("自动存档", autoSave,
                (v) =>
                {
                    autoSave = v;
                });
            variablesSetting.AddDropdownItem("自动存档间隔回合", autoSaveTurnType,
                new List<string>(new string[]
                {
                    "1回合",
                    "2回合",
                    "3回合",
                    "5回合",
                    "10回合",
                }),
                (index) =>
                {
                    autoSaveTurnType = index;
                });
        }


        void OnForceTurnStart(Force force, Scenario scenario)
        {
            if (autoSave && force.IsPlayer)
            {
                currentTurnCount++;
                if (currentTurnCount >= autoTurn[autoSaveTurnType])
                {
                    AutoSave();
                    currentTurnCount = 0;
                }
            }
        }

        string GetSaveFileName(int index)
        {
            return $"{Path.SaveRootPath}/Save/save{index}.json";
        }
        string GetAutoSaveFileName(int index)
        {
            return $"{Path.SaveRootPath}/Save/auto_save{index}.json";
        }

        void InitSaveFile()
        {
            for (int i = 1; i <= all_saved_scenario_list.Length; i++)
            {
                string fileName = GetSaveFileName(i);
                if (File.Exists(fileName))
                {
#if SANGO_DEBUG
                    Sango.Log.Print($"Find Saved data : {fileName}");
#endif
                    ShortScenario scenario = new ShortScenario(fileName);
                    all_saved_scenario_list[i - 1] = scenario;
                }
            }
        }
        void InitAutoSaveFile()
        {
            for (int i = 1; i <= all_auto_saved_scenario_list.Length; i++)
            {
                string fileName = GetAutoSaveFileName(i);
                if (File.Exists(fileName))
                {
#if SANGO_DEBUG
                    Sango.Log.Print($"Find Saved data : {fileName}");
#endif
                    ShortScenario scenario = new ShortScenario(fileName);
                    if (scenario.Info.dateTime > autoSaveTime)
                    {
                        autoSaveTime = scenario.Info.dateTime;
                        autoSaveIndex = i - 1;
                        if (autoSaveIndex >= all_auto_saved_scenario_list.Length)
                            autoSaveIndex = 0;
                    }
                    all_auto_saved_scenario_list[i - 1] = scenario;
                }
                else
                {
                    if (autoSaveIndex == -1)
                        autoSaveIndex = i - 1;
                }
            }
        }
        public void Save(int index)
        {
            string fileName = GetSaveFileName(index + 1);
            GameEvent.OnGameSave?.Invoke(Scenario.Cur, index, false);
            Scenario.Cur.Save(fileName);
            ShortScenario scenario = new ShortScenario(fileName);
            all_saved_scenario_list[index] = scenario;
        }

        public void Load(int index)
        {
            Window.Instance.CloseAll();
            Window.Instance.Open("window_loading");
            Quit();
            string fileName = GetSaveFileName(index + 1);
            Scenario.CurSelected = new Scenario(fileName);
            Scenario.StartScenario(Scenario.CurSelected);
        }

        public void AutoSave()
        {
            string fileName = GetAutoSaveFileName(autoSaveIndex + 1);
            GameEvent.OnGameSave?.Invoke(Scenario.Cur, autoSaveIndex, true);
            Scenario.Cur.Save(fileName);
            ShortScenario scenario = new ShortScenario(fileName);
            all_auto_saved_scenario_list[autoSaveIndex] = scenario;
            autoSaveIndex++;
            if (autoSaveIndex >= all_auto_saved_scenario_list.Length)
                autoSaveIndex = 0;
        }

        public void LoadAutoFile(int index)
        {
            Window.Instance.CloseAll();
            Window.Instance.Open("window_loading");
            Quit();
            string fileName = GetAutoSaveFileName(index + 1);
            Scenario.CurSelected = new Scenario(fileName);
            Scenario.StartScenario(Scenario.CurSelected);
        }

        public void Quit()
        {
            Scenario.Cur?.OnGameShutdown();
        }

        public void QuitToMainMenu()
        {
            Scenario.Cur?.OnGameShutdown();
            Window.Instance.CloseAll();
            Window.Instance.DestroyAll();
            Window.Instance.Open("window_start");

        }
    }
}
