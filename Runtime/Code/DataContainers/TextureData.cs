using UnityEngine;

namespace EasyMaskTool
{
    [CreateAssetMenu(fileName = "TextureData")]
    public class TextureData : ScriptableObject
    {
        public RenderTexture currentTexture;
        public Vector2Int textureOutputResolution = new (512, 512);
        public Vector2Int textureEditorResolution = new (512, 512);
        private Vector2Int rangeSize = new (4, 4096);
        public ComputeShader computePaint;
        public Material debugMaterial;
        public ETextureOutputType textureOutputType = ETextureOutputType.TGA;
        public ETextureChannel textureChannel = ETextureChannel.RGB;
        public EAxisSymmetry symmetry = EAxisSymmetry.None;
        public Vector3 symmetryPoint;
        public bool hasSymmetryPoint = false;

        private void OnValidate()
        {
            textureOutputResolution = ClampTextureResolution(textureOutputResolution);
        }

        private Vector2Int ClampTextureResolution(Vector2Int resolution)
        {
            int clampedWidth = Mathf.Clamp(resolution.x, rangeSize.x, rangeSize.y);
            int clampedHeight = Mathf.Clamp(resolution.y, rangeSize.x, rangeSize.y);

            return new Vector2Int(clampedWidth, clampedHeight);
        }
        public enum ETextureOutputType
        {
            PNG,
            TGA
        }
        public enum ETextureChannel
        {
            RGB,
            R,
            G,
            B
        }
        public enum EAxisSymmetry
        {
            None,
            X,
            Y
        }
    }
}
