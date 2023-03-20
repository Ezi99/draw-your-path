using OctoXR.KinematicInteractions;
using OctoXR.KinematicInteractions.Utilities;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    public enum GrabbableType
    {
        Kinematic,
        Joint,
        Scaleable
    }
    public class CreateGrabbable : EditorWindow
    {
        private string objectName = "DefaultName";
        private Mesh meshModel;
        private GameObject gameObjectModel;
        private Material newMaterial;

        private int layerNumber = 30;

        private Vector3 boxColliderScale;
        private float sphereColliderRadius;

        private GameObject selectedObject;
        private GameObject newGrabbableObject;
        private GameObject newGrabbableVisual;

        [Range(0, 1)] [SerializeField] private float modelScale;

        private bool isJoint;
        private GrabbableType grabbableType;
        private bool isPrecision;

        private static Texture image;
        private static ColliderType grabbableColliderType;
        private static ColliderType modelColliderType;
        private static ModelType modelType;

        [MenuItem("Window/OctoXR/Kinematic Interactions/Grabbable/Create new grabbable")]
        public static void ShowEditorWindow()
        {
            GetWindow<CreateGrabbable>("Grabbable creator window");
        }

        [MenuItem("GameObject/OctoXR/Kinematic Interactions/Grabbable/Create new grabbable", false, 0)]
        public static void ShowHierarchyWindow()
        {
            GetWindow<CreateGrabbable>("Grabbable creator window");
        }

        private void OnEnable()
        {
            image = Resources.Load<Texture>("oxr_logo");
        }

        private void OnGUI()
        {
            GUILayoutSetup();
            SetupButtons();
        }

        private void GUILayoutSetup()
        {
            minSize = new Vector2(350, 500);

            GUILayout.BeginHorizontal();
            GUILayout.Box(image);

            GUILayout.BeginVertical();
            GUILayout.Box("Object properties");
            
            objectName = EditorGUILayout.TextField("Object name:", objectName);
            grabbableType = (GrabbableType) EditorGUILayout.EnumPopup("Grabbable type:", grabbableType);
            isPrecision = EditorGUILayout.Toggle("Precision grab:", isPrecision);
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Box("Model properties");

            modelType = (ModelType) EditorGUILayout.EnumPopup("Look for models of type: ", modelType);
            if (modelType == ModelType.mesh)
            {
                meshModel = (Mesh) EditorGUILayout.ObjectField("Choose model (Mesh):", meshModel, typeof(Mesh), false);
                if (!meshModel)
                    EditorGUILayout.HelpBox("Please assign a mesh as the grabbable model!", MessageType.Info);

                newMaterial =
                    (Material) EditorGUILayout.ObjectField("Choose Material:", newMaterial, typeof(Material), false);
                if (!newMaterial) EditorGUILayout.HelpBox("Please assign a material!", MessageType.Info);
            }
            else
            {
                gameObjectModel = (GameObject) EditorGUILayout.ObjectField("Choose model (Prefab):", gameObjectModel,
                    typeof(GameObject), false);
                if (!gameObjectModel)
                    EditorGUILayout.HelpBox("Please assign a prefab as the grabbable model!", MessageType.Info);
            }

            modelScale = EditorGUILayout.FloatField("Model scale: ", modelScale);
            if (modelScale <= 0)
                EditorGUILayout.HelpBox("Model scale cannot be zero!", MessageType.Info);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Box("Collider properties");
            grabbableColliderType =
                (ColliderType) EditorGUILayout.EnumPopup("Grab zone collider type:", grabbableColliderType);
            ColliderSelection();

            if (!SphereColliderCheck()) EditorGUILayout.HelpBox("Collider size cannot be zero!", MessageType.Info);
            if (!BoxColliderCheck()) EditorGUILayout.HelpBox("Box collider axis cannot be zero!", MessageType.Info);

            BoxColliderCheck();

            GUILayout.EndVertical();
        }

        private void SetupButtons()
        {
            if (GUILayout.Button("Create grabbable")) CreateButton();

            if (GUILayout.Button("Update last grabbable"))
            {
                if (!selectedObject) return;
                UpdateButton();
            }

            if (GUILayout.Button("Delete last grabbable")) DeleteButton();
        }

        private void CreateButton()
        {
            if (grabbableColliderType == ColliderType.box && boxColliderScale == Vector3.zero)
            {
                return;
            }

            if (grabbableColliderType == ColliderType.sphere && sphereColliderRadius == 0)
            {
                return;
            }

            if (modelType == ModelType.mesh)
            {
                if (!meshModel || modelScale == 0 || !newMaterial)
                {
                    if (!meshModel)
                        WarningWindow.Open("Missing model",
                            "No model found! Please assign a model to continue!");

                    if (!newMaterial)
                        WarningWindow.Open("Missing material",
                            "No material found! Please assign a material if you don't want to see eye burning pink!");

                    if (modelScale == 0)
                        WarningWindow.Open("Scale zero",
                            "Model scale cannot be zero! Please change the scale to continue!");
                    return;
                }

                newGrabbableObject = InstantiateGrabbable();
                newGrabbableVisual = CreateModelFromMesh();
            }
            else if (modelType == ModelType.gameObject)
            {
                if (!gameObjectModel || modelScale == 0)
                {
                    if (!gameObjectModel) return;
                    if (modelScale == 0) return;
                    return;
                }

                newGrabbableObject = InstantiateGrabbable();
                newGrabbableVisual = CreateModelFromGameObject();
            }

            var newGrabbable = newGrabbableObject.AddComponent<Grabbable>();

            if (grabbableType == GrabbableType.Kinematic)
            {
                var kinematic = newGrabbableObject.AddComponent<KinematicGrabbableModifier>();
                newGrabbable.GrabbableModifier = kinematic;
            }

            if (grabbableType == GrabbableType.Joint)
            {
                var joint = newGrabbableObject.AddComponent<JointBasedGrabbableModifier>();
                newGrabbable.GrabbableModifier = joint;
            }

            if (grabbableType == GrabbableType.Scaleable)
            {
                var scaleable = newGrabbableObject.AddComponent<ScaleableGrabbableModifier>();
                newGrabbable.GrabbableModifier = scaleable;
            }

            PrecisionGrabCheck();

            InstantiateGrabPoints();

            AddColliders();

            HandleModelVariables(newGrabbableVisual);
            
            selectedObject = newGrabbableVisual;
            selectedObject.AddComponent<ShiftFocusToParent>();
            
            Selection.SetActiveObjectWithContext(newGrabbableObject, newGrabbableVisual);
        }

        private void PrecisionGrabCheck()
        {
            if (isPrecision)
            {
                newGrabbableObject.GetComponent<Grabbable>().IsPrecisionGrab = true;
            }
            else
            {
                newGrabbableObject.GetComponent<Grabbable>().IsPrecisionGrab = false;
            }
        }

        private void InstantiateGrabPoints()
        {
            var grabPointLeft = new GameObject();
            var grabPointRight = new GameObject();
            SetupGrabPoint(grabPointLeft, "GrabPoint_L", HandType.Left);
            SetupGrabPoint(grabPointRight, "GrabPoint_R", HandType.Right);
        }

        private void SetupGrabPoint(GameObject grabPoint, string grabPointName, HandType handType)
        {
            grabPoint.transform.localPosition = newGrabbableObject.transform.localPosition;
            grabPoint.transform.SetParent(newGrabbableObject.transform);
            grabPoint.name = grabPointName;
            grabPoint.AddComponent<GrabPoint>().HandType = handType;
        }

        private void UpdateButton()
        {
            newGrabbableObject.name = objectName;

            if (modelType == ModelType.mesh) UpdateMeshModel();

            if (modelType == ModelType.gameObject) UpdateGameObjectModel();
            
            RemoveColliders();
            AddColliders();
            
            if (grabbableColliderType == ColliderType.sphere)
                newGrabbableObject.GetComponent<SphereCollider>().radius = sphereColliderRadius;

            if (grabbableColliderType == ColliderType.box)
                newGrabbableObject.GetComponent<BoxCollider>().size = boxColliderScale;

            HandleModelVariables(newGrabbableVisual);

            Selection.SetActiveObjectWithContext(newGrabbableObject, newGrabbableVisual);
        }

        private void UpdateGameObjectModel()
        {
            DestroyImmediate(selectedObject);
            newGrabbableVisual = CreateModelFromGameObject();
        }

        private void UpdateMeshModel()
        {
            DestroyImmediate(selectedObject);
            newGrabbableVisual = CreateModelFromMesh();
        }

        private void DeleteButton()
        {
            if (newGrabbableObject)
            {
                DestroyImmediate(newGrabbableObject);
            }

            else
            {
                return;
            }
        }

        private void ColliderSelection()
        {
            if (grabbableColliderType == ColliderType.sphere)
            {
                sphereColliderRadius =
                    EditorGUILayout.Slider("Set sphere collider radius:", sphereColliderRadius, 0f, 10f);
            }
            
            else if (grabbableColliderType == ColliderType.box)
            {
                boxColliderScale = EditorGUILayout.Vector3Field("Set box collider scale:", boxColliderScale);
                if (boxColliderScale.x <= 0) boxColliderScale.x = 0;
                if (boxColliderScale.y <= 0) boxColliderScale.y = 0;
                if (boxColliderScale.z <= 0) boxColliderScale.z = 0;
            }
        }

        private void HandleModelVariables(GameObject newGrabbableModel)
        {
            newGrabbableModel.name = $"{objectName} model";
            newGrabbableModel.gameObject.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
            newGrabbableModel.transform.SetParent(newGrabbableObject.transform);
            selectedObject = newGrabbableModel;
        }

        private void AddColliders()
        {
            if (grabbableColliderType == ColliderType.sphere)
                newGrabbableObject.AddComponent<SphereCollider>().radius = sphereColliderRadius;
            else
                newGrabbableObject.AddComponent<BoxCollider>().size = boxColliderScale;

            newGrabbableObject.GetComponent<Collider>().isTrigger = true;
        }

        private void RemoveColliders()
        {
            newGrabbableObject.TryGetComponent(out BoxCollider boxColliderTemp);
            newGrabbableObject.TryGetComponent(out SphereCollider sphereColliderTemp);
            
            if(boxColliderTemp) DestroyImmediate(boxColliderTemp);
            if(sphereColliderTemp) DestroyImmediate(sphereColliderTemp);
        }

        private GameObject CreateModelFromMesh()
        {
            newGrabbableVisual = new GameObject();
            newGrabbableVisual.transform.SetParent(newGrabbableObject.transform);
            if (!newGrabbableVisual.GetComponent<MeshFilter>()) newGrabbableVisual.AddComponent<MeshFilter>();
            if (!newGrabbableVisual.GetComponent<MeshRenderer>()) newGrabbableVisual.AddComponent<MeshRenderer>();

            var newGrabbableModelMeshRenderer = newGrabbableVisual.GetComponent<MeshRenderer>();
            var newGrabbableMesh = newGrabbableVisual.GetComponent<MeshFilter>();

            newGrabbableModelMeshRenderer.sharedMaterial = newMaterial;
            newGrabbableMesh.sharedMesh = meshModel;

            newGrabbableVisual.AddComponent<MeshCollider>();
            newGrabbableVisual.GetComponent<MeshCollider>().convex = true;

            newGrabbableVisual.transform.position = newGrabbableObject.transform.localPosition;
            return newGrabbableVisual;
        }

        private GameObject CreateModelFromGameObject()
        {
            newGrabbableVisual = Instantiate(gameObjectModel, newGrabbableObject.transform);
            newGrabbableVisual.transform.position = newGrabbableObject.transform.localPosition;
            return newGrabbableVisual;
        }

        private GameObject InstantiateGrabbable()
        {
            newGrabbableObject = new GameObject
            {
                name = objectName,
                layer = layerNumber
            };
            var rb = newGrabbableObject.AddComponent<Rigidbody>();
            newGrabbableObject.AddComponent<VelocityEstimator>();

            if (grabbableType == GrabbableType.Scaleable)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            return newGrabbableObject;
        }

        private bool BoxColliderCheck()
        {
            if (grabbableColliderType == ColliderType.box &&
                (boxColliderScale.x <= 0 || boxColliderScale.y <= 0 || boxColliderScale.z <= 0)) return false;

            return true;
        }

        private bool SphereColliderCheck()
        {
            if (grabbableColliderType == ColliderType.sphere && sphereColliderRadius <= 0) return false;

            return true;
        }
    }
}
