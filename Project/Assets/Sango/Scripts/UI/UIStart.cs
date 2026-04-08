using Sango.Manager;
using UnityEngine;
using UnityEngine.UI;

using Sango.Core; namespace Sango.UI
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
#if UNITY_ANDROID && !UNITY_EDITOR
            mapEditorBtn.SetActive(false);
#endif
        }

        public void OnNewGame()
        {
            GameMedia.Instance.PlayButtonSfx();
            Game.Instance.StartNewGame();
        }
        public void QuitGame()
        {
            GameMedia.Instance.PlayButtonSfx();
            GameDialog.Open("是否要退出游戏??", () =>
            {
                GameDialog.Close();
                Application.Quit();
            }).cancelAction = () =>
            {
                GameDialog.Close();
            };
        }

        public void OnGameSetting()
        {
            GameMedia.Instance.PlayButtonSfx();
            Window.Instance.Open("window_game_setting");
        }

        public void OnMapEditor()
        {
            GameMedia.Instance.PlayButtonSfx();
            Game.Instance.EnterMapEditor();
        }

        public void OnLoadGame()
        {
            GameMedia.Instance.PlayButtonSfx();
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
            GameMedia.Instance.PlayButtonSfx();
            Window.Instance.Open("window_mod_manager");
            Window.Instance.Close("window_start");
        }
    }
}
