using UnityEngine;

namespace OctoXR
{
    public class BasicHandSkeleton : HandSkeleton
    {
        [SerializeField]
        [Tooltip("Determines when the hand skeleton should update the hand pose")]
        private UpdateRun updateRun = UpdateRun.Update;
        /// <summary>
        /// Determines when the hand skeleton should update the hand pose
        /// </summary>
        public UpdateRun UpdateRun
        {
            get => updateRun;
            set
            {
#if UNITY_EDITOR
                if (updateRun == value)
                {
                    return;
                }
#endif
                updateRun = value;

                ObjectUtility.SetObjectDirty(this);
            }
        }

        protected virtual void FixedUpdate()
        {
            if ((updateRun & UpdateRun.FixedUpdate) == UpdateRun.FixedUpdate)
            {
                RunUpdate();
            }
        }

        protected virtual void Update()
        {
            if ((updateRun & UpdateRun.Update) == UpdateRun.Update)
            {
                RunUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            if ((updateRun & UpdateRun.LateUpdate) == UpdateRun.LateUpdate)
            {
                RunUpdate();
            }
        }
        
        private void RunUpdate()
        {
            UpdatePose();
            SetBoneBindPoses();
            FinalizePoseUpdate();
        }
    }
}
