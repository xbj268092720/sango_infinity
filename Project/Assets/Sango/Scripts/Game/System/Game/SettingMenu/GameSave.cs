namespace Sango.Core.Player
{
    [GameSystem]
    public class GameSave : GameSettingMenuBase
    {
        public GameSave() {
            customMenuName = "保存";
            customMenuOrder = 50;
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
            Window.WindowInterface windowInterface = Window.Instance.Open(windowName, 0);
        }

        public override void OnDestroy()
        {
            Window.Instance.Close(windowName);
        }
    }
}
