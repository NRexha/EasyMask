using UnityEditor;
using UnityEngine;

namespace EasyMaskTool
{
    public static class EasyMaskPainter
    {
        public static void InitializeComputeShader(TextureData textureData)
        {
            textureData.computePaint = Resources.Load<ComputeShader>("Shaders/EasyMaskPainter");
            textureData.computePaint.SetTexture(0, "ResultTexture", textureData.currentTexture);
        }
        public static void PaintOnSurface(TextureData textureData, BrushData brushData)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
                {
                    Vector2 uv = hit.textureCoord;

                    Vector4 hitUV = new Vector4(uv.x, uv.y, 0, 0);

                    if (textureData.symmetry != TextureData.EAxisSymmetry.None)
                    {
                        Vector3 mirroredMousePosition = GetMirroredMousePosition(Event.current.mousePosition, textureData);

                        Ray mirroredRay = HandleUtility.GUIPointToWorldRay(mirroredMousePosition);
                        if (Physics.Raycast(mirroredRay, out RaycastHit mirroredHit))
                        {
                            if (mirroredHit.collider is MeshCollider && mirroredHit.textureCoord != Vector2.zero)
                            {
                                Vector2 mirroredUV = mirroredHit.textureCoord;
                                hitUV.z = mirroredUV.x;
                                hitUV.w = mirroredUV.y;
                            }
                        }
                    }
                    PaintAtUV(textureData, brushData, hitUV);
                }
            }
        }

        private static void PaintAtUV(TextureData textureData, BrushData brushData, Vector4 hitUV)
        {
            int kernel = textureData.computePaint.FindKernel("CSMain");
            textureData.computePaint.SetTexture(kernel, "ResultTexture", textureData.currentTexture);
            textureData.computePaint.SetTexture(kernel, "BrushTexture", brushData.brushShapeContainer);
            textureData.computePaint.SetVector("HitUV", hitUV);
            textureData.computePaint.SetFloat("Radius", brushData.brushRadius * 0.01f);
            textureData.computePaint.SetVector("BrushColor", brushData.brushColor);
            textureData.computePaint.SetInt("TargetChannel", (int)textureData.textureChannel);
            textureData.computePaint.SetInt("SymmetryAxis", (int)textureData.symmetry);

            int threadGroupsX = textureData.currentTexture.width / 8;
            int threadGroupsY = textureData.currentTexture.height / 8;
            textureData.computePaint.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);
        }

        private static Vector3 GetMirroredMousePosition(Vector3 originalMousePosition, TextureData textureData)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView.camera;
            if (sceneCamera == null)
                return originalMousePosition;

            Ray ray = HandleUtility.GUIPointToWorldRay(originalMousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPosition = hit.point;

     
                Vector3 cameraRight = sceneCamera.transform.right;
                Vector3 cameraUp = sceneCamera.transform.up;

                switch (textureData.symmetry)
                {
                    case TextureData.EAxisSymmetry.X: 
                        hitPosition -= 2 * Vector3.Dot(hitPosition - textureData.symmetryPoint, cameraRight) * cameraRight;
                        break;
                    case TextureData.EAxisSymmetry.Y: 
                        hitPosition -= 2 * Vector3.Dot(hitPosition - textureData.symmetryPoint, cameraUp) * cameraUp;
                        break;
                }

                Vector3 mirroredMousePosition = HandleUtility.WorldToGUIPoint(hitPosition);
                return mirroredMousePosition;
            }

            return originalMousePosition;
        }



    }
}
