using UnityEngine;

namespace OctoXR.Integrations.Oculus
{
    public static class OVRUtility
    {
        public static OVRPlugin.Hand HandTypeToOvrHand(HandType handType) => (OVRPlugin.Hand)handType;
        public static OVRPlugin.SkeletonType HandTypeToOvrSkeletonType(HandType handType) => (OVRPlugin.SkeletonType)handType;

        public static Pose GetBonePoseFromOvrBonePose(OVRPlugin.Posef ovrPose, HandType handType) =>
            handType == HandType.Left ? GetBonePoseFromOvrBonePoseLeft(ovrPose) : GetBonePoseFromOvrBonePoseRight(ovrPose);

        public static Pose GetBonePoseFromOvrBonePoseLeft(OVRPlugin.Posef ovrPose)
        {
            var pose = new Pose();

            SetBonePositionFromOvrBonePositionLeft(in ovrPose.Position, ref pose.position);
            SetBoneRotationFromOvrBoneRotationLeft(in ovrPose.Orientation, ref pose.rotation);

            return pose;
        }

        public static Pose GetBonePoseFromOvrBonePoseRight(OVRPlugin.Posef ovrPose)
        {
            var pose = new Pose();

            SetBonePositionFromOvrBonePositionRight(in ovrPose.Position, ref pose.position);
            SetBoneRotationFromOvrBoneRotationRight(in ovrPose.Orientation, ref pose.rotation);

            return pose;
        }

        public static void SetBoneRotationFromOvrBoneRotationLeft(
            in OVRPlugin.Quatf ovrBoneRotation,
            ref Quaternion boneRotation)
        {
            boneRotation.x = ovrBoneRotation.z;
            boneRotation.y = -ovrBoneRotation.x;
            boneRotation.z = -ovrBoneRotation.y;
            boneRotation.w = ovrBoneRotation.w;
        }

        public static void SetBoneRotationFromOvrBoneRotationRight(
            in OVRPlugin.Quatf ovrBoneRotation,
            ref Quaternion boneRotation)
        {
            boneRotation.x = ovrBoneRotation.z;
            boneRotation.y = ovrBoneRotation.x;
            boneRotation.z = ovrBoneRotation.y;
            boneRotation.w = ovrBoneRotation.w;
        }

        public static void SetBonePositionFromOvrBonePositionLeft(
            in OVRPlugin.Vector3f ovrBonePosition,
            ref Vector3 bonePosition)
        {
            bonePosition.x = -ovrBonePosition.z;
            bonePosition.y = ovrBonePosition.x;
            bonePosition.z = ovrBonePosition.y;
        }

        public static void SetBonePositionFromOvrBonePositionRight(
            in OVRPlugin.Vector3f ovrBonePosition,
            ref Vector3 bonePosition)
        {
            bonePosition.x = -ovrBonePosition.z;
            bonePosition.y = -ovrBonePosition.x;
            bonePosition.z = -ovrBonePosition.y;
        }

        public static void SetBoneRotationComposedFromOvrConnectedBonesLeft(
            in OVRPlugin.Quatf ovrBoneRotationParent,
            in OVRPlugin.Quatf ovrBoneRotationChild,
            ref Quaternion boneRotation)
        {
            var boneRotationParent = new Quaternion();

            SetBoneRotationFromOvrBoneRotationLeft(in ovrBoneRotationParent, ref boneRotationParent);
            SetBoneRotationFromOvrBoneRotationLeft(in ovrBoneRotationChild, ref boneRotation);

            boneRotation = boneRotationParent * boneRotation;
        }

        public static void SetBoneRotationComposedFromOvrConnectedBonesRight(
            in OVRPlugin.Quatf ovrBoneRotationParent,
            in OVRPlugin.Quatf ovrBoneRotationChild,
            ref Quaternion boneRotation)
        {
            var boneRotationParent = new Quaternion();

            SetBoneRotationFromOvrBoneRotationRight(in ovrBoneRotationParent, ref boneRotationParent);
            SetBoneRotationFromOvrBoneRotationRight(in ovrBoneRotationChild, ref boneRotation);

            boneRotation = boneRotationParent * boneRotation;
        }
    }
}
