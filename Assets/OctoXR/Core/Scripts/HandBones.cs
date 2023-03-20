using System;

namespace OctoXR
{
    [Flags]
    public enum HandBones
    {
        None = 0,
        WristRoot = 1 << HandBoneId.WristRoot,
        ThumbFingerMetacarpal = 1 << HandBoneId.ThumbFingerMetacarpal,
        ThumbFingerProximalPhalanx = 1 << HandBoneId.ThumbFingerProximalPhalanx,
        ThumbFingerDistalPhalanx = 1 << HandBoneId.ThumbFingerDistalPhalanx,
        IndexFingerProximalPhalanx = 1 << HandBoneId.IndexFingerProximalPhalanx,
        IndexFingerMiddlePhalanx = 1 << HandBoneId.IndexFingerMiddlePhalanx,
        IndexFingerDistalPhalanx = 1 << HandBoneId.IndexFingerDistalPhalanx,
        MiddleFingerProximalPhalanx = 1 << HandBoneId.MiddleFingerProximalPhalanx,
        MiddleFingerMiddlePhalanx = 1 << HandBoneId.MiddleFingerMiddlePhalanx,
        MiddleFingerDistalPhalanx = 1 << HandBoneId.MiddleFingerDistalPhalanx,
        RingFingerProximalPhalanx = 1 << HandBoneId.RingFingerProximalPhalanx,
        RingFingerMiddlePhalanx = 1 << HandBoneId.RingFingerMiddlePhalanx,
        RingFingerDistalPhalanx = 1 << HandBoneId.RingFingerDistalPhalanx,
        PinkyFingerProximalPhalanx = 1 << HandBoneId.PinkyFingerProximalPhalanx,
        PinkyFingerMiddlePhalanx = 1 << HandBoneId.PinkyFingerMiddlePhalanx,
        PinkyFingerDistalPhalanx = 1 << HandBoneId.PinkyFingerDistalPhalanx,
        ThumbFingerTip = 1 << HandBoneId.ThumbFingerTip,
        IndexFingerTip = 1 << HandBoneId.IndexFingerTip,
        MiddleFingerTip = 1 << HandBoneId.MiddleFingerTip,
        RingFingerTip = 1 << HandBoneId.RingFingerTip,
        PinkyFingerTip = 1 << HandBoneId.PinkyFingerTip,
        All = int.MaxValue >> (30 - HandBoneId.PinkyFingerTip),
        RotatingBones = All & ~(ThumbFingerTip | IndexFingerTip | MiddleFingerTip | RingFingerTip | PinkyFingerTip),
        FingerBones = All & ~WristRoot,
        RotatingFingerBones = RotatingBones & ~WristRoot
    }
}
