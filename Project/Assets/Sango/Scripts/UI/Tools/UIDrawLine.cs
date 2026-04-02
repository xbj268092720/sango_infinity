using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 画线 
/// https://github.com/monitor1394/unity-ugui-XCharts
/// </summary>
public class UIDrawLine
{
    private static UIVertex[] vertex = new UIVertex[4];
    public static void DrawLine(VertexHelper vh, Vector3 p1, Vector3 p2, float size, Color32 color)
    {
        if (p1 == p2) return;
        Vector3 v = Vector3.Cross(p2 - p1, Vector3.forward).normalized * size;
        vertex[0].position = p1 - v;
        vertex[1].position = p2 - v;
        vertex[2].position = p2 + v;
        vertex[3].position = p1 + v;

        for (int j = 0; j < 4; j++)
        {
            vertex[j].color = color;
            vertex[j].uv0 = Vector2.zero;
        }
        vh.AddUIVertexQuad(vertex);
    }



    public static void GetBezierList(ref List<Vector3> posList, Vector3 sp, Vector3 ep,
           Vector3 lsp, Vector3 nep, float smoothness = 2f, float k = 2.0f)
    {
        float dist = Mathf.Abs(sp.x - ep.x);
        Vector3 cp1, cp2;
        var dir = (ep - sp).normalized;
        var diff = dist / k;
        if (lsp == sp)
        {
            cp1 = sp + dist / k * dir * 1;
            cp1.y = sp.y;
            cp1 = sp;
        }
        else
        {
            cp1 = sp + (ep - lsp).normalized * diff;
        }
        if (nep == ep) cp2 = ep;
        else cp2 = ep - (nep - sp).normalized * diff;
        dist = Vector3.Distance(sp, ep);
        int segment = (int)(dist / (smoothness <= 0 ? 2f : smoothness));
        if (segment < 1) segment = (int)(dist / 0.5f);
        if (segment < 4) segment = 4;
        GetBezierList2(ref posList, sp, ep, segment, cp1, cp2);
    }
    public static void GetBezierList2(ref List<Vector3> posList, Vector3 sp, Vector3 ep, int segment, Vector3 cp,
          Vector3 cp2)
    {
        posList.Clear();
        if (posList.Capacity < segment + 1)
        {
            posList.Capacity = segment + 1;
        }
        for (int i = 0; i < segment; i++)
        {
            posList.Add((GetBezier2(i / (float)segment, sp, cp, cp2, ep)));
        }
        posList.Add(ep);
    }
    public static Vector3 GetBezier2(float t, Vector3 sp, Vector3 p1, Vector3 p2, Vector3 ep)
    {
        t = Mathf.Clamp01(t);
        var oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * sp +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * ep;
    }
}
