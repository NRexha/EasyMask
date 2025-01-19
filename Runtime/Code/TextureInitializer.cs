using UnityEngine;

namespace EasyMaskTool
{
    public static class TextureInitializer
    {
        public static void InitializeTexture(TextureData textureData)
        {
            textureData.currentTexture = null;
            textureData.currentTexture = new RenderTexture(textureData.textureEditorResolution.x, textureData.textureEditorResolution.y, 0, RenderTextureFormat.ARGB32)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                enableRandomWrite = true,
                hideFlags = HideFlags.DontSave,
            };
            textureData.currentTexture.Create();
            Material blackMaterial = new(Shader.Find("Unlit/Color"))
            {
                color = new Color(0, 0, 0, 1)
            };

            Graphics.Blit(null, textureData.currentTexture, blackMaterial);
        }
    }
}
