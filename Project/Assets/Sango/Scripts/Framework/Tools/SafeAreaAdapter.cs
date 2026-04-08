using UnityEngine;
using UnityEngine.UI;

public class SafeAreaAdapter : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        // 转换为标准化锚点坐标（0~1 范围）
        Vector2 anchorMin = new Vector2(
            safeArea.xMin / Screen.width,
            safeArea.yMin / Screen.height
        );
        Vector2 anchorMax = new Vector2(
            safeArea.xMax / Screen.width,
            safeArea.yMax / Screen.height
        );

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
    private ScreenOrientation lastOrientation;

    void Update()
    {
        if (Screen.orientation != lastOrientation)
        {
            ApplySafeArea();
            lastOrientation = Screen.orientation;
        }
    }
}
