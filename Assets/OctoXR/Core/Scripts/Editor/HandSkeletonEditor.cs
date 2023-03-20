using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomEditor(typeof(HandSkeleton), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class HandSkeletonEditor : UnityEditor.Editor
    {
        public const string AutoAddBonesButtonLabel = "Auto Add Bones";
        public const string AutoAddBonesSearchRootObjectLabel = "Search Bones Root";
        public const float SelectBonesAndRelatedObjectsButtonMaxWith = 250;
        private GameObject manuallyAssignedAutoAddBonesRoot;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomEditorUtility.DrawInspectorScriptProperty(serializedObject);

            DrawHandSkeletonMessages();
            DrawHandSkeletonProperties();

            GUILayout.Space(10);

            DrawAutoAddBonesControls();
            DrawSelectBonesAndRelatedObjectsButtons();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawHandSkeletonProperties()
        {
            DrawPropertiesExcluding(serializedObject, CustomEditorUtility.InspectorScriptPropertyPath);
        }

        protected virtual void DrawHandSkeletonMessages()
        {
        }

        protected virtual void DrawAutoAddBonesControls()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (targets.Length == 1)
            {
                GUILayout.Label(
                    new GUIContent(
                        AutoAddBonesSearchRootObjectLabel,
                        "Object whose child objects down the hierarchy should be searched for hand bones to add to the current " +
                        $"selected hand skeleton when {AutoAddBonesButtonLabel} functionality is used. This object is condsidered only " +
                        $"when using {AutoAddBonesButtonLabel} with a single hand skeleton being edited, it is not supported in " +
                        $"multi-edit mode."));

                manuallyAssignedAutoAddBonesRoot = (GameObject)EditorGUILayout.ObjectField(
                    manuallyAssignedAutoAddBonesRoot,
                    typeof(GameObject),
                    true,
                    GUILayout.MaxWidth(200));
            }
            else
            {
                manuallyAssignedAutoAddBonesRoot = null;
            }

            if (GUILayout.Button(
                new GUIContent(
                    AutoAddBonesButtonLabel,
                    "Tries to add missing hand skeleton bones by searching the hand skeleton's child objects or child objects of " +
                    $"{AutoAddBonesSearchRootObjectLabel} if one is assigned and checking their names for keywords, e.g. if the object's " +
                    "name contains 'Index' and 'Distal' then it is added as index finger distal phalanx or if the object has HandBone " +
                    "derived component attached, but it is not added to the skeleton. Objects that are deactivated are not considered"),
                GUILayout.MaxWidth(SelectBonesAndRelatedObjectsButtonMaxWith)))
            {
                for (var i = 0; i < serializedObject.targetObjects.Length; i++)
                {
                    var handSkeleton = (HandSkeleton)serializedObject.targetObjects[i];

                    if (handSkeleton.IsComplete)
                    {
                        continue;
                    }

                    var boneKeywords = CreateBoneKeywords();
                    var removedKeywords = 0;

                    for (var j = 1; j < handSkeleton.Bones.Count; j++)
                    {
                        var boneId = handSkeleton.Bones[j].BoneId;
                        var removeKeywordAt = (int)boneId - removedKeywords - 1;

                        boneKeywords.RemoveAt(removeKeywordAt);

                        ++removedKeywords;
                    }

                    var root = manuallyAssignedAutoAddBonesRoot ? manuallyAssignedAutoAddBonesRoot.transform : handSkeleton.Transform;

                    for (var j = 0; j < root.childCount; j++)
                    {
                        TryAutoAddBonesToHandSkeletonFromRoot(root.GetChild(j), handSkeleton, boneKeywords);
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        protected virtual HandBone GetHandBoneComponent(GameObject gameObject) => gameObject.GetComponent<HandBone>();

        protected virtual void DrawSelectBonesAndRelatedObjectsButtons()
        {
            DrawSelectAllBonesButton();
        }

        protected void DrawSelectAllBonesButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Bones",
                "Select all bones attached to the current selected hand skeleton(s)",
                b => new[] { b.gameObject });
        }

        protected void DrawSelectBonesOrBoneRelatedObjectsButton(
            string buttonText,
            string buttonTooltip,
            Func<HandBone, IEnumerable<GameObject>> objectsToSelectFilter)
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(
                new GUIContent(buttonText, buttonTooltip),
                GUILayout.MaxWidth(SelectBonesAndRelatedObjectsButtonMaxWith)))
            {
                var objectsToSelect = new List<UnityEngine.Object>();

                for (var i = 0; i < serializedObject.targetObjects.Length; ++i)
                {
                    var handSkeleton = (HandSkeleton)serializedObject.targetObjects[i];

                    for (var j = 0; j < handSkeleton.Bones.Count; ++j)
                    {
                        var objects = objectsToSelectFilter(handSkeleton.Bones[j]);
                        
                        if (objects != null)
                        {
                            foreach (var obj in objects)
                            {
                                if (obj)
                                {
                                    objectsToSelect.Add(obj);
                                }
                            }
                        }
                    }
                }

                if (objectsToSelect.Count > 0)
                {
                    Selection.objects = objectsToSelect.ToArray();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void TryAutoAddBonesToHandSkeletonFromRoot(
            Transform root,
            HandSkeleton handSkeleton,
            List<HandBoneKeywordIdentity> boneKeywords)
        {
            if (!root.gameObject.activeSelf || (root.TryGetComponent<HandSkeleton>(out var otherSkeleton) && otherSkeleton != handSkeleton))
            {
                return;
            }

            var objectName = root.gameObject.name;
            var objectBoneId = default(HandBoneId?);

            for (var i = 0; i < boneKeywords.Count; ++i)
            {
                var boneKeyword = boneKeywords[i];
                var indexForFinger = objectName.IndexOf(boneKeyword.FingerKeyword, StringComparison.OrdinalIgnoreCase);

                if (indexForFinger == -1)
                {
                    continue;
                }

                var startIndexForBone = indexForFinger + boneKeyword.FingerKeyword.Length;

                if (objectName.IndexOf(boneKeyword.BoneKeyword, startIndexForBone, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    objectBoneId = boneKeyword.BoneId;

                    break;
                }
            }

            if (!objectBoneId.HasValue)
            {
                var boneComponent = GetHandBoneComponent(root.gameObject);

                if (boneComponent)
                {
                    objectBoneId = boneComponent.BoneId;

                    if (objectBoneId == HandBoneId.WristRoot)
                    {
                        objectBoneId = null;
                    }
                }
            }

            if (objectBoneId.HasValue)
            {
                var boneComponent = root.GetComponent<HandBone>();

                if (!boneComponent || (!handSkeleton.Bones.Contains(boneComponent) && !boneComponent.HandSkeleton))
                {
                    for (var i = 0; i < boneKeywords.Count; i++)
                    {
                        if (boneKeywords[i].BoneId == objectBoneId)
                        {
                            boneKeywords.RemoveAt(i);

                            break;
                        }
                    }

                    handSkeleton.AddBone(objectBoneId.Value, root.gameObject);
                    EditorUtility.SetDirty(handSkeleton);
                }
            }

            if (!handSkeleton.IsComplete)
            {
                for (var i = 0; i < root.childCount; i++)
                {
                    TryAutoAddBonesToHandSkeletonFromRoot(root.GetChild(i), handSkeleton, boneKeywords);
                }
            }
        }

        private struct HandBoneKeywordIdentity
        {
            public string FingerKeyword;
            public string BoneKeyword;
            public HandBoneId BoneId;

            public HandBoneKeywordIdentity(string boneKeyword, HandBoneId boneId)
            {
                FingerKeyword = string.Empty;
                BoneKeyword = boneKeyword;
                BoneId = boneId;
            }

            public HandBoneKeywordIdentity(string fingerKeyword, string boneKeyword, HandBoneId boneId)
            {
                FingerKeyword = fingerKeyword;
                BoneKeyword = boneKeyword;
                BoneId = boneId;
            }
        }

        private static List<HandBoneKeywordIdentity> CreateBoneKeywords()
        {
            return new List<HandBoneKeywordIdentity>
            {
                //new HandBoneKeywordIdentity("Wrist", "Root", HandBoneId.WristRoot),
                new HandBoneKeywordIdentity("Thumb", "Metacarpal", HandBoneId.ThumbFingerMetacarpal),
                new HandBoneKeywordIdentity("Thumb", "Proximal", HandBoneId.ThumbFingerProximalPhalanx),
                new HandBoneKeywordIdentity("Thumb", "Distal", HandBoneId.ThumbFingerDistalPhalanx),
                new HandBoneKeywordIdentity("Index", "Proximal", HandBoneId.IndexFingerProximalPhalanx),
                new HandBoneKeywordIdentity("Index", "Middle", HandBoneId.IndexFingerMiddlePhalanx),
                new HandBoneKeywordIdentity("Index", "Distal", HandBoneId.IndexFingerDistalPhalanx),
                new HandBoneKeywordIdentity("Middle", "Proximal", HandBoneId.MiddleFingerProximalPhalanx),
                new HandBoneKeywordIdentity("Middle", "Middle", HandBoneId.MiddleFingerMiddlePhalanx),
                new HandBoneKeywordIdentity("Middle", "Distal", HandBoneId.MiddleFingerDistalPhalanx),
                new HandBoneKeywordIdentity("Ring", "Proximal", HandBoneId.RingFingerProximalPhalanx),
                new HandBoneKeywordIdentity("Ring", "Middle", HandBoneId.RingFingerMiddlePhalanx),
                new HandBoneKeywordIdentity("Ring", "Distal", HandBoneId.RingFingerDistalPhalanx),
                new HandBoneKeywordIdentity("Pinky", "Proximal", HandBoneId.PinkyFingerProximalPhalanx),
                new HandBoneKeywordIdentity("Pinky", "Middle", HandBoneId.PinkyFingerMiddlePhalanx),
                new HandBoneKeywordIdentity("Pinky", "Distal", HandBoneId.PinkyFingerDistalPhalanx),
                new HandBoneKeywordIdentity("Thumb", "Tip", HandBoneId.ThumbFingerTip),
                new HandBoneKeywordIdentity("Index", "Tip", HandBoneId.IndexFingerTip),
                new HandBoneKeywordIdentity("Middle", "Tip", HandBoneId.MiddleFingerTip),
                new HandBoneKeywordIdentity("Ring", "Tip", HandBoneId.RingFingerTip),
                new HandBoneKeywordIdentity("Pinky", "Tip", HandBoneId.PinkyFingerTip),
            };
        }
    }
}
