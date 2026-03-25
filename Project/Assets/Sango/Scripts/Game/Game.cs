using Sango.Game.Action;
using Sango.Mod;
using Sango.Render;
using Sango.Tools;
using Sango.Manager;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sango.Game
{
    public class Game : App<Game>
    {
        public Camera UICamera { get; internal set; }
        public RectTransform UIRoot { get; internal set; }
        public Canvas RootCanvas { get; internal set; }
        public CanvasScaler CanvasScaler { get; internal set; }
        public float CanvasScalerFactor { get; internal set; }
        bool inited = false;
        public override void Init(MonoBehaviour start, Platform.PlatformName targetPlatform)
        {
            inited = false;
            CanvasScalerFactor = CanvasScaler.referenceResolution.y / 1080f;
            base.Init(start, targetPlatform);
            Window.Instance.Init(1024, 720);
            ActionBase.Init();
            Condition.Init();
            BuffEffect.Init();
            SkillEffect.Init();
            PersonFunctions.Init();
            TroopCompareFunction.Init();
            SkillSuccessMethod.Init();
            SkillCriticalMethod.Init();
            ModManager.Instance.Init();
            GameLanguage.Instance.Init("cn");
            GameSystemManager.Instance.Init();
            StartCoroutine(GameInit());
        }

        public override void Shutdown()
        {
            MapRender.Instance.Clear();
#if SANGO_DEBUG
            Sango.Log.Print("游戏关闭");
#endif
            GameEvent.OnGameShutdown?.Invoke();
        }

        public override void Pause()
        {
#if SANGO_DEBUG
            Sango.Log.Print("游戏暂停");
#endif
            GameEvent.OnGamePause?.Invoke();
        }

        public override void Resume()
        {
#if SANGO_DEBUG
            Sango.Log.Print("游戏恢复");
#endif
            GameEvent.OnGameResume?.Invoke();
        }

        IEnumerator GameInit()
        {
            Window.Instance.Open("window_loading");
            //yield return new WaitForSeconds(0.5f);
            yield return null;
            ModManager.Instance.InitMods();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            CursorManager.Instance.InitCursorTextures();
            CursorManager.Instance.SetCursorStyle(0);
#endif
            // 初始化音效管理器
            AudioManager.Instance.Init();
            SkillVisualizer.Init();

            // 初始化外交管理器
            DiplomacyManager.Instance.Init();
            DiplomacyEventManager.Instance.Init();
            
            GameData.Instance.Init();
            while (true)
            {
                bool all_ready = true;
                for (int i = 0; i < ShortScenario.all_scenario_info_list.Count; i++)
                {
                    if (!ShortScenario.all_scenario_info_list[i].loadOK)
                    {
                        all_ready = false;
                        yield return null;
                    }
                }
                if (all_ready) break;
            }

            GameEvent.OnGameInit?.Invoke();
            GameState.Instance.ChangeState((int)GameState.State.GAME_START_MENU);
            Window.Instance.Open("window_start");
            Window.Instance.Close("window_loading");
            inited = true;
            //Scenario scenario = new Scenario();
            //string path = Path.FindFile("Data/Scenario/Scenario.json");
            //scenario.FilePath = path;
            //scenario.CommonData = GameData.Instance.LoadCommonData();
            ////EnterMapEdior();
            //Scenario.Start(scenario);
            ////scenario.Save(Path.ContentRootPath + "/Save/Scenario.xml");
            //Player.Player.Instance.Init();
        }

        public void EnterMapEditor()
        {
            Window.Instance.Close("window_start");
            GameObject map = new GameObject("map");
            MapEditor mapEditor = map.AddComponent<MapEditor>();
        }

        public void StartNewGame()
        {
            Window.Instance.Open("window_scenario_select");
            Window.Instance.Close("window_start");
        }

        public void StartGame(Scenario target)
        {

        }

        public override void Update()
        {
            if(!inited) return;
            GameController.Instance.Update();
            base.Update();
            // 更新音效管理器
            AudioManager.Instance.Update();
            Scenario scenario = Scenario.Cur;
            if (scenario != null)
            {
                GameEvent.OnScenarioTick?.Invoke(scenario, Time.deltaTime);
                if (!Scenario.Cur.useThreadRun)
                    Scenario.Cur.Run();
            }
        }

        public static void DebugAI()
        {
            GameAIDebug.Enabled = !GameAIDebug.Enabled;
        }

    }
}
