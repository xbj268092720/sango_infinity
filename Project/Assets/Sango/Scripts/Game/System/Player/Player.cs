namespace Sango.Core.Player
{
    [GameSystem]
    public class Player : GameSystem
    {
        public ShortScenario[] all_saved_scenario_list = new ShortScenario[50];

        public override void Init()
        {
            InitSaveFile();
        }

        string GetSaveFileName(int index)
        {
            return $"{Path.SaveRootPath}/Save/save{index}.json";
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

        public void Save(int index)
        {
            string fileName = GetSaveFileName(index + 1);
            GameEvent.OnGameSave?.Invoke(Scenario.Cur, index);
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
