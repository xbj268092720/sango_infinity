namespace Sango.Core.Player
{
    [GameSystem]
    public class GameLoad : GameSettingMenuBase
    {
        public GameLoad()
        {
            customMenuName = "加载";
            customMenuOrder = 60;
            windowName = "window_scenario_save_in_game";
        }

        public override void Init()
        {
            base.Init();
            GameEvent.OnRightMouseButtonContextMenuShow += OnGameSettingContextMenuShow;
        }

        public override void Clear()
        {
            base.Clear();
            GameEvent.OnRightMouseButtonContextMenuShow -= OnGameSettingContextMenuShow;
        }

        public override void OnEnter()
        {
            Window.WindowInterface windowInterface = Window.Instance.Open(windowName, 1);
        }

        public override void OnDestroy()
        {
            Window.Instance.Close(windowName);
        }

    }
}
