using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    public class CreateSnapZone : EditorWindow
    {
        private string snapZoneName = "DefaultName";
        private Vector3 boxScale = Vector3.one;
        private Vector3 rotationFactor;
        private float sphereRadius = 1;
        private float scaleFactor;
        private ColliderType snapZoneType;
        private Material newMaterial;
        private GameObject snapZone;

        private bool isUniformScaling = true;

        private static Texture image;

        [MenuItem("Window/OctoXR/Kinematic Interactions/Utilities/Create new snap zone")]
        public static void ShowEditorWindow()
        {
            GetWindow<CreateSnapZone>("Snap Zone Window");
        }

        [MenuItem("GameObject/OctoXR/Kinematic Interactions/Utilities/Create snap zone", false, 0)]
        public static void ShowHierarchyWindow()
        {
            GetWindow<CreateSnapZone>("Snap Zone Window");
        }

        private void OnEnable()
        {
            image = Resources.Load<Texture>("oxr_logo");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(image);

            GUILayout.BeginVertical();
            GUILayout.Box("Snap zone properties");

            snapZoneName = EditorGUILayout.TextField("Snap zone name:", snapZoneName);
            snapZoneType = (ColliderType) EditorGUILayout.EnumPopup("Snap zone type: ", snapZoneType);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            isUniformScaling = EditorGUILayout.Toggle("Uniform scale?", isUniformScaling);

            if (isUniformScaling)
            {
                sphereRadius = EditorGUILayout.Slider("Set snap zone scale:", sphereRadius, 0f, 10f);
                if (sphereRadius <= 0)
                    EditorGUILayout.HelpBox("Scale cannot be zero!", MessageType.Info);
            }
            else
            {
                boxScale = EditorGUILayout.Vector3Field("Set snap zone scale:", boxScale);
                if (boxScale.x <= 0) boxScale.x = 0;
                if (boxScale.y <= 0) boxScale.y = 0;
                if (boxScale.z <= 0) boxScale.z = 0;
                
                if(boxScale.x <= 0 || boxScale.y <= 0 || boxScale.z <= 0)
                    EditorGUILayout.HelpBox("Scale cannot be zero!", MessageType.Info);
            }

            GUILayout.EndVertical();

            newMaterial =
                (Material) EditorGUILayout.ObjectField("Choose Material:", newMaterial, typeof(Material), false);
            if (!newMaterial) EditorGUILayout.HelpBox("Please assign a material!", MessageType.Info);

            GUILayout.Box("Captured object properties");

            // GUILayout.BeginVertical();
            // scaleFactor = EditorGUILayout.Slider("Scale factor:", scaleFactor, 0f, 1f);
            // rotationFactor = EditorGUILayout.Vector3Field("Rotation factor:", rotationFactor);
            // GUILayout.EndVertical();
            //
            // GUILayout.Box("", GUIStyle.none);

            if (GUILayout.Button("Create snap zone"))
            {
                if (snapZoneType == ColliderType.sphere)
                    snapZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                else
                    snapZone = GameObject.CreatePrimitive(PrimitiveType.Cube);
                snapZone.GetComponent<MeshRenderer>().sharedMaterial = newMaterial;
                if (isUniformScaling)
                {
                    snapZone.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                }
                else
                {
                    snapZone.transform.localScale = boxScale;
                }
                snapZone.name = snapZoneName;
                snapZone.AddComponent<SnapZone>();
                snapZone.GetComponent<Collider>().isTrigger = true;
                
                //var snapZoneComponent = snapZone.GetComponent<SnapZone>();
                //snapZoneComponent.ChangeScaleFactor(scaleFactor);
                //snapZoneComponent.ChangeRotation(rotationFactor.x, rotationFactor.y, rotationFactor.z);
            }
        }
    }
}