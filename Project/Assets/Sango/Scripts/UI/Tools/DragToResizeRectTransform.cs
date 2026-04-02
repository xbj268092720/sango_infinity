
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DragToResizeRectTransform : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("拖拽设置")]
    [SerializeField] private float edgeThreshold = 10f; // 边缘检测阈值
    [SerializeField] private float minHeight = 50f;     // 最小高度
    [SerializeField] private float maxHeight = 1000f;   // 最大高度

    private RectTransform rectTransform;
    private bool isDraggingTopEdge = false;
    private bool isDraggingBottomEdge = false;
    private Vector2 lastMousePosition;
    private float originalHeight;
    private Vector2 originalPosition;
    float scaleFactor = 1;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        CanvasScaler canvasScaler = GetComponentInParent<CanvasScaler>();
        if (canvasScaler != null)
        {
            scaleFactor = canvasScaler.referenceResolution.y / Screen.height;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 将屏幕坐标转换为本地坐标
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        // 检查是否点击在上边缘
        if (localPoint.y >= rectTransform.rect.height - edgeThreshold)
        {
            isDraggingTopEdge = true;
        }
        // 检查是否点击在下边缘
        else if (localPoint.y <= edgeThreshold)
        {
            isDraggingBottomEdge = true;
        }

        // 记录初始状态
        if (isDraggingTopEdge || isDraggingBottomEdge)
        {
            originalHeight = rectTransform.rect.height;
            originalPosition = rectTransform.anchoredPosition;
            lastMousePosition = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingTopEdge && !isDraggingBottomEdge) return;

        // 计算鼠标位置变化
        Vector2 delta = eventData.position - lastMousePosition;
        lastMousePosition = eventData.position;

        // 根据拖拽边缘调整高度和位置
        if (isDraggingTopEdge)
        {
            // 向上拖拽增加高度，向下拖拽减少高度
            float newHeight = originalHeight + delta.y * scaleFactor;
            newHeight = Mathf.Clamp(newHeight, minHeight, maxHeight);

            // 调整位置以保持底部固定
            float heightDiff = newHeight - rectTransform.rect.height;
            Vector2 newPosition = rectTransform.anchoredPosition;
            newPosition.y += heightDiff / 2 * scaleFactor;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            rectTransform.anchoredPosition = newPosition;
            originalHeight = newHeight;
        }
        else if (isDraggingBottomEdge)
        {
            // 向下拖拽增加高度，向上拖拽减少高度
            float newHeight = originalHeight - delta.y * scaleFactor;
            newHeight = Mathf.Clamp(newHeight, minHeight, maxHeight);

            // 调整位置以保持顶部固定
            float heightDiff = newHeight - rectTransform.rect.height;
            Vector2 newPosition = rectTransform.anchoredPosition;
            newPosition.y -= heightDiff / 2;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
            originalHeight = newHeight;
            rectTransform.anchoredPosition = newPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 重置拖拽状态
        isDraggingTopEdge = false;
        isDraggingBottomEdge = false;
    }
}
