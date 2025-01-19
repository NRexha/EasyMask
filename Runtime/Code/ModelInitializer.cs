using System.Collections.Generic;
using UnityEngine;

namespace EasyMaskTool
{
    public static class ModelInitializer
    {
        private static Shader s_debugShader;
        private static Material s_debugMaterial;
        private static readonly string s_debugShaderName = "Hidden/EasyMaskDebug";
        private static Dictionary<GameObject, Material> s_originalMaterials = new();
        private static Dictionary<Collider, bool> s_previousColliderStates = new();

        #region Colliders
        public static void ProvideColliders(Transform parentTransform, SelectionData selectionData)
        {
            DisableAllOtherColliders(parentTransform);

            if (!parentTransform.TryGetComponent(out MeshCollider parentCollider))
            {
                parentCollider = parentTransform.gameObject.AddComponent<MeshCollider>();
                selectionData.addedColliders.Add(parentCollider);
            }
            AddMeshColliderToChildren(parentTransform, selectionData);
        }

        private static void AddMeshColliderToChildren(Transform parentTransform, SelectionData selectionData)
        {
            foreach (Transform child in parentTransform)
            {
                if (!child.TryGetComponent(out MeshCollider childCollider))
                {
                    childCollider = child.gameObject.AddComponent<MeshCollider>();
                    selectionData.addedColliders.Add(childCollider);
                }
                AddMeshColliderToChildren(child, selectionData);
            }
        }

        public static void RemoveAllAddedColliders(SelectionData selectionData)
        {
            foreach (var collider in selectionData.addedColliders)
            {
                if (collider != null)
                    Object.DestroyImmediate(collider);
            }
            selectionData.addedColliders.Clear();

            RestorePreviousColliderStates();
        }

        private static void DisableAllOtherColliders(Transform rootTransform)
        {
            foreach (Collider collider in rootTransform.GetComponentsInChildren<Collider>(true))
            {
                if (!(collider is MeshCollider))
                {
                    if (!s_previousColliderStates.ContainsKey(collider))
                    {
                        s_previousColliderStates[collider] = collider.enabled;
                    }
                    collider.enabled = false;
                }
            }
        }

        private static void RestorePreviousColliderStates()
        {
            foreach (var pair in s_previousColliderStates)
            {
                if (pair.Key != null)
                {
                    pair.Key.enabled = pair.Value;
                }
            }
            s_previousColliderStates.Clear();
        }
        #endregion

        #region Materials

        public static void InitializeDebugMaterial(TextureData textureData)
        {
            s_debugShader = Shader.Find(s_debugShaderName);
            s_debugMaterial = new Material(s_debugShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            textureData.debugMaterial = s_debugMaterial;

            if (textureData.currentTexture == null) return;

            s_debugMaterial.mainTexture = textureData.currentTexture;
        }

        public static void StoreOriginalMaterials(IEnumerable<GameObject> selectedObjects)
        {
            foreach (var obj in selectedObjects)
            {
                StoreOriginalMaterialInHierarchy(obj);
            }
        }

        private static void StoreOriginalMaterialInHierarchy(GameObject obj)
        {
            if (obj == null) return;

            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null && !s_originalMaterials.ContainsKey(obj))
            {
                s_originalMaterials[obj] = renderer.sharedMaterial;
            }

            foreach (Transform child in obj.transform)
            {
                StoreOriginalMaterialInHierarchy(child.gameObject);
            }
        }

        public static void AssignDebugMaterial(IEnumerable<GameObject> selectedObjects)
        {
            foreach (var obj in selectedObjects)
            {
                ApplyMaterialToHierarchy(obj, s_debugMaterial);
            }
        }

        private static void ApplyMaterialToHierarchy(GameObject obj, Material material)
        {
            if (obj == null) return;

            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            foreach (Transform child in obj.transform)
            {
                ApplyMaterialToHierarchy(child.gameObject, material);
            }
        }

        public static void RevertToOriginalMaterials()
        {
            foreach (var pair in s_originalMaterials)
            {
                RevertMaterialInHierarchy(pair.Key, pair.Value);
            }
            s_originalMaterials.Clear();
        }

        private static void RevertMaterialInHierarchy(GameObject obj, Material originalMaterial)
        {
            if (obj == null) return;

            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = originalMaterial;
            }

            foreach (Transform child in obj.transform)
            {
                RevertMaterialInHierarchy(child.gameObject, originalMaterial);
            }
        }
        #endregion
    }
}
