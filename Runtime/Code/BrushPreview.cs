using UnityEngine;
using UnityEditor;

namespace EasyMaskTool
{
    public static class BrushPreview
    {
        private static Vector3 s_previousPosition = Vector3.zero;
        private static Vector3 s_previousNormal = Vector3.up;
        public static void DrawBrushPreview(BrushData brushData)
        {
            var brushPreview = GetBrushPreview();
            Vector3 position = brushPreview.Item1;
            Vector3 normal = brushPreview.Item2;

            Handles.color = Color.grey;
            Handles.DrawWireDisc(position + normal * 0.02f, normal, brushData.brushRadius.KeepSize() * 0.01f, 2.5f);
            Handles.color = Color.white;
            Handles.DrawWireDisc(position + normal * 0.02f, normal, brushData.brushRadius.KeepSize() * 0.01f * (1 - brushData.brushSmoothness));
        }

        private static (Vector3, Vector3) GetBrushPreview()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                s_previousPosition = Vector3.Lerp(s_previousPosition, hit.point, 0.1f);
                s_previousNormal = Vector3.Lerp(s_previousNormal, hit.normal, 0.1f);
                return (s_previousPosition, s_previousNormal);
            }
            return (Vector3.zero, Vector3.up);
        }
    }
}
