using UnityEditor;

namespace OctoXR.Editor
{
    [CustomEditor(typeof(HandSkeletonSourcedPoseProvider), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class HandSkeletonSourcedPoseProviderEditor : UnityEditor.Editor
    {
        public static readonly string ScalePropertyPath =
            nameof(HandSkeletonSourcedPoseProvider.Scale).Substring(0, 1).ToLowerInvariant() +
            nameof(HandSkeletonSourcedPoseProvider.Scale).Substring(1);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomEditorUtility.DrawInspectorScriptProperty(serializedObject);

            CheckAndDrawSourceHandSkeletonNotCompleteAndWithoutPoseProviderWarning();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(ScalePropertyPath));
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }

        private void CheckAndDrawSourceHandSkeletonNotCompleteAndWithoutPoseProviderWarning()
        {
            var sourceHandSkeletonsNotCompleteAndWithoutPoseProviderPresent = false;

            for (var i = 0; i < serializedObject.targetObjects.Length; ++i)
            {
                var poseProvider = (HandSkeletonSourcedPoseProvider)serializedObject.targetObjects[i];

                if (!poseProvider.HandSkeleton.IsComplete && !poseProvider.HandSkeleton.PoseProvider)
                {
                    sourceHandSkeletonsNotCompleteAndWithoutPoseProviderPresent = true;

                    break;
                }
            }

            if (sourceHandSkeletonsNotCompleteAndWithoutPoseProviderPresent)
            {
                var warningMessage =
                    "Hand skeleton used as a source for the pose provider's poses does not have a pose provider assigned and it is not complete. " +
                    "Hand skeleton sourced pose provider cannot function with such hand skeleton";

                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning, true);
            }
        }
    }
}
