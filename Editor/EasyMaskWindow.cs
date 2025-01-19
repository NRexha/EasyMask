using UnityEditor;
using UnityEngine;

namespace EasyMaskTool
{
    public class EasyMaskWindow : EditorWindow
    {
        private DataManager _dataManager;
        private Vector2 _scrollPosition;
        private bool _isPainting;
        private bool _usingEraser;

        #region Properties
        private SerializedObject _sOBrush;
        private SerializedProperty _propBrushShape;
        private SerializedProperty _propBrushRadius;
        private SerializedProperty _propBrushOpacity;
        private SerializedProperty _propBrushSmoothness;
        private SerializedProperty _propBrushRepetition;

        private SerializedObject _sOTexture;
        private SerializedProperty _propTextureResolution;
        private SerializedProperty _propOutputType;
        private SerializedProperty _propTextureChannel;
        private SerializedProperty _propAxisSymmetry; 
        #endregion

        [MenuItem("Tools/Easy Mask")]
        public static void CreateWindow()
        {
            EasyMaskWindow mainWindow = GetWindowWithRect< EasyMaskWindow>(new Rect(0, 0, 300, 360));
            Vector2 windowSize = new (300, 200);
            mainWindow.minSize = windowSize;
            Texture2D icon = Resources.Load<Texture2D>("EasyMaskIcon");
            mainWindow.titleContent = new GUIContent("Easy Mask", icon);
            mainWindow.Focus();
        }


        private void OnEnable()
        {
            _dataManager = new DataManager();

            Selection.selectionChanged += Repaint;
            Selection.selectionChanged += OnSelectionChanged;
            SceneView.duringSceneGui += OnSceneGUI;
            OnSelectionChanged();
            SetupProperties();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            Selection.selectionChanged -= OnSelectionChanged;
            SceneView.duringSceneGui -= OnSceneGUI;
            ResetData();

            _dataManager.Cleanup();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            Event currentEvent = Event.current;
            HandleSymmetry(currentEvent);
            if (currentEvent.alt) return;

            if (_isPainting)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                Paint(currentEvent);
            }
        }

