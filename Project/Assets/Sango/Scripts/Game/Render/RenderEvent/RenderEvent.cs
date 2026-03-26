using System.Collections.Generic;

namespace Sango.Game.Render
{
    public class RenderEvent : Singleton<RenderEvent>
    {
        List<IRenderEventBase> eventQueue = new List<IRenderEventBase>();
        IRenderEventBase CurEvent { get; set; }
        public void Add(IRenderEventBase renderEvent)
        {
            if (renderEvent.IsStack)
                eventQueue.Insert(0, renderEvent);
            else
                eventQueue.Add(renderEvent);
        }

        public bool Update(Scenario scenario, float deltaTime)
        {
            while (CurEvent != null)
            {
                if (!CurEvent.Update(scenario, deltaTime))
                    return false;

                CurEvent.Exit(scenario);
                eventQueue.Remove(CurEvent);
                CurEvent = null;

                if (eventQueue.Count > 0)
                {
                    CurEvent = eventQueue[0];
                    CurEvent.Enter(scenario);
                }
            }

            if (eventQueue.Count > 0)
            {
                CurEvent = eventQueue[0];
                CurEvent.Enter(scenario);
                return false;
            }

            return true;
        }
    }
}
