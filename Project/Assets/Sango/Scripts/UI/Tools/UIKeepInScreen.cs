using UnityEngine;

namespace UnityEngine.UI
{
    public class UIKeepInScreen : MonoBehaviour
    {
        public bool judgmentWidth = true;
        public bool judgmentHeight = true;
        public Vector2 offset;
        public float delayTime = 0;
        float cur_delayTime = 0;
        public Vector2 cachePosition;
        public bool dontResetCachePosition = true;
        RectTransform cacheTrans;
        RectTransform uiRoot;
        public void Start()
        {
            cur_delayTime = delayTime;
            cacheTrans = transform as RectTransform;
            if (cacheTrans != null)
            {
                cachePosition = cacheTrans.anchoredPosition;
                uiRoot = Sango.Game.Game.Instance.UIRoot;
            }

            if (!dontResetCachePosition)
                LateUpdate();
        }

        public void OnEnable()
        {
            if (cacheTrans != null )
            {
                if(!dontResetCachePosition)
                    cacheTrans.anchoredPosition = cachePosition;
            }

            Start();
        }

        public bool JudgmentUiInScreen(bool w, bool h)
        {
            if (!w && !h) return false;

            RectTransform rect = cacheTrans;
            rect.anchoredPosition -= offset;

            Vector2 pivotVec = rect.pivot;
            Vector3 pos = uiRoot.InverseTransformPoint(cacheTrans.position); //取得当前UI在屏幕当中的位置
            Vector3 moveDistance = new Vector3(0, 0, 0);
            //判断当前的位置 与屏幕坐标的关系
            bool isInScreen = true;
            if (w)
            {
                float leftX, rightX;
                leftX = pos.x - rect.sizeDelta.x * pivotVec.x * rect.localScale.x;
                rightX = pos.x + rect.sizeDelta.x * (1 - pivotVec.x) * rect.localScale.x;
                float ScreenWidthHalf = uiRoot.sizeDelta.x / 2;
                if (rightX > ScreenWidthHalf)
                {
                    isInScreen = false;
                    moveDistance.x = ScreenWidthHalf - rightX;
                }
                else if (leftX < -ScreenWidthHalf)
                {
                    isInScreen = false;
                    moveDistance.x = -ScreenWidthHalf - leftX;
                }
            }
            if (h)
            {
                float upY, downY;
                downY = pos.y - rect.sizeDelta.y * pivotVec.y * rect.localScale.y;
                upY = pos.y + rect.sizeDelta.y * (1 - pivotVec.y) * rect.localScale.y;
                float ScreenHeightHalf = uiRoot.sizeDelta.y / 2;
                if (upY > ScreenHeightHalf)
                {
                    isInScreen = false;
                    moveDistance.y = ScreenHeightHalf - upY;
                }
                else if (downY < -ScreenHeightHalf)
                {
                    isInScreen = false;
                    moveDistance.y = -ScreenHeightHalf - downY;
                }
            }
            moveDistance.x = moveDistance.x + pos.x;
            moveDistance.y = moveDistance.y + pos.y;
            //RectTransform parentRectTransform = rect.parent.GetComponent<RectTransform>();
            rect.transform.position = uiRoot.TransformPoint(moveDistance);
            rect.anchoredPosition += offset;
            //GameDebug.Log($"----------JudgmentUiInScreen ,[{isInScreen}],[{moveDistance}] ,[{ScreenWidthHalf}],[{ScreenHeightHalf}]");
            return isInScreen;
        }

        public void LateUpdate()
        {
            if (cacheTrans == null)
                return;

            if (cur_delayTime > 0)
            {
                cur_delayTime -= Time.deltaTime;
                return;
            }
            //cacheTrans.anchoredPosition = cachePosition;

            // 左
            JudgmentUiInScreen(judgmentWidth, judgmentHeight);
        }
    }
}