        #region Helpers
        private void Initialize()
        {
            TextureInitializer.InitializeTexture(_dataManager.TextureData);
            ModelInitializer.InitializeDebugMaterial(_dataManager.TextureData);
            EasyMaskPainter.InitializeComputeShader(_dataManager.TextureData);
        }
        private void ResetData()
        {
            ModelInitializer.RemoveAllAddedColliders(_dataManager.SelectionData);
            ModelInitializer.RevertToOriginalMaterials();
            _dataManager.TextureData.symmetry = TextureData.EAxisSymmetry.None;
            _dataManager.TextureData.hasSymmetryPoint = false;

        }
        private void OnSelectionChanged()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            _dataManager.SelectionData.selectedObjects = selectedObjects.Length > 0 ? new(selectedObjects) : new();
        }
        private void Paint(Event currentEvent)
        {
            _dataManager.BrushData.brushShapeMaterial.SetTexture("_BrushShape", _dataManager.BrushData.brushShape);
            Vector3 brushParams = new Vector3(
                _dataManager.BrushData.brushSmoothness,
                _dataManager.BrushData.brushOpacity,
                _dataManager.BrushData.brushRepetition
            );
            _dataManager.BrushData.brushShapeMaterial.SetVector("_BrushParams", brushParams);

            if (_dataManager.TextureData.debugMaterial != null)
                _dataManager.TextureData.debugMaterial.SetInt("_ActiveChannel", (int)_dataManager.TextureData.textureChannel);
            

            BrushPreview.DrawBrushPreview(_dataManager.BrushData);

            if ((currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag) && currentEvent.button == 0)
            {
                EasyMaskPainter.PaintOnSurface(_dataManager.TextureData, _dataManager.BrushData);
                currentEvent.Use();
            }
        }
        private void HandleSymmetry(Event currentEvent)
        {
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _dataManager.TextureData.symmetryPoint = hit.point;
                    _dataManager.TextureData.hasSymmetryPoint = true;
                    currentEvent.Use();
                }
                Repaint();

            }
            AxisSymmetry.DrawSymmetryPoint(_dataManager.TextureData );
        }
        #endregion

        #region DrawGUI
        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Painting Settings", StylePresets.SubtitleStyle);
                DrawPaintProperties();
            }
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Brush Settings", StylePresets.SubtitleStyle);
                DrawBrushParams();
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Output Texture Settings", StylePresets.SubtitleStyle);
                DrawTextureProperties();
            }
            EditorGUILayout.EndScrollView();
        }

        private void SetupProperties()
        {
            _sOBrush = new SerializedObject(_dataManager.BrushData);
            _propBrushShape = _sOBrush.FindProperty(HandyMethods.GetName(() => _dataManager.BrushData.brushShape));
            _propBrushRadius = _sOBrush.FindProperty(HandyMethods.GetName(() => _dataManager.BrushData.brushRadius));
            _propBrushOpacity = _sOBrush.FindProperty(HandyMethods.GetName(() => _dataManager.BrushData.brushOpacity));
            _propBrushSmoothness = _sOBrush.FindProperty(HandyMethods.GetName(() => _dataManager.BrushData.brushSmoothness));
            _propBrushRepetition = _sOBrush.FindProperty(HandyMethods.GetName(() => _dataManager.BrushData.brushRepetition));
            _sOTexture = new SerializedObject(_dataManager.TextureData);
            _propTextureResolution = _sOTexture.FindProperty(HandyMethods.GetName(() => _dataManager.TextureData.textureOutputResolution));
            _propOutputType = _sOTexture.FindProperty(HandyMethods.GetName(() => _dataManager.TextureData.textureOutputType));
            _propTextureChannel = _sOTexture.FindProperty(HandyMethods.GetName(() => _dataManager.TextureData.textureChannel));
            _propAxisSymmetry = _sOTexture.FindProperty(HandyMethods.GetName(() => _dataManager.TextureData.symmetry));
        }
        private void DrawBrushParams()
        {

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!_isPainting || !_usingEraser))
                {
                    if (GUILayout.Button("Brush", StylePresets.ButtonStyle))
                    {
                        _dataManager.BrushData.brushColor = Color.white;
                        _usingEraser = false;
                    }
                    
                }
                using (new EditorGUI.DisabledScope(!_isPainting || _usingEraser))
                {
                    if (GUILayout.Button("Eraser", StylePresets.ButtonStyle))
                    {
                        _usingEraser = true;
                        _dataManager.BrushData.brushColor = Color.black;
                    }
                }
            }
            using (new EditorGUI.DisabledScope(!_isPainting))
            {
                _sOBrush.Update();
                EditorGUILayout.PropertyField(_propBrushShape);
                EditorGUILayout.PropertyField(_propBrushRadius);
                EditorGUILayout.PropertyField(_propBrushOpacity);
                EditorGUILayout.PropertyField(_propBrushSmoothness);
                EditorGUILayout.PropertyField(_propBrushRepetition);
                _sOBrush.ApplyModifiedProperties();
            }
        }
        private void DrawTextureProperties()
        {
            _sOTexture.Update();
            using (new EditorGUI.DisabledScope(!_isPainting))
            {
                EditorGUILayout.PropertyField(_propOutputType);
                EditorGUILayout.PropertyField(_propTextureResolution);
            }
            _sOTexture.ApplyModifiedProperties();
            using (new EditorGUI.DisabledScope(_dataManager.TextureData.currentTexture == null || !_isPainting))
            {
                if (GUILayout.Button("Export Texture", StylePresets.ButtonStyle))
                {
                    TextureExporter.SaveTexture(_dataManager.TextureData);
                }
            }
        }
        private void DrawPaintProperties()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0 || _isPainting))
                {
                    if (GUILayout.Button("Enter Paint Mode", StylePresets.ButtonStyle))
                    {
                        _isPainting = true;
                        Initialize();
                        foreach (GameObject obj in _dataManager.SelectionData.selectedObjects)
                        {
                            ModelInitializer.ProvideColliders(obj.transform, _dataManager.SelectionData);
                            ModelInitializer.StoreOriginalMaterials(_dataManager.SelectionData.selectedObjects);
                            ModelInitializer.AssignDebugMaterial(_dataManager.SelectionData.selectedObjects);
                        }
                    }
                }
                using (new EditorGUI.DisabledScope(!_isPainting))
                {
                    if (GUILayout.Button("Exit Paint Mode", StylePresets.ButtonStyle))
                    {
                        _isPainting = false;
                        ResetData();
                    }
                }

            }
            _sOTexture.Update();

            using (new EditorGUI.DisabledScope(!_isPainting))
            {
                EditorGUILayout.PropertyField(_propTextureChannel);
                using (new EditorGUI.DisabledScope(_dataManager.TextureData.hasSymmetryPoint == false))
                {
                    EditorGUILayout.PropertyField(_propAxisSymmetry);
                }
            }


            _sOTexture.ApplyModifiedProperties();
            using (new EditorGUI.DisabledScope(!_isPainting || (!_dataManager.TextureData.hasSymmetryPoint &&
                                               _dataManager.TextureData.symmetry == TextureData.EAxisSymmetry.None) ||
                                               !_dataManager.TextureData.hasSymmetryPoint))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Clear Symmetry Point", StylePresets.ButtonStyle))
                    {
                        _dataManager.TextureData.hasSymmetryPoint = false;
                    }
                    using (new EditorGUI.DisabledScope(_dataManager.SelectionData.selectedObjects.Count == 0 || !_dataManager.TextureData.hasSymmetryPoint))
                    {
                        if (GUILayout.Button("Center Symmetry Point", StylePresets.ButtonStyle))
                        {
                            AxisSymmetry.CenterSymmetryPoint(_dataManager.TextureData);
                        }
                    }
                      
                }
                   
            }
        }

        #endregion
    }
}
