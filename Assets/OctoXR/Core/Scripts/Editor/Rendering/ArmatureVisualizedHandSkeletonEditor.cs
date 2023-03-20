using OctoXR.Rendering;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OctoXR.Editor.Rendering
{
    [CustomEditor(typeof(ArmatureVisualizedHandSkeleton), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class ArmatureVisualizedHandSkeletonEditor : HandSkeletonEditor
    {
        public const string ArmatureVisualizedBoneItemsPropertyPath = "armatureVisualizedBones.items";
        public const string ArmatureVisualizedHandBoneJointRadiusPropertyPath = "jointRadius";

        private static readonly GUIContent rootRadiusPropertyLabel =
            new GUIContent("Root Radius", "Radius of the skeleton root visualization");

        protected override void DrawSelectBonesAndRelatedObjectsButtons()
        {
            DrawSelectAllBonesButton();
            DrawSelectJointsButton();
            DrawSelectSegmentsButton();
            DrawSelectJointsAndSegmentsButton();
        }

        protected void DrawSelectJointsButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Joints",
                "Select all skeleton bone joint visualizations",
                b => (b is ArmatureVisualizedHandBone vb && vb.Joint != null && vb.Joint.Transform) ?
                    new[] { vb.Joint.Transform.gameObject } : null);
        }

        protected void DrawSelectSegmentsButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Segments",
                "Select all skeleton bone segment visualizations",
                b => b is ArmatureVisualizedHandBone vb ? 
                    vb.Segments.Where(s => s != null && s.Transform).Select(s => s.Transform.gameObject) : null);
        }

        protected void DrawSelectJointsAndSegmentsButton()
        {
            DrawSelectBonesOrBoneRelatedObjectsButton(
                "Select Joints And Segments",
                "Select all skeleton bone joint and segment visualizations",
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
