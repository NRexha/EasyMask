using UnityEngine;

namespace EasyMaskTool
{
    public class DataManager
    {
        public TextureData TextureData { get; set; }
        public BrushData BrushData { get; set; }
        public SelectionData SelectionData { get; set; }

        public DataManager()
        {
            CreateDataContainers();
        }

        private void CreateDataContainers()
        {
            TextureData = ScriptableObject.CreateInstance<TextureData>();
            BrushData = ScriptableObject.CreateInstance<BrushData>();
            SelectionData = ScriptableObject.CreateInstance<SelectionData>();
        }

        public void Cleanup()
        {
            if (TextureData != null)
                Object.DestroyImmediate(TextureData);

            if (BrushData != null)
                Object.DestroyImmediate(BrushData);

            if (SelectionData != null)
                Object.DestroyImmediate(SelectionData);
        }
    }
}
