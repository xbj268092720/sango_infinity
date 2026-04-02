using Sango.Core;
using Sango.Core.Object.Arrow;
using UnityEngine;

namespace Sango.Render
{
    public abstract class ObjectRender : IRender
    {
        public delegate void OnModelVisibleChangeFunction(MapObject obj);

        public OnModelVisibleChangeFunction onModelVisibleChangeFunction;

        public bool IsVisible()
        {
            return MapObject != null && MapObject.visible;
        }

        public virtual SangoObject Owener { get; set; }
        public virtual MapObject MapObject { get; set; }

        public Transform GetTransform()
        {
            return MapObject.transform;
        }

        public virtual Vector3 GetPosition()
        {
            return MapObject.position;
        }
        public virtual void SetPosition(Vector3 pos)
        {
            MapObject.position = pos;
        }
        public virtual Vector3 GetForward()
        {
            return MapObject.forward;
        }
        public virtual void SetForward(Vector3 forward)
        {
            MapObject.forward = forward;
        }

        public virtual void CastArrow(Vector3 target)
        {
            BowmanUnitArrowManager arrowManager = MapObject.GetComponentInChildren<BowmanUnitArrowManager>(true);
            arrowManager?.FireArrows(target);
        }

        public virtual void Clear()
        {
            if (MapObject != null)
            {
                MapObject.Destroy();
                MapObject = null;
            }
        }

        public virtual void SetFlash(bool b)
        {
           
        }

        public virtual void UpdateRender() { }
        public virtual void ShowInfo(int damage, int damageType) {; }
        public virtual void ShowSkill(SkillInstance skill, bool isFail, bool isCritical) {; }

        public virtual void OnModelVisibleChange(MapObject obj) {

            onModelVisibleChangeFunction?.Invoke(obj);
        }
    }
}
