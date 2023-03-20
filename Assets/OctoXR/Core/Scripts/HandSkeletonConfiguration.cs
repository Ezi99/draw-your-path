using OctoXR.Collections;
using System;
using System.Linq;

namespace OctoXR
{
    /// <summary>
    /// Provides constant values for a standard configuration of a hand skeleton and its bones 
    /// </summary>
    public static class HandSkeletonConfiguration
    {
        public const int BoneCount = (int)HandBoneId.PinkyFingerTip + 1;
        public const HandBoneId FirstBoneId = HandBoneId.WristRoot;
        public const HandBoneId FirstFingerBoneId = HandBoneId.ThumbFingerMetacarpal;
        public const HandBoneId LastBoneId = HandBoneId.PinkyFingerTip;

        public const int FingerCount = 5;
        public const int BonesPerFinger = 4;
        public const int RotatingBonesPerFinger = 3;

        public static readonly ReadOnlyCollection<HandBoneId> Bones = new ReadOnlyCollection<HandBoneId>(
            Enum.GetValues(typeof(HandBoneId)).OfType<HandBoneId>().ToArray());

        public static readonly HandBoneKeyedReadOnlyCollection<HandBoneId?> ParentBones = 
            new HandBoneKeyedReadOnlyCollection<HandBoneId?>(
                new HandBoneId?[]
                {
                    null,
                    HandBoneId.WristRoot,
                    HandBoneId.ThumbFingerMetacarpal,
                    HandBoneId.ThumbFingerProximalPhalanx,
                    HandBoneId.WristRoot,
                    HandBoneId.IndexFingerProximalPhalanx,
                    HandBoneId.IndexFingerMiddlePhalanx,
                    HandBoneId.WristRoot,
                    HandBoneId.MiddleFingerProximalPhalanx,
                    HandBoneId.MiddleFingerMiddlePhalanx,
                    HandBoneId.WristRoot,
                    HandBoneId.RingFingerProximalPhalanx,
                    HandBoneId.RingFingerMiddlePhalanx,
                    HandBoneId.WristRoot,
                    HandBoneId.PinkyFingerProximalPhalanx,
                    HandBoneId.PinkyFingerMiddlePhalanx,
                    HandBoneId.ThumbFingerDistalPhalanx,
                    HandBoneId.IndexFingerDistalPhalanx,
                    HandBoneId.MiddleFingerDistalPhalanx,
                    HandBoneId.RingFingerDistalPhalanx,
                    HandBoneId.PinkyFingerDistalPhalanx
                });

        public static readonly HandBoneKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>> ChildBones =
            new HandBoneKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>>(
                new ReadOnlyCollection<HandBoneId>[BoneCount]
                {
                    // WristRoot
                    new ReadOnlyCollection<HandBoneId>(
                        new HandBoneId[]
                        {
                            HandBoneId.ThumbFingerMetacarpal,
                            HandBoneId.IndexFingerProximalPhalanx,
                            HandBoneId.MiddleFingerProximalPhalanx,
                            HandBoneId.RingFingerProximalPhalanx,
                            HandBoneId.PinkyFingerProximalPhalanx
                        }),

                    // ThumbMetacarpal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.ThumbFingerProximalPhalanx }),
                    // ThumbProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.ThumbFingerDistalPhalanx }),
                    // ThumbDistal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.ThumbFingerTip }),

                    // IndexProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.IndexFingerMiddlePhalanx }),
                    // IndexMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.IndexFingerDistalPhalanx }),
                    // IndexDistal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.IndexFingerTip }),

                    // MiddleProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.MiddleFingerMiddlePhalanx }),
                    // MiddleMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.MiddleFingerDistalPhalanx }),
                    // MiddleDistal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.MiddleFingerTip }),

                    // RingProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.RingFingerMiddlePhalanx }),
                    // RingMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.RingFingerDistalPhalanx }),
                    // RingDistal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.RingFingerTip }),

                    // PinkyProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.PinkyFingerMiddlePhalanx }),
                    // PinkyMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.PinkyFingerDistalPhalanx }),
                    // PinkyDistal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.PinkyFingerTip }),

                    // ThumbTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // IndexTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // MiddleTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // RingTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // PinkyTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>())
                });

        public static readonly HandBoneKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>> ChildRotatingBones =
            new HandBoneKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>>(
                new ReadOnlyCollection<HandBoneId>[BoneCount]
                {
                    // WristRoot
                    new ReadOnlyCollection<HandBoneId>(
                        new HandBoneId[]
                        {
                            HandBoneId.ThumbFingerMetacarpal,
                            HandBoneId.IndexFingerProximalPhalanx,
                            HandBoneId.MiddleFingerProximalPhalanx,
                            HandBoneId.RingFingerProximalPhalanx,
                            HandBoneId.PinkyFingerProximalPhalanx
                        }),

                    // ThumbMetacarpal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.ThumbFingerProximalPhalanx }),
                    // ThumbProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.ThumbFingerDistalPhalanx }),
                    // ThumbDistal
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),

                    // IndexProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.IndexFingerMiddlePhalanx }),
                    // IndexMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.IndexFingerDistalPhalanx }),
                    // IndexDistal
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),

