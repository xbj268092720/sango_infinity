using UnityEngine;
using UnityEngine.UI;
public class DrawStatusComponent : Graphic
{
    public Color32[] colors = new Color32[6];
    public float [] scaleLenth = new float[5];
    bool needUpdate = false;
    private void Update()
    {
        if(needUpdate)
        {
            UpdateGeometry();
            needUpdate = false;
        }
    }

    public void UpdateContent()
    {
        needUpdate = true;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (colors.Length < 6)
            colors = new Color32[6];

        vh.Clear();
        vh.AddVert(Vector3.zero, colors[0], Vector4.zero);
        Vector3 up = Vector3.up;
        float dig = 360f / 5f;
        vh.AddVert(up * scaleLenth[0], colors[1], Vector4.zero);

        for (int i = 1; i < 5; i++)
        {
            up = Quaternion.Euler(0, 0, i * dig) * Vector3.up;
            vh.AddVert(up * scaleLenth[5-i], colors[1+i], Vector4.zero);
        }
        vh.AddTriangle(0, 2, 1);
        vh.AddTriangle(0, 3, 2);
        vh.AddTriangle(0, 4, 3);
        vh.AddTriangle(0, 5, 4);
        vh.AddTriangle(0, 1, 5);
    }
}


