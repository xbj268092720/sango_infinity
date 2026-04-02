using Sango.Core;

namespace Sango.Render
{
    public class DialogEvent : RenderEventBase
    {
        public GameDialog.DialogStyle dialogStyle;
        public string content;
        public Person person;
        public System.Action sureAction;
        public System.Action cancelAction;
        GameDialog.IDialog dialog;

        public void Init(GameDialog.DialogStyle dialogStyle, string content, Person person, System.Action sureAction, System.Action cancelAction)
        {
            this.dialogStyle = dialogStyle;
            this.content = content;
            this.person = person;
            this.sureAction = sureAction;
            this.cancelAction = cancelAction;
            IsDone = false;
        }
        public override void Enter(Scenario scenario)
        {
            dialog = GameDialog.Open(dialogStyle, content, () =>
            {
                sureAction?.Invoke();
                GameDialog.Close();
                IsDone = true;
            });
            if (person != null)
                dialog.SetPerson(person);
            dialog.SetCancelAction(() =>
            {
                GameDialog.Close();
                IsDone = true;
                cancelAction?.Invoke();
            });
        }
    }
}
