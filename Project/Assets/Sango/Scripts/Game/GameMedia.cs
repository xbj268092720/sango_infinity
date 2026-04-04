using Sango.Manager;

namespace Sango.Core
{
    public class GameMedia : Singleton<GameMedia>
    {
        public string buttonSfx = "Assets/Sound/button.mp3";
        public string menuClickSfx = "Assets/Sound/btn2.mp3";
        public string subMenuClickSfx = "Assets/Sound/btn3.mp3";
        public string cancelSfx = "Assets/Sound/cancel.mp3";
        public string doactionSfx = "Assets/Sound/doaction.mp3";
        public string newTrunSfx = "Assets/Sound/new_turn.mp3";

        public int PlayButtonSfx()
        {
            return AudioManager.Instance.PlaySfx(buttonSfx);
        }

        public int PlayCancelSfx()
        { 
            return AudioManager.Instance.PlaySfx(cancelSfx);
        }

        public int PlayDoAcitonSfx()
        {
            return AudioManager.Instance.PlaySfx(doactionSfx);
        }

        public int PlayMenuClickSfx()
        {
            return AudioManager.Instance.PlaySfx(menuClickSfx);
        }
        public int PlaySubMenuClickSfx()
        {
            return AudioManager.Instance.PlaySfx(subMenuClickSfx);
        }

        public int PlayNewTurnSfx()
        {
            return AudioManager.Instance.PlaySfx(newTrunSfx);
        }
    }
}
