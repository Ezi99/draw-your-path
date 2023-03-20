using UnityEngine;

namespace OctoXR.Rendering
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SkinnedMeshVisualizedHandSkeleton : VisualizedHandSkeleton
    {
        [SerializeField]
        [HideInInspector]
        private new SkinnedMeshRenderer renderer;
        /// <summary>
        /// Renderer used to render the hand skeleton skinned mesh
        /// </summary>
        public SkinnedMeshRenderer Renderer => renderer;

        [SerializeField]
        [HideInInspector]
        private Transform[] _bones;

        protected override void Reset()
        {
            base.Reset();

            renderer = GetComponent<SkinnedMeshRenderer>();

            UpdateBones();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            renderer = GetComponent<SkinnedMeshRenderer>();

            UpdateBones();
        }

        protected override void Awake()
        {
            base.Awake();

            renderer = GetComponent<SkinnedMeshRenderer>();

            UpdateBones();
        }

        protected override void LateUpdate()
        {
            UpdatePose();
            SetBoneBindPoses();

            UpdateBones();

            FinalizePoseUpdate();
        }

        private void UpdateBones()
        {
            var bones = Bones;
#if UNITY_EDITOR
            var setDirty = false;
#endif
            if (_bones == null || _bones.Length != bones.Count)
            {
                _bones = new Transform[bones.Count];
#if UNITY_EDITOR
                setDirty = true;
#endif
            }

            for (var i = 0; i < _bones.Length; i++)
            {
#if UNITY_EDITOR
                if (_bones[i] != bones[i].Transform)
                {
                    setDirty = true;
                }
#endif
                _bones[i] = bones[i].Transform;
            }
#if UNITY_EDITOR
            if (setDirty)
            {
                ObjectUtility.SetObjectDirty(this);
            }
#endif
            renderer.bones = _bones;
        }
    }
}
