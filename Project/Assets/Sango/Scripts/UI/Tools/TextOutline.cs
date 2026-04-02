using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[AddComponentMenu("LFramework/UI/Effect/TextOutline", 0)]
public class TextOutline : BaseMeshEffect
{
    protected TextOutline()
    {

    }

    //描边颜色
    public Color m_OutlineColor = new Color(0.12f, 0.12f, 0.12f);
    //描边宽度
    public float m_OutlineWidth = 1;

    public bool m_gray = false;

    private static List<UIVertex> m_VetexList = new List<UIVertex>();

    private Text text;
    private bool needUpdateMat = false;

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void ResetMat()
    {
        Text t = GetComponent<Text>();
        t.material = null;
    }

    protected override void Awake()
    {
        base.Awake();
        text = GetComponent<Text>();
        if(text.enabled == true ) { OnFreshCavasCommon(); }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Font.textureRebuilt += OnFontTextureRebuilt;
    }

    protected override void OnDisable()
    {
        Font.textureRebuilt -= OnFontTextureRebuilt;
        base.OnDisable();
    }

    private void OnFontTextureRebuilt(Font font)
    {
        if (text.enabled == true)
        {
            needUpdateMat = true;
            StopAllCoroutines();
            StartCoroutine(DoUpdateMat());
        }
        //#if UNITY_EDITOR
        //        Debug.Log("OnFontTextureRebuilt");
        //#endif
        //if (text != null)
        //{
        //    text.material = text.material;
        //}
        //Refresh();
    }

    IEnumerator DoUpdateMat()
    {
        yield return new WaitForSeconds(0.03f);
        if (needUpdateMat)
        {
            needUpdateMat = false;
            text.enabled = false;
            text.enabled = true;
        }
    }

    private void OnFreshCavasCommon()
    {
        if (graphic)
        {
            if (graphic.canvas)
            {
                var v1 = graphic.canvas.additionalShaderChannels;
                var v2 = AdditionalCanvasShaderChannels.TexCoord1;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
                v2 = AdditionalCanvasShaderChannels.TexCoord2;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
                v2 = AdditionalCanvasShaderChannels.TexCoord3;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
                v2 = AdditionalCanvasShaderChannels.Tangent;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
                v2 = AdditionalCanvasShaderChannels.Normal;
                if ((v1 & v2) != v2)
                {
                    graphic.canvas.additionalShaderChannels |= v2;
                }
            }
            Refresh();
        }
    }

    public void OnRefreshShaderChannels()
    {
        OnFreshCavasCommon();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        if (graphic.material.name != "OutlineMat")
        {
            Material m = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Sango/Resources/OutlineMat.mat");
            Text t = GetComponent<Text>();
            t.material = m;
        }
        Refresh();
        //if (graphic.material != null)
        //{
        //    if (graphic.material.shader.name != "UI/TextOutline")
        //    {
        //        Shader s = Shader.Find("UI/TextOutline");

        //        //Debug.LogError("没有找到材质TextOutline.mat");

        //    }
        //    Refresh();
        //}
    }
#endif

    private void Refresh()
    {
        graphic.SetVerticesDirty();
    }



    private string path()
    {
        var p = transform;
        var ret = "";
        while (p != null)
        {

            ret = p.name + "/" + ret;
            p = p.parent;
        }
        return ret;
    }

    public void SetGray(bool gray)
    {
        if (m_gray != gray)
        {
            m_gray = gray;
            Refresh();
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(m_VetexList);

        ProcessVertices();

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_VetexList);
    }

