namespace Sango.Core
{
    /// <summary>
    /// 城池治安系统逻辑
    /// </summary>
    [GameSystem]
    public class PlayerChoice : GameSystem
    {
        public struct ChoiceData
        {
            public string lab;
            public System.Action call;
        }

        public ChoiceData[] choiceDatas;
        string windowName = "window_choice";

        public void Start(ChoiceData[] choices)
        {
            choiceDatas = choices;
            Push();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Window.Instance.Open(windowName, this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Window.Instance.Close(windowName);
        }

        public void OnPlayerChoose(int index)
        {
            if (index < 0 || index >= choiceDatas.Length)
                return;
            ChoiceData data = choiceDatas[index];
            data.call?.Invoke();
            Done();
        }
    }
}
