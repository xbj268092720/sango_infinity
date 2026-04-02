using Sango.Game.Player;

namespace Sango.Game.Render.UI
{
    public class UIMobileCancel : UGUIWindow
    {
        public void OnCancel()
        {
            GameController.Instance.OnCancel();
        }
    }
}
