using Sango.Render;
using UnityEngine;

namespace Sango.Game.Render
{
    public class CameraMoveEvent : RenderEventBase
    {
        public Vector3 targetPosition;
        public float moveDuration = 1.0f;
        private float elapsedTime = 0;
        private Vector3 startPosition;

        public override void Enter(Scenario scenario)
        {
            base.Enter(scenario);
            startPosition = MapRender.Instance.GetCameraPos();
            elapsedTime = 0;
            MapRender.Instance.MoveCameraTo(targetPosition);
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            return IsDone;
        }
    }
}