using TKNewtonsoft.Json.Linq;
using Sango.Game.Render;
using System.Collections.Generic;

namespace Sango.Game
{
    /// <summary>
    /// 逃跑(伪报)
    /// </summary>
    public class Escape : BuffEffect
    {
        public override void Init(JObject p, BuffInstance master)
        {
            base.Init(p, master);
            GameEvent.OnTroopTurnStart += OnTroopTurnStart;
        }

        void OnTroopTurnStart(Troop troop, Scenario scenario)
        {
            if(troop == master.Target)
            {
                troop.ActionOver = true;
                troop.TryMoveToCity(troop.BelongCity);
                TroopEscapeToCityEvent @event = Render.RenderEvent.Instance.Create<TroopEscapeToCityEvent>();
                @event.Init(troop, troop.BelongCity, null);
                Render.RenderEvent.Instance.Add(@event);
            }
        }

        public override void Action(BuffInstance skillInstance, Troop troop, Cell spellCell, List<Cell> atkCellList)
        {
        }

        public override void Clear()
        {
            base.Clear();
            GameEvent.OnTroopTurnStart -= OnTroopTurnStart;

        }
    }
}
