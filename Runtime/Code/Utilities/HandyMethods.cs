using System.Linq.Expressions;
using System;
using UnityEngine;
using UnityEditor;

namespace EasyMaskTool
{
    public static class HandyMethods
    {
        public static string GetName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("!invalid input!", nameof(propertyExpression));
        }

        public static Texture2D ToTexture2D(this RenderTexture rTex, TextureFormat format = TextureFormat.RFloat)
        {
            RenderTexture currentActiveRT = RenderTexture.active;

            RenderTexture.active = rTex;

            Texture2D texture = new(rTex.width, rTex.height, format, false);

            texture.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            texture.Apply();

            RenderTexture.active = currentActiveRT;

            return texture;
        }

        public static float KeepSize(this float size)
        {
            Camera sceneCamera = SceneView.lastActiveSceneView?.camera;
            if (sceneCamera == null)
                return size;

            Transform camTransform = sceneCamera.transform;
            Vector3 point = SceneView.lastActiveSceneView.pivot;
            float distance = Vector3.Distance(camTransform.position, point);

            return size * distance;
        }
    }
}