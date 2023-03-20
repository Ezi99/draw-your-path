using OctoXR.Editor.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Scripts.Editor
{
    [InitializeOnLoad]
    public class OctoSetupWizard : EditorWindow
    {
        private static OctoSetupWizard window;
        private static OctoSettings octoSettings;
        private static Texture image;
        private static float spacing = 12f;

        static OctoSettings OctoSettings
        {
            get
            {
                if (!octoSettings)
                {
                    octoSettings = Resources.Load<OctoSettings>("DefaultOctoSettings");
                }

                return octoSettings;
            }
        }

        static OctoSetupWizard()
        {
            EditorApplication.update += Start;
        }
    
        static void Start()
        {
            if (OctoSettings.ignoreSetup == false) OpenWindow();    
            EditorApplication.update -= Start;
        }

        [MenuItem("Window/OctoXR/Setup Window")]
        public static void OpenWindow() {
            window = GetWindow<OctoSetupWizard>(true);
            window.minSize = new Vector2(320, 440);
            window.maxSize = new Vector2(360, 500);
            window.titleContent = new GUIContent("OctoXR setup");
        }

        private void OnEnable()
        {
            image = Resources.Load<Texture>("oxr_banner");
        }
    
        private void OnGUI()
        {
            GUILayoutSetup();
        }
    
        private void GUILayoutSetup()
        {
            var headerLabelStyle = OctoGUIStyles.LabelStyle(TextAnchor.MiddleLeft, FontStyle.Bold, 15);
            var smallLabelStyle = OctoGUIStyles.LabelStyle(TextAnchor.MiddleLeft, FontStyle.Normal, 12);
            var titleLabelStyle = OctoGUIStyles.LabelStyle(TextAnchor.MiddleCenter, FontStyle.Normal, 25);
            var rect = EditorGUILayout.GetControlRect();
            rect.height *= 5;
        
            minSize = new Vector2(350, 500);

            GUILayout.BeginHorizontal();
        
            GUI.Label(rect, (Texture2D)Resources.Load("oxr_banner", typeof(Texture2D)), OctoGUIStyles.LabelStyle(TextAnchor.MiddleCenter, FontStyle.Normal, 25));
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(spacing / 2);
            GUILayout.Label("OctoXR Setup", titleLabelStyle);
        
            GUILayout.Space(spacing * 3);

            GUILayout.Box("OctoXR has a few prerequisites that are necessary in order for the package to work, " +
                          "like Grabbable and Hands layers " +
                          "as well as a specific set of physics and time settings. " +
                          "Press the button below to set everything up.",OctoGUIStyles.BoxStyle(TextAnchor.MiddleCenter, FontStyle.Normal, 12));

            GUILayout.Label("Layers", headerLabelStyle);
            GUILayout.Label("OctoPlayer on layer 29", smallLabelStyle);
            GUILayout.Label("Grabbable on layer 30", smallLabelStyle);
            GUILayout.Label("Hand on Layer 31", smallLabelStyle);
        
            GUILayout.Space(spacing);
        
            GUILayout.Label("Physics settings", headerLabelStyle);
            GUILayout.Label("Default solver iterations: 25", smallLabelStyle);
            GUILayout.Label("Default solver velocity iterations: 15", smallLabelStyle);
            GUILayout.Label("Hands layer ignores self collision", smallLabelStyle);
        
            GUILayout.Space(spacing);
        
            GUILayout.Label("Time settings", headerLabelStyle);
            GUILayout.Label("Fixed timestep: 0.01111111", smallLabelStyle);
        
            GUILayout.EndVertical();
        
            GUILayout.Space(spacing);
        
            if (GUILayout.Button("Add required layers and modify physics settings"))
            {
                SetRequiredSettings();
                EditorUtility.SetDirty(OctoSettings);
                WarningWindow.Open("Settings confirmation", "Settings applied!");
                OctoSettings.ignoreSetup = true;
            };
        
        }

        public static void SetRequiredSettings()
        {
            GenerateRequiredLayers();
            GeneratePhysicsSettings();
            GenerateTimeSettings();
        }

        private static void GenerateRequiredLayers()
        {
            AddLayer(Constants.OctoPlayer, 29);
            AddLayer(Constants.Grabbable, 30);
            AddLayer(Constants.Hand, 31);
        }
    
        private static void AddLayer(string layerName, int layerNumber)
        {
            Object[] tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        
            if (tagManager != null && tagManager.Length > 0)
            {
                SerializedObject serializedObject = new SerializedObject(tagManager[0]);
                SerializedProperty layers = serializedObject.FindProperty("layers");

                layers.DeleteArrayElementAtIndex(layerNumber);
                layers.InsertArrayElementAtIndex(layerNumber);
                layers.GetArrayElementAtIndex(layerNumber).stringValue = layerName;

                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void GeneratePhysicsSettings()
        {
            UnityEngine.Physics.defaultSolverIterations = OctoSettings.defSolverIterations;
            UnityEngine.Physics.defaultSolverVelocityIterations = OctoSettings.defSolverVelocityIterations;
            UnityEngine.Physics.IgnoreLayerCollision(29, 29);
            UnityEngine.Physics.IgnoreLayerCollision(29, 30);
            UnityEngine.Physics.IgnoreLayerCollision(29, 31);
        }

        private static void GenerateTimeSettings()
        {
            Time.fixedDeltaTime = octoSettings.fixedTimeStep;
        }
    }
}
