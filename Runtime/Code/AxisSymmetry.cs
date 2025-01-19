using UnityEditor;
using UnityEngine;

namespace EasyMaskTool
{
    public static class AxisSymmetry
    {
        public static void DrawSymmetryPoint(TextureData textureData)
        {
            if (textureData.hasSymmetryPoint)
            {
                Handles.color = Color.yellow;
                float size = 0.03f;
                Handles.SphereHandleCap(
                    0,
                    textureData.symmetryPoint,
                    Quaternion.identity,
                    size.KeepSize(),
                    EventType.Repaint
                );

                Vector3 newPoint = Handles.PositionHandle(textureData.symmetryPoint, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    textureData.symmetryPoint = newPoint;
                }
            }
        }

        public static void CenterSymmetryPoint(TextureData textureData)
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 0)
                return;

            Vector3 totalCenter = Vector3.zero;
            int objectCount = 0;

            foreach (var selectedObject in selectedObjects)
            {
                MeshRenderer meshRenderer = selectedObject.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                {
                    totalCenter += meshRenderer.bounds.center;
                    objectCount++;
                }
            }

            if (objectCount > 0)
            {
                textureData.symmetryPoint = totalCenter / objectCount;
                textureData.hasSymmetryPoint = true;
            }

        }
    }
}
