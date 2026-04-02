namespace Sango.Core.Player
{
    public class TroopInteractiveBase : GameSystem
    {
        public Troop TargetTroop { get; set; }
        public Cell ActionCell { get; set; }

        public string content;
        public virtual bool IsValid { get; protected set; }

        public override void Init()
        {
            GameEvent.OnTroopInteractiveContextDialogShow += OnTroopInteractiveContextDialogShow;
        }

        public override void Clear()
        {
            GameEvent.OnTroopInteractiveContextDialogShow -= OnTroopInteractiveContextDialogShow;
        }

        protected virtual void OnTroopInteractiveContextDialogShow(ITroopInteractiveDialogData dialogData, Troop troop, Cell actionCell)
        {

            if (troop.BelongForce != null && troop.BelongForce.IsPlayer && troop.BelongForce == Scenario.Cur.CurRunForce)
            {
                TargetTroop = troop;
                ActionCell = actionCell;

                if (Check(troop, actionCell))
                {
                    dialogData.SetContent(content);
                    dialogData.SetSureAction(OnSure);
                }
            }
        }

        protected virtual bool Check(Troop troop, Cell actionCell)
        {
            return false;
        }

        protected virtual void OnSure()
        {
            Start(TargetTroop, ActionCell);
        }

        public virtual void Start(Troop troop, Cell actionCell)
        {
            TargetTroop = troop;
            ActionCell = actionCell;
            GameSystemManager.Instance.Push(this);
        }
    }
}
