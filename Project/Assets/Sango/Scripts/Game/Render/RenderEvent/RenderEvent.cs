using Sango.Core;
using System.Collections.Generic;

namespace Sango.Render
{
    public class RenderEvent : Singleton<RenderEvent>
    {
        List<IRenderEventBase> eventQueue = new List<IRenderEventBase>();
        IRenderEventBase CurEvent { get; set; }
        Dictionary<string, Stack<IRenderEventBase>> eventPool = new Dictionary<string, Stack<IRenderEventBase>>();

        public T Create<T>() where T : IRenderEventBase, new()
        {
            string key = typeof(T).FullName;

            Stack<IRenderEventBase> stack;
            if (!eventPool.TryGetValue(key, out stack))
            {
                stack = new Stack<IRenderEventBase>();
                eventPool[key] = stack;
            }

            if (stack.Count > 0)
            {
                return (T)stack.Pop();
            }
            else
            {
                return new T();
            }
        }

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
                ReturnToPool(CurEvent);
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

        void ReturnToPool(IRenderEventBase renderEvent)
        {
            string key = renderEvent.GetType().FullName;
            Stack<IRenderEventBase> stack;
            if (!eventPool.TryGetValue(key, out stack))
            {
                stack = new Stack<IRenderEventBase>();
                eventPool[key] = stack;
            }
            stack.Push(renderEvent);
        }
    }
}
