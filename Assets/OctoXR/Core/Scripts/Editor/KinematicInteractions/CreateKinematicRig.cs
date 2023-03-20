using OctoXR.KinematicInteractions;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.KinematicInteractions
{
    public class CreateKinematicRig : EditorWindow
    {
        private Texture image;
        private string rigName = "OctoRig";
        
        private static GameObject kinematicRig;
        private GameObject instantiatedRig;
        private GameObject leftHandProximity;
        private GameObject rightHandProximity;

        private GameObject leftHandDistance;
        private GameObject rightHandDistance;

        private KinematicInteractorType leftHandKinematicInteractorType;
        private KinematicInteractorType rightHandKinematicInteractorType;

        private KinematicInteractionType leftHandPinchCheck;
        private KinematicInteractionType rightHandPinchCheck;

        //[MenuItem("Window/OctoXR/Kinematic Interactions/Create Kinematic Rig")]
        public static void SpawnRig()
        {
            GetWindow<CreateKinematicRig>("Kinematic Rig Window");
        }

        //[MenuItem("GameObject/OctoXR/Kinematic Interactions/Create Kinematic Rig")]
        public static void SpawnRigFromHierarchy()
        {
            GetWindow<CreateKinematicRig>("Kinematic Rig Window");
        }

        private void OnEnable()
        {
            image = Resources.Load<Texture>("oxr_logo");
        }

        private void Awake()
        {
            kinematicRig = (GameObject) Resources.Load("OctoKinematicRig");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(image);

            GUILayout.BeginVertical();
            GUILayout.Box("Rig properties");

            rigName = EditorGUILayout.TextField("Rig name:", rigName);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Box("Left hand properties");

            leftHandKinematicInteractorType =
                (KinematicInteractorType) EditorGUILayout.EnumPopup("Left hand grabber type:",
                    leftHandKinematicInteractorType);
            leftHandPinchCheck =
                (KinematicInteractionType) EditorGUILayout.EnumPopup("Pinch or grab:", leftHandPinchCheck);

            GUILayout.Box("Right hand properties");

            rightHandKinematicInteractorType =
                (KinematicInteractorType) EditorGUILayout.EnumPopup("Right hand grabber type:",
                    rightHandKinematicInteractorType);
            rightHandPinchCheck =
                (KinematicInteractionType) EditorGUILayout.EnumPopup("Pinch or grab:", rightHandPinchCheck);

            if (rightHandKinematicInteractorType == KinematicInteractorType.both ||
                leftHandKinematicInteractorType == KinematicInteractorType.both)
                EditorGUILayout.HelpBox(
                    "Please note that using both interactor types on a single hand is experimental and may cause unwanted behaviour!",
                    MessageType.Warning);

            GUILayout.Box("", GUIStyle.none);

            if (GUILayout.Button("Create kinematic rig"))
            {
                if(instantiatedRig) DestroyImmediate(instantiatedRig);
                
                instantiatedRig = Instantiate(kinematicRig);
                instantiatedRig.name = rigName;

                FindInteractors();
                
                TogglePinch(leftHandPinchCheck, leftHandProximity);
                TogglePinch(rightHandPinchCheck, rightHandProximity);
                
                ActivateGrabType(leftHandKinematicInteractorType, leftHandProximity, leftHandDistance);
                ActivateGrabType(rightHandKinematicInteractorType, rightHandProximity, rightHandDistance);
                
                Selection.SetActiveObjectWithContext(instantiatedRig, null);
            }
        }

        private void FindInteractors()
        {
            leftHandProximity =
                GameObject.Find(
                    $"{rigName}/TrackingSpace/OctoHand_Left/HandModel_Left/Root/Marker_PalmCenter/ProximityInteractor");
            leftHandDistance =
                GameObject.Find(
                    $"{rigName}/TrackingSpace/OctoHand_Left/HandModel_Left/Root/Marker_PalmCenter/DistanceInteractor");

            rightHandProximity =
                GameObject.Find(
                    $"{rigName}/TrackingSpace/OctoHand_Right/HandModel_Right/Root/Marker_PalmCenter/ProximityInteractor");
            rightHandDistance =
                GameObject.Find(
                    $"{rigName}/TrackingSpace/OctoHand_Right/HandModel_Right/Root/Marker_PalmCenter/DistanceInteractor");
        }

        private void ActivateGrabType(KinematicInteractorType kinematicInteractorType, GameObject proximity,
            GameObject distance)
        {
            if (kinematicInteractorType == KinematicInteractorType.proximity)
            {
                proximity.SetActive(true);
                distance.SetActive(false);
            }

            if (kinematicInteractorType == KinematicInteractorType.distance)
            {
                proximity.SetActive(false);
                distance.SetActive(true);
            }

            if (kinematicInteractorType == KinematicInteractorType.both)
            {
                proximity.SetActive(true);
                distance.SetActive(true);
            }
        }

        private void TogglePinch(KinematicInteractionType kinematicInteractionType, GameObject interactor)
        {
            if (kinematicInteractionType == KinematicInteractionType.pinch)
            {
                interactor.GetComponent<InteractionHandler>().InteractionHand.IsPinch = true;    
            }

            if (kinematicInteractionType == KinematicInteractionType.grab)
            {
                interactor.GetComponent<InteractionHandler>().InteractionHand.IsPinch = false;
            }
        }
    }
}