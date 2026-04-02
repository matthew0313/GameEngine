using UnityEngine;
using UnityEngine.UI;

public class GridGraphic : MaskableGraphic
{
    public Color gridColor = new Color(1f, 1f, 1f, 0.1f);
    public float lineWidth = 1f;
    public float spacing = 50.0f;
    public Vector2 offset;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect rect = rectTransform.rect;
        if (spacing < 4f) return;

        Vector2 offset = new(
            this.offset.x % spacing,
            this.offset.y % spacing
        );
        float half = lineWidth * 0.5f;

        float x = rect.center.x + offset.x;
        while (x > rect.xMin) x -= spacing;
        while (x <= rect.xMax)
        {
            AddQuad(vh, new Vector2(x - half, rect.yMin), new Vector2(x + half, rect.yMax));
            x += spacing;
        }

        float y = rect.center.y + offset.y;
        while (y > rect.yMin) y -= spacing;
        while (y <= rect.yMax)
        {
            AddQuad(vh, new Vector2(rect.xMin, y - half), new Vector2(rect.xMax, y + half));
            y += spacing;
        }
    }

    void AddQuad(VertexHelper vh, Vector2 min, Vector2 max)
    {
        int i = vh.currentVertCount;
        UIVertex v = UIVertex.simpleVert;
        v.color = gridColor;

        v.position = new Vector3(min.x, min.y);
        vh.AddVert(v);
        v.position = new Vector3(min.x, max.y);
        vh.AddVert(v);
        v.position = new Vector3(max.x, max.y);
        vh.AddVert(v);
        v.position = new Vector3(max.x, min.y);
        vh.AddVert(v);

        vh.AddTriangle(i, i + 1, i + 2);
        vh.AddTriangle(i, i + 2, i + 3);
    }
}
