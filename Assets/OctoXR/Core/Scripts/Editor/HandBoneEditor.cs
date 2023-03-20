using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor
{
    [CustomEditor(typeof(HandBone), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class HandBoneEditor : UnityEditor.Editor
    {
        public const float SelectBonesAndRelatedObjectsButtonMaxWith = HandSkeletonEditor.SelectBonesAndRelatedObjectsButtonMaxWith;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CustomEditorUtility.DrawInspectorScriptProperty(serializedObject);

            DrawHandBoneMessages();
            DrawHandBoneProperties();

            GUILayout.Space(10);

            DrawSelectBonesAndRelatedObjectsButtons();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawHandBoneProperties()
        {
            DrawPropertiesExcluding(serializedObject, CustomEditorUtility.InspectorScriptPropertyPath);
        }

        protected virtual void DrawHandBoneMessages()
        {
            CheckAndDrawNoHandSkeletonWarning();
        }

        protected virtual void DrawSelectBonesAndRelatedObjectsButtons()
        {
            DrawSelectAllBonesButton();
            DrawSelectParentBonesButton();
        }

        protected void CheckAndDrawNoHandSkeletonWarning()
        {
            const string warningMessageStartForSingleObject = "Hand bone is not ";
            const string warningMessageStartForMultipleObjects = "One or more of selected hand bones are not ";

            var noHandPhysicsSkeletonBonesPresent = false;

            for (var i = 0; i < serializedObject.targetObjects.Length; ++i)
            {
                var bone = (HandBone)serializedObject.targetObjects[i];

                if (!bone.HandSkeleton)
                {
                    noHandPhysicsSkeletonBonesPresent = true;

                    break;
                }
            }

            if (noHandPhysicsSkeletonBonesPresent)
            {
                var warningMessage = (serializedObject.isEditingMultipleObjects ?
                    warningMessageStartForMultipleObjects : warningMessageStartForSingleObject) +
                    "added to a hand skeleton. Hand bones should always be added to a hand skeleton because " +
                    "they have no meaningful function on their own.";

                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning, true);
            }
        }

        protected void DrawSelectAllBonesButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select All Bones",
                "Select all bones attached to the same hand skeleton(s) as the current selected bone(s)",
                b => b.HandSkeleton ? b.HandSkeleton.Bones.Select(x => x.gameObject) : null);
        }

        protected void DrawSelectParentBonesButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Parent Bone",
                "Select parent bone(s) of the current selected bone(s)",
                b => b.ParentBone ? new[] { b.ParentBone.gameObject } : null);
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
                    var handBone = (HandBone)serializedObject.targetObjects[i];
                    var objects = objectsToSelectFilter(handBone);

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

                if (objectsToSelect.Count > 0)
                {
                    Selection.objects = objectsToSelect.ToArray();
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}
