using Sango.Game.Render.UI;

namespace Sango.Game.Render
{
    public class DialogEvent : RenderEventBase
    {
        public override bool IsStack => true;
        public UIDialog.DialogStyle dialogStyle;
        public string content;
        public Person person;
        public System.Action sureAction;
        public System.Action cancelAction;
        UIDialog dialog;

        public void Init(UIDialog.DialogStyle dialogStyle, string content, Person person, System.Action sureAction, System.Action cancelAction)
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
            dialog = UIDialog.Open(dialogStyle, content, () =>
            {
                sureAction?.Invoke();
                UIDialog.Close();
                IsDone = true;
            });
            if (person != null)
                dialog.SetPerson(person);
            dialog.cancelAction = () =>
            {
                UIDialog.Close();
                IsDone = true;
                cancelAction?.Invoke();
            };
        }
    }
}
