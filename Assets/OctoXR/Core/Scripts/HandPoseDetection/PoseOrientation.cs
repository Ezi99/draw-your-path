using System;

namespace OctoXR.HandPoseDetection
{
    [Flags]
    public enum PoseOrientation
    {
        PalmUp = 1,
        PalmDown = PalmUp << 1,
        PalmForward = PalmDown << 1,
        PalmBack = PalmForward << 1,
        PalmRight = PalmBack << 1,
        PalmLeft = PalmRight << 1,
        FingersUp = PalmLeft << 1,
        FingersDown = FingersUp << 1,
        FingersForward = FingersDown << 1,
        FingersRight = FingersForward << 1,
        FingersLeft = FingersRight << 1
    }
}
