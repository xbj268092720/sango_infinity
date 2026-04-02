using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LineType
{
    Straight,
    Smooth
}

public class DrawLineComponent : Graphic
{
    [Header("线的属性")]
    [SerializeField] private LineType lineType = LineType.Straight;
    [SerializeField] public Line line = new StraightLine();
    [SerializeField] protected float m_ChartWidth;
    [NonSerialized] private bool m_RefreshChart = false;
    public float chartWidth { get { return m_ChartWidth; } }
    public LineType LineType
    {
        get => lineType;
        set
        {
            lineType = value;
            if (value == LineType.Straight)
                line = new StraightLine();
            else
                line = new SmoothLine();
            m_RefreshChart = true;

        }
    }

    protected override void Awake()
    {
        base.Awake();
        m_ChartWidth = rectTransform.sizeDelta.x;
        if (LineType == LineType.Smooth)
            line = new SmoothLine();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        line.DrawLine(vh);
    }
    private void Update()
    {
        CheckRefreshChart();
    }

    public void RefreshChart()
    {
        m_RefreshChart = true;
    }

    protected void CheckRefreshChart()
    {
        if (m_RefreshChart)
        {
            int tempWid = (int)chartWidth;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid - 1);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempWid);
            m_RefreshChart = false;
        }
    }
    public void AddPoint(Vector3 v3)
    {
        line.AddPoint(v3);
        m_RefreshChart = true;
    }
    public void AddPoint(List<Vector3> points)
    {
        line.AddPoint(points);
        m_RefreshChart = true;

    }

    public void SetSize(float size)
    {
        line.size = size;
        m_RefreshChart = true;

    }
    public void SetColor(Color color)
    {
        line.lineColor = color;
        m_RefreshChart = true;

    }
    public void SetSmoothness(float smoothness)
    {
        if (LineType == LineType.Smooth)
            ((SmoothLine)line).smoothness = smoothness;

        m_RefreshChart = true;

    }
    public void SetLineSmoothStyle(float smoothStyle)
    {
        if (LineType == LineType.Smooth)
            ((SmoothLine)line).lineSmoothStyle = smoothStyle;
        m_RefreshChart = true;

    }
}

[System.Serializable]
public class Line
{
    [SerializeField]
    public List<Vector3> dataPoints = new List<Vector3>();
    [SerializeField] public float size = 1;
    [SerializeField] public Color lineColor = Color.black;
    public virtual void DrawLine(VertexHelper vh)
    {
    }
    public void AddPoint(Vector3 p)
    {
        dataPoints.Add(p);
    }
    public void AddPoint(List<Vector3> points)
    {
        dataPoints.AddRange(points);
    }
}
public class StraightLine : Line
{
    public override void DrawLine(VertexHelper vh)
    {
        for (int i = 0; i < dataPoints.Count; i++)
        {
            if (i < dataPoints.Count - 1)
            {
                UIDrawLine.DrawLine(vh, dataPoints[i], dataPoints[i + 1], size, lineColor);
            }
        }
    }
}
public class SmoothLine:Line
{
    /// <summary>
    //曲线平滑度。值越小曲线越平滑，但顶点数也会随之增加。
    /// </summary>
    [SerializeField] public float smoothness = 2;
    /// <summary>
    /// 曲线平滑系数。通过调整平滑系数可以改变曲线的曲率，得到外观稍微有变化的不同曲线。
    /// </summary>
    [SerializeField] public float lineSmoothStyle = 2;


    private List<Vector3> bezierPoints = new List<Vector3>();
    public override void DrawLine(VertexHelper vh)
    {
        Vector3 lp = Vector3.zero;
        Vector3 np = Vector3.zero;
        Vector3 llp = Vector3.zero;
        Vector3 nnp = Vector3.zero;

        for (int i = 0; i < dataPoints.Count; i++)
        {
            if (i < dataPoints.Count - 1)
            {
                llp = i > 1 ? dataPoints[i - 2] : lp;
                nnp = i < dataPoints.Count - 1 ? dataPoints[i + 1] : np;
                UIDrawLine.GetBezierList(ref bezierPoints, dataPoints[i], dataPoints[i + 1], llp, nnp, smoothness, lineSmoothStyle);
                for (int j = 0; j < bezierPoints.Count; j++)
                {
                    if (j < bezierPoints.Count - 1)
                    {
                        UIDrawLine.DrawLine(vh, bezierPoints[j], bezierPoints[j + 1], size, lineColor);
                    }
                }
            }
        }
    }


}


