using System;
using UnityEngine;

namespace EasyMaskTool
{
    [CreateAssetMenu(fileName = "BrushData")]
    public class BrushData : ScriptableObject
    {
        public Texture2D brushShape;
        public CustomRenderTexture brushShapeContainer;
        public Color brushColor = Color.white;
        [Range(0.01f, 1f)] public float brushOpacity = 1f;
        [Range(0.1f, 1f)] public float brushSmoothness = 0.5f;
        [Range(0.1f,20f)] public float brushRadius = 4f;
        [Range(1,10)] public int brushRepetition = 1;
        public Material brushShapeMaterial;


        private void OnEnable()
        {
            brushShape = Resources.Load<Texture2D>("Brushes/EasyMaskBasicBrush");
            brushShapeMaterial = new Material(Shader.Find("Hidden/EasyMaskBrush"))
            {
                hideFlags = HideFlags.HideAndDontSave,
            };
            brushShapeContainer = Resources.Load<CustomRenderTexture>("BrushShapeContainer");
            brushShapeContainer.material = brushShapeMaterial;
        }
    }
}
