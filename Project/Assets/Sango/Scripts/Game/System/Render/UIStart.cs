using Sango.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game.Render.UI
{
    /// <summary>
    /// 游戏开始界面
    /// </summary>
    public class UIStart : UGUIWindow
    {
        public Text version;
        public GameObject mapEditorBtn;

        private void Start()
        {
            version.text = $"版本: {Application.version}";
            AudioManager.Instance.PlayBgm("Assets/Sound/2238.ogg");
#if UNITY_ANDORID && !UNITY_EDITOR
            mapEditorBtn.SetActive(false);
#endif
        }

        public void OnNewGame()
        {
            Game.Instance.StartNewGame();
        }

        public void OnMapEditor()
        {
            Game.Instance.EnterMapEditor();
        }

        public void OnLoadGame()
        {
            Window.Instance.Open("window_scenario_save", 2);
        }

        public void OnTest()
        {
            string path = Sango.Path.FindFile("Scenario/Scenario.json");
            Scenario scenario = new Scenario(path);

            scenario.Info.cameraPosition = new UnityEngine.Vector3(1407, 0, 796);
            scenario.Info.cameraRotation = new UnityEngine.Vector3(40f, -50f, 0f);
            scenario.Info.cameraDistance = 400f;

            Scenario.StartScenario(scenario);
        }

        public void OnModManager()
        {
            Window.Instance.Open("window_mod_manager");
            Window.Instance.Close("window_start");
        }
    }
}
