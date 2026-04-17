using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    RectTransform tran;
    RectTransform parentTran;
    private Vector2 pointerOffset;

    public RectTransform dragRect;
    public Camera uiCamera;
    bool draging = false;
    public bool isOverlayCamera = false;

    private void Awake()
    {
        if (tran == null)
            tran = GetComponent<RectTransform>();
        if (parentTran == null)
            parentTran = tran.parent.GetComponent<RectTransform>();
    }

    void Start()
    {
        if (!isOverlayCamera && uiCamera == null)
            uiCamera = Sango.Core.Game.Instance.UICamera;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (dragRect != null && !RectTransformUtility.RectangleContainsScreenPoint(dragRect, eventData.position, uiCamera))
            return;
        draging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTran, eventData.position, uiCamera, out Vector2 localPoint);

        // 计算触摸点与拖动对象的偏移量
        pointerOffset = localPoint - (Vector2)tran.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draging)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTran, eventData.position, uiCamera, out Vector2 localPoint);

        // 更新拖动对象的位置
        tran.anchoredPosition = localPoint - pointerOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 重置偏移量
        pointerOffset = Vector2.zero;
        draging = false;
    }
}

