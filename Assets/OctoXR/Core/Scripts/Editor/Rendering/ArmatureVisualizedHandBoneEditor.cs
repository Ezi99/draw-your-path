using OctoXR.Rendering;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.Rendering
{
    [CustomEditor(typeof(ArmatureVisualizedHandBone), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class ArmatureVisualizedHandBoneEditor : HandBoneEditor
    {
        public const string JointRadiusPropertyPath = "jointRadius";
        public const string SegmentRadiusPropertyPath = "segmentRadius";

        private UnityEditor.Editor jointRendererEditor;
        private UnityEditor.Editor segmentRendererEditor;

        private bool showJointRenderer;
        private bool showSegmentRenderer;

        private readonly List<Object> jointRenderers = new List<Object>();
        private readonly List<Object> segmentRenderers = new List<Object>();

        private void OnEnable()
        {
            jointRenderers.Clear();
            segmentRenderers.Clear();

            for (var i = 0; i < serializedObject.targetObjects.Length; i++)
            {
                var bone = (ArmatureVisualizedHandBone)serializedObject.targetObjects[i];

                if (bone.Joint != null && bone.Joint.Renderer)
                {
                    jointRenderers.Add(bone.Joint.Renderer);
                }

                for (var j = 0; j < bone.Segments.Count; j++)
                {
                     var segment = bone.Segments[j];

                    if (segment != null && segment.Renderer)
                    {
                        segmentRenderers.Add(segment.Renderer);
                    }
                }
            }
        }

        private void OnDisable()
        {
            if (jointRendererEditor)
            {
                DestroyImmediate(jointRendererEditor);
            }

            if (segmentRendererEditor)
            {
                DestroyImmediate(segmentRendererEditor);
            }
        }

        protected override void DrawHandBoneProperties()
        {
            base.DrawHandBoneProperties();

            DrawEditorInFoldout(ref showJointRenderer, "Joint Renderer", ref jointRendererEditor, jointRenderers);
            DrawEditorInFoldout(ref showSegmentRenderer, "Segment Renderer", ref segmentRendererEditor, segmentRenderers);
        }

        private static void DrawEditorInFoldout(ref bool foldout, string foldoutHeader, ref UnityEditor.Editor editor, List<Object> editorTargetObjects)
        {
            foldout = editorTargetObjects.Count != 0 && EditorGUILayout.Foldout(foldout, foldoutHeader, true);

            if (foldout)
            {
                if (!editor)
                {
                    editor = CreateEditor(editorTargetObjects.ToArray());
                }

                DrawEditorIndented(editor);
            }
        }

        private static void DrawEditorIndented(UnityEditor.Editor editor)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            editor.OnInspectorGUI();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        protected override void DrawSelectBonesAndRelatedObjectsButtons()
        {
            base.DrawSelectBonesAndRelatedObjectsButtons();

            DrawSelectJointButton();
            DrawSelectSegmentsButton();
            DrawSelectJointAndSegmentsButton();
        }

        protected void DrawSelectJointButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Joint",
                "Select bone joint visualization",
                b => (b is ArmatureVisualizedHandBone vb && vb.Joint != null && vb.Joint.Transform) ?
                    new[] { vb.Joint.Transform.gameObject } : null);
        }

        protected void DrawSelectSegmentsButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Segments",
                "Select bone segment visualizations",
                b => b is ArmatureVisualizedHandBone vb ? 
                    vb.Segments.Where(s => s != null && s.Transform).Select(s => s.Transform.gameObject) : null);
        }

        protected void DrawSelectJointAndSegmentsButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Joint And Segments",
                "Select bone joint and segment visualizations",
                b =>
                {
                    var selectedObjects = new List<GameObject>();

                    if (b is ArmatureVisualizedHandBone vb)
                    {
                        if (vb.Joint != null && vb.Joint.Transform)
                        {
                            selectedObjects.Add(vb.Joint.Transform.gameObject);
                        }

                        for (var i = 0; i < vb.Segments.Count; i++)
                        {
                            var segment = vb.Segments[i];

                            if (segment != null && segment.Transform)
                            {
                                selectedObjects.Add(segment.Transform.gameObject);
                            }
                        }
                    }

                    return selectedObjects;
                });
        }
    }
}