                    // MiddleProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.MiddleFingerMiddlePhalanx }),
                    // MiddleMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.MiddleFingerDistalPhalanx }),
                    // MiddleDistal
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),

                    // RingProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.RingFingerMiddlePhalanx }),
                    // RingMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.RingFingerDistalPhalanx }),
                    // RingDistal
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),

                    // PinkyProximal
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.PinkyFingerMiddlePhalanx }),
                    // PinkyMiddle
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]{ HandBoneId.PinkyFingerDistalPhalanx }),
                    // PinkyDistal
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),

                    // ThumbTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // IndexTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // MiddleTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // RingTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>()),
                    // PinkyTip
                    new ReadOnlyCollection<HandBoneId>(Array.Empty<HandBoneId>())
                });

        public static readonly ReadOnlyCollection<HandFinger> Fingers = new ReadOnlyCollection<HandFinger>(
            Enum.GetValues(typeof(HandFinger)).OfType<HandFinger>().ToArray());

        public static readonly HandFingerKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>> FingerBones =
            new HandFingerKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>>(
                new ReadOnlyCollection<HandBoneId>[]
                {
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.ThumbFingerMetacarpal,
                        HandBoneId.ThumbFingerProximalPhalanx,
                        HandBoneId.ThumbFingerDistalPhalanx,
                        HandBoneId.ThumbFingerTip
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.IndexFingerProximalPhalanx,
                        HandBoneId.IndexFingerMiddlePhalanx,
                        HandBoneId.IndexFingerDistalPhalanx,
                        HandBoneId.IndexFingerTip
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.MiddleFingerProximalPhalanx,
                        HandBoneId.MiddleFingerMiddlePhalanx,
                        HandBoneId.MiddleFingerDistalPhalanx,
                        HandBoneId.MiddleFingerTip
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.RingFingerProximalPhalanx,
                        HandBoneId.RingFingerMiddlePhalanx,
                        HandBoneId.RingFingerDistalPhalanx,
                        HandBoneId.RingFingerTip
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.PinkyFingerProximalPhalanx,
                        HandBoneId.PinkyFingerMiddlePhalanx,
                        HandBoneId.PinkyFingerDistalPhalanx,
                        HandBoneId.PinkyFingerTip
                    })
                });

        public static readonly HandFingerKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>> FingerRotatingBones =
            new HandFingerKeyedReadOnlyCollection<ReadOnlyCollection<HandBoneId>>(
                new ReadOnlyCollection<HandBoneId>[]
                {
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.ThumbFingerMetacarpal,
                        HandBoneId.ThumbFingerProximalPhalanx,
                        HandBoneId.ThumbFingerDistalPhalanx
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.IndexFingerProximalPhalanx,
                        HandBoneId.IndexFingerMiddlePhalanx,
                        HandBoneId.IndexFingerDistalPhalanx
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.MiddleFingerProximalPhalanx,
                        HandBoneId.MiddleFingerMiddlePhalanx,
                        HandBoneId.MiddleFingerDistalPhalanx
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.RingFingerProximalPhalanx,
                        HandBoneId.RingFingerMiddlePhalanx,
                        HandBoneId.RingFingerDistalPhalanx
                    }),
                    new ReadOnlyCollection<HandBoneId>(new HandBoneId[]
                    {
                        HandBoneId.PinkyFingerProximalPhalanx,
                        HandBoneId.PinkyFingerMiddlePhalanx,
                        HandBoneId.PinkyFingerDistalPhalanx
                    })
                });

        /// <summary>
        /// Indicates whether the specified hand bone is part of a hand finger
        /// </summary>
        /// <returns></returns>
        public static bool IsFingerBone(HandBoneId bone) => bone != HandBoneId.WristRoot;

        /// <summary>
        /// Indicates whether the specified hand bone is one of the bones that rotate depending on the poses obtained from a backing 
        /// hand tracking data provider. In practice, bone that specifies wrist root or a finger tip never rotates
        /// </summary>
        /// <returns></returns>
        public static bool IsRotatingBone(HandBoneId bone) => bone != HandBoneId.WristRoot && bone < HandBoneId.ThumbFingerTip;

        /// <summary>
        /// Indicates whether the specified hand bone is a finger tip bone
        /// </summary>
        /// <returns></returns>
        public static bool IsFingerTipBone(HandBoneId bone) => bone > HandBoneId.PinkyFingerDistalPhalanx;

        /// <summary>
        /// Returns a finger that the specified bone is part of. If the bone is not a finger bone, null is returned
        /// </summary>
        /// <returns></returns>
        public static HandFinger? GetBoneFinger(HandBoneId bone)
        {
            if (bone == HandBoneId.WristRoot)
            {
                return null;
            }

            if (bone < HandBoneId.ThumbFingerTip)
            {
                return (HandFinger)(((int)bone - 1) / RotatingBonesPerFinger);
            }

            return (HandFinger)(bone - HandBoneId.ThumbFingerTip);
        }
    }
}