    private void ProcessVertices()
    {
        for (int i = 0, count = m_VetexList.Count - 3; i <= count; i += 3)
        {
            var v1 = m_VetexList[i];
            var v2 = m_VetexList[i + 1];
            var v3 = m_VetexList[i + 2];

            //计算原顶点坐标中心点
            var minX = Min(v1.position.x, v2.position.x, v3.position.x);
            var minY = Min(v1.position.y, v2.position.y, v3.position.y);
            var maxX = Max(v1.position.x, v2.position.x, v3.position.x);
            var maxY = Max(v1.position.y, v2.position.y, v3.position.y);
            var posCenter = new Vector2(minX + maxX, minY + maxY) * 0.5f;

            //计算原始顶点坐标和UV的方向
            Vector2 triX, triY, uvX, uvY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;
            if (Mathf.Abs(Vector2.Dot((pos2 - pos1).normalized, Vector2.right))
                > Mathf.Abs(Vector2.Dot((pos3 - pos2).normalized, Vector2.right)))
            {
                triX = pos2 - pos1;
                triY = pos3 - pos2;
                uvX = v2.uv0 - v1.uv0;
                uvY = v3.uv0 - v2.uv0;
            }
            else
            {
                triX = pos3 - pos2;
                triY = pos2 - pos1;
                uvX = v3.uv0 - v2.uv0;
                uvY = v2.uv0 - v1.uv0;
            }

            //计算原始uv框
            var uvMin = Min(v1.uv0, v2.uv0, v3.uv0);
            var uvMax = Max(v1.uv0, v2.uv0, v3.uv0);

            //OutlineColor和OutlineWidth也传入,避免出现不一样的材质球
            var col_rg = new Vector2(m_OutlineColor.r, m_OutlineColor.g);//描边颜色用uv3和tangent的zw传递
            var col_ba = new Vector4(0, 0, m_OutlineColor.b, m_OutlineColor.a);
            var normal = new Vector3(m_gray ? 1 : 0, 0, m_OutlineWidth);//描边的宽度用normal的z传递, x传递灰度信息

            //为每一个顶点设置新的Position和UV，并传入原始UV框
            v1 = SetNewPosAndUV(v1, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v1.uv3 = col_rg;
            v1.tangent = col_ba;
            v1.normal = normal;
            v2 = SetNewPosAndUV(v2, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v2.uv3 = col_rg;
            v2.tangent = col_ba;
            v2.normal = normal;
            v3 = SetNewPosAndUV(v3, m_OutlineWidth, posCenter, triX, triY, uvX, uvY, uvMin, uvMax);
            v3.uv3 = col_rg;
            v3.tangent = col_ba;
            v3.normal = normal;

            //应用设置后的UIVertex
            m_VetexList[i] = v1;
            m_VetexList[i + 1] = v2;
            m_VetexList[i + 2] = v3;
        }
    }

    private static UIVertex SetNewPosAndUV(UIVertex pVertex, float pOutLineWidth,
        Vector2 pPosCenter,
        Vector2 pTriangleX, Vector2 pTriangleY,
        Vector2 pUVX, Vector2 pUVY,
        Vector2 pUVOriginMin, Vector2 pUVOriginMax)
    {
        //Position
        var pos = pVertex.position;
        var posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        var posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pos.y += posYOffset;
        pVertex.position = pos;
        //UV
        var uv = pVertex.uv0;



        Vector2 v1 = pUVX / pTriangleX.magnitude * posXOffset * (Vector2.Dot(pTriangleX, Vector2.right) > 0 ? 1 : -1);
        uv.x += v1.x;
        uv.y += v1.y;
        Vector2 v2 = pUVY / pTriangleY.magnitude * posYOffset * (Vector2.Dot(pTriangleY, Vector2.up) > 0 ? 1 : -1); ;
        uv.x += v2.x;
        uv.y += v2.y;

        pVertex.uv0 = uv;

        pVertex.uv1 = pUVOriginMin;
        pVertex.uv2 = pUVOriginMax;

        return pVertex;
    }

    private static float Min(float pA, float pB, float pC)
    {
        return Mathf.Min(Mathf.Min(pA, pB), pC);
    }

    private static float Max(float pA, float pB, float pC)
    {
        return Mathf.Max(Mathf.Max(pA, pB), pC);
    }

    private static Vector2 Min(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Min(pA.x, pB.x, pC.x), Min(pA.y, pB.y, pC.y));
    }

    private static Vector2 Max(Vector2 pA, Vector2 pB, Vector2 pC)
    {
        return new Vector2(Max(pA.x, pB.x, pC.x), Max(pA.y, pB.y, pC.y));
    }

}
