using UnityEngine;
using UnityEngine.UI;

namespace MainMenuUI
{
    [AddComponentMenu("UI/Effects/UI Gradient Vertical")]
    public class UIGradientVertical : BaseMeshEffect
    {
        public Color topColor = Color.white;
        public Color bottomColor = new Color(0.72f, 0.72f, 0.78f, 1f);

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            int count = vh.currentVertCount;
            UIVertex vertex = default;

            float minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                minY = Mathf.Min(minY, vertex.position.y);
                maxY = Mathf.Max(maxY, vertex.position.y);
            }

            float range = Mathf.Max(0.0001f, maxY - minY);
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                float t = (vertex.position.y - minY) / range;
                Color tint = Color.Lerp(bottomColor, topColor, t);
                vertex.color *= tint;
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}
