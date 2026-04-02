using Sango.Game.Render;
using Sango.Game.Render.UI;

namespace Sango.Game.Player
{
    [GameSystem]
    public class PlayerTurnStartGreeting : GameSystem
    {
        public PlayerTurnStartGreeting()
        {

        }

        public override void OnEnter()
        {
            OnBack(null);
        }

        public override void OnBack(ICommandEvent whoGone)
        {
            Force force = Scenario.Cur.CurRunForce;
            //UIDialog dialog = UIDialog.Open(UIDialog.DialogStyle.ClickPersonSay, $"{force.ColorName}大人，\n终于轮到我们了啊。", null);
            Person person = force.Counsellor;
            if (person == null || person.BelongForce != force)
            {
                int max = force.Governor.BelongCity.allPersons.Count;
                person = force.Governor.BelongCity.allPersons.Get(GameRandom.Range(0, max));
            }
      
            PlayerMessage.AddPersonMessage($"下一步，我们该干些什么呢？。", person);
            DialogEvent te = RenderEvent.Instance.Create<DialogEvent>();
            te.Init(UIDialog.DialogStyle.ClickPersonSay, $"{force.ColorName}大人，\n终于轮到我们了啊。", person, null, () =>
            {
                Done();
            });
            RenderEvent.Instance.Add(te);

        }

        public override void OnDestroy()
        {
            
        }

        public override void HandleEvent(CommandEventType eventType, Cell cell, UnityEngine.Vector3 clickPosition, bool isOverUI)
        {
            switch (eventType)
            {
                case CommandEventType.Cancel:
                case CommandEventType.ClickDown:
                case CommandEventType.RClickDown:
                    GameSystemManager.Instance.Back(); break;
            }

        }
    }
}
