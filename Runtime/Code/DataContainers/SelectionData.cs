using System.Collections.Generic;
using UnityEngine;

namespace EasyMaskTool
{
    [CreateAssetMenu(fileName = "SelectionData")]
    public class SelectionData : ScriptableObject
    {
        public List<GameObject> selectedObjects = new();
        public List<MeshCollider> addedColliders = new();

    }
}
