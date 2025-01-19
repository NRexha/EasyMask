using UnityEngine;
using System.IO;
using UnityEditor;

namespace EasyMaskTool
{
    public static class TextureExporter
    {
        public static void SaveTexture(TextureData textureData)
        {
            if (textureData.currentTexture == null)
            {
                Debug.LogError("EasyMask: There is no texture to export");
                return;
            }

            Vector2Int targetResolution = textureData.textureOutputResolution;
            if (targetResolution.x <= 0 || targetResolution.y <= 0)
            {
                Debug.LogError("EasyMask: Invalid resolution specified in TextureData");
                return;
            }

            Texture2D outputTexture = textureData.currentTexture.ToTexture2D(TextureFormat.ARGB32);

            if (outputTexture.width != targetResolution.x || outputTexture.height != targetResolution.y)
                outputTexture = ResizeTexture(outputTexture, targetResolution.x, targetResolution.y);
            

            byte[] fileData;
            switch (textureData.textureOutputType)
            {
                case TextureData.ETextureOutputType.PNG:
                    fileData = outputTexture.EncodeToPNG();
                    break;

                case TextureData.ETextureOutputType.TGA:
                    fileData = outputTexture.EncodeToTGA();
                    break;

                default:
                    Debug.LogError("EasyMask: Texture output format not supported");
                    return;
            }

            if (fileData == null) return;

            string filePath = EditorUtility.SaveFilePanel(
                "Save Texture",
                "",
                "Texture",
                textureData.textureOutputType.ToString().ToLower()
            );

            if (string.IsNullOrEmpty(filePath)) return;

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            

            try
            {
                File.WriteAllBytes(filePath, fileData);
                string relativePath = "Assets" + filePath.Substring(Application.dataPath.Length);
                AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);

                TextureImporter textureImporter = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                if (textureImporter != null)
                {
                    textureImporter.sRGBTexture = false;
                    textureImporter.maxTextureSize = 8192;
                    AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
                }

                Debug.Log($"EasyMask: Texture successfully saved to {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"EasyMask: Failed to save texture: {e.Message}");
            }
        }
        private static Texture2D ResizeTexture(Texture2D originalTexture, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;

            Graphics.Blit(originalTexture, rt);

            Texture2D resizedTexture = new Texture2D(width, height, originalTexture.format, false);
            resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            resizedTexture.Apply();

            RenderTexture.active = null;
            rt.Release();

            return resizedTexture;
        }
    }
}
