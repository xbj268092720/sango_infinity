using Sango.Core;

namespace Sango.Render
{
    public interface IRenderEventBase
    {
        /// <summary>
        /// 是否栈式添加在最前面,否则顺序添加至末尾
        /// </summary>
        bool IsStack { get; set; }
        bool IsDone { get; set; }
        bool IsVisible();
        bool Update(Scenario scenario, float deltaTime);
        void Enter(Scenario scenario);
        void Exit(Scenario scenario);
    }
}
