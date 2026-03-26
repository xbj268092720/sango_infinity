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
            startPosition = Camera.main.transform.position;
            elapsedTime = 0;
        }

        public override bool Update(Scenario scenario, float deltaTime)
        {
            elapsedTime += deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            IsDone = t >= 1.0f;
            return IsDone;
        }
    }
}