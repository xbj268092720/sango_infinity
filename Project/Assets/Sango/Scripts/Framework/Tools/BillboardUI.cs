using UnityEngine;
using System.Collections;
using Sango.Render;
using Sango.Core;

namespace Sango
{
    public class BillboardUI : MonoBehaviour
    {
        public Vector3 initScale = new Vector3(-1f, 1f, 1f);
        public Vector2 scaleFactor = Vector2.one;
        public Vector3 offsetFactor = new Vector3(0f, 0f, 0f);
        public Vector3 cacheOffset = new Vector3(0f, 0f, 0f);
        private Transform cacheTrans;
        float tempFactor;

        private void Start()
        {
            CatchMainCamera();
            tempFactor = -1;
        }

        private void OnDisable()
        {
            tempFactor = -1;
        }
        private void OnEnable()
        {
            tempFactor = -1;
        }
        void CatchMainCamera()
        {
            if (cacheTrans == null)
                cacheTrans = Camera.main.transform;
        }

        public void Update()
        {
            CatchMainCamera();
            transform.LookAt(transform.position + cacheTrans.rotation * Vector3.back, cacheTrans.rotation * Vector3.up);
            float factor = MapRender.Instance.mapCamera.cameraDistanceFactor;
            if (factor != tempFactor)
            {
                tempFactor = factor;
                transform.localScale = Vector3.Lerp(initScale * scaleFactor.x, initScale * scaleFactor.y, factor);
                transform.localPosition = cacheOffset + Vector3.Lerp(offsetFactor, Vector3.zero, factor);
            }
        }
    }
}
