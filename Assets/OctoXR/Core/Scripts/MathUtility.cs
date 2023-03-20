using System;
using UnityEngine;

namespace OctoXR
{
	public enum EulerAnglesSequence
	{
        XYX = 0b00000100,
        XYZ = 0b00100100,
        XZX = 0b00001000,
        XZY = 0b00011000,
        YXY = 0b00010001,
        YXZ = 0b00100001,
        YZX = 0b00001001,
        YZY = 0b00011001,
        ZXY = 0b00010010,
        ZXZ = 0b00100010,
        ZYX = 0b00000110,
        ZYZ = 0b00100110
	}

    public struct Angles
    {
        public float Angle1;
        public float Angle2;
        public float Angle3;

        public Angles(float angle1, float angle2, float angle3)
        {
            Angle1 = angle1;
            Angle2 = angle2;
            Angle3 = angle3;
        }

        public override string ToString()
        {
            return $"({Angle1}, {Angle2}, {Angle3})";
        }

		public Quaternion ToQuaternion(EulerAnglesSequence sequence)
		{
			Vector3 axis1, axis2, axis3;

			if (sequence == EulerAnglesSequence.XYX)
			{
				axis1 = Vector3.right;
				axis2 = Vector3.up;
				axis3 = axis1;
			}
			else if (sequence == EulerAnglesSequence.XYZ)
			{
				axis1 = Vector3.right;
				axis2 = Vector3.up;
				axis3 = Vector3.forward;
			}
			else if (sequence == EulerAnglesSequence.XZX)
			{
				axis1 = Vector3.right;
				axis2 = Vector3.forward;
				axis3 = axis1;
			}
			else if (sequence == EulerAnglesSequence.XZY)
			{
				axis1 = Vector3.right;
				axis2 = Vector3.forward;
				axis3 = Vector3.up;
			}
			else if (sequence == EulerAnglesSequence.YXY)
			{
				axis1 = Vector3.up;
				axis2 = Vector3.right;
				axis3 = axis1;
			}
			else if (sequence == EulerAnglesSequence.YXZ)
			{
				axis1 = Vector3.up;
				axis2 = Vector3.right;
				axis3 = Vector3.forward;
			}
			else if (sequence == EulerAnglesSequence.YZX)
			{
				axis1 = Vector3.up;
				axis2 = Vector3.forward;
				axis3 = Vector3.right;
			}
			else if (sequence == EulerAnglesSequence.YZY)
			{
				axis1 = Vector3.up;
				axis2 = Vector3.forward;
				axis3 = axis1;
			}
			else if (sequence == EulerAnglesSequence.ZXY)
			{
				axis1 = Vector3.forward;
				axis2 = Vector3.right;
				axis3 = Vector3.up;
			}
			else if (sequence == EulerAnglesSequence.ZXZ)
			{
				axis1 = Vector3.forward;
				axis2 = Vector3.right;
				axis3 = axis1;
			}
			else if (sequence == EulerAnglesSequence.ZYX)
			{
				axis1 = Vector3.forward;
				axis2 = Vector3.up;
				axis3 = Vector3.right;
			}
			else if (sequence == EulerAnglesSequence.ZYZ)
			{
				axis1 = Vector3.forward;
				axis2 = Vector3.up;
				axis3 = axis1;
			}
			else
			{
				throw new ArgumentException("Specified Euler angles sequence is not supported.");
			}

			return Quaternion.AngleAxis(Angle1, axis1) *
				Quaternion.AngleAxis(Angle2, axis2) *
				Quaternion.AngleAxis(Angle3, axis3);
		}
    }

    public struct EulerAnglesRotation
    {
        public Angles Angles;
        public Angles SecondaryAngles;
        public EulerAnglesSequence Sequence;

        public EulerAnglesRotation(Angles angles, Angles secondaryAngles, EulerAnglesSequence sequence)
        {
            Angles = angles;
            SecondaryAngles = secondaryAngles;
            Sequence = sequence;
        }

        public override string ToString()
        {
            return $"{Sequence}: {Angles} or {SecondaryAngles}";
        }
    }
	
    public static class MathUtility
    {
        /// <summary>
        /// Returns an arbitrary non-normalized vector that is perpendicular to the specified vector
        /// </summary>
        /// <param name="fromVector"></param>
        /// <returns></returns>
        public static Vector3 GetPerpendicularVector(in Vector3 fromVector)
        {
            GetPerpendicularVector(in fromVector, out var result);

            return result;
        }

        /// <summary>
        /// Returns an arbitrary non-normalized vector that is perpendicular to the specified vector
        /// </summary>
        /// <param name="fromVector"></param>
        /// <returns></returns>
        public static void GetPerpendicularVector(in Vector3 fromVector, out Vector3 result)
        {
            float x = 0, y = 0, z = 0;

            if (!Mathf.Approximately(fromVector.x, 0f))
            {
                y = z = 1;
                x = -(fromVector.y + fromVector.z) / fromVector.x;
            }
            else if (!Mathf.Approximately(fromVector.y, 0f))
            {
                x = z = 1;
                y = -(fromVector.x + fromVector.z) / fromVector.y;
            }
            else if (!Mathf.Approximately(fromVector.z, 0f))
            {
                x = y = 1;
                z = -(fromVector.x + fromVector.y) / fromVector.z;
            }

            result = new Vector3(x, y, z);
        }

        public static float AngleToRangeMinus180To180(float degrees)
        {
            if (degrees > 180)
            {
                degrees -= 360;
            }
            else if (degrees < -180)
            {
                degrees += 360;
            }
            
            return degrees;
        }

        #region EXTENSIONS

        #region POSE-RELATED

        public static Pose Inverted(in this Pose pose)
        {
            Inverted(in pose, out var poseInverse);

            return poseInverse;
        }

        public static void Inverted(in this Pose pose, out Pose invertedPose)
        {
            var rotationInverse = Quaternion.Inverse(pose.rotation);

            invertedPose.position = rotationInverse * (-pose.position);
            invertedPose.rotation = rotationInverse;
        }

        public static Pose TransformedBy(in this Pose source, in Pose pose)
        {
            TransformedBy(in source, in pose, out var transformedPose);

            return transformedPose;
        }

        public static void TransformedBy(in this Pose source, in Pose pose, out Pose transformedPose)
        {
            transformedPose.position = pose.position + pose.rotation * source.position;
            transformedPose.rotation = pose.rotation * source.rotation;
        }

        public static Pose TransformedBy(in this Pose source, Transform transform)
        {
            TransformedBy(in source, transform, out var transformedPose);

            return transformedPose;
        }

        public static void TransformedBy(in this Pose source, Transform transform, out Pose transformedPose)
        {
            if (!transform)
            {
                transformedPose = source;
                return;
            }

            transformedPose.position = transform.TransformPoint(source.position);
            transformedPose.rotation = transform.rotation * source.rotation;
        }

        public static Pose RelativeTo(in this Pose pose, in Pose relativeToPose)
        {
            RelativeTo(in pose, in relativeToPose, out var relativePose);

            return relativePose;
        }

        public static void RelativeTo(in this Pose pose, in Pose relativeToPose, out Pose relativePose)
        {
            var parentRotationInverse = Quaternion.Inverse(relativeToPose.rotation);

            relativePose.position = parentRotationInverse * (pose.position - relativeToPose.position);
            relativePose.rotation = parentRotationInverse * pose.rotation;
        }

        public static Pose RelativeTo(in this Pose pose, Transform relativeToTransform)
        {
            RelativeTo(in pose, relativeToTransform, out var relativePose);

            return relativePose;
        }

        public static void RelativeTo(in this Pose pose, Transform relativeToTransform, out Pose relativePose)
        {
            if (!relativeToTransform)
            {
                relativePose = Pose.identity;
                return;
            }

            var parentRotationInverse = Quaternion.Inverse(relativeToTransform.rotation);

            relativePose.position = relativeToTransform.InverseTransformPoint(pose.position);
            relativePose.rotation = parentRotationInverse * pose.rotation;
        }

        public static Vector3 RelativeTo(in this Vector3 position, in Pose relativeToPose)
        {
            RelativeTo(in position, in relativeToPose, out var relativePosition);

            return relativePosition;
        }

        public static void RelativeTo(in this Vector3 position, in Pose relativeToPose, out Vector3 relativePosition)
        {
            var parentRotationInverse = Quaternion.Inverse(relativeToPose.rotation);

            relativePosition = parentRotationInverse * (position - relativeToPose.position);
        }

        public static Vector3 TransformedBy(in this Vector3 position, in Pose pose)
        {
            TransformedBy(in position, in pose, out var transformedPosition);

            return transformedPosition;
        }

        public static void TransformedBy(in this Vector3 position, in Pose pose, out Vector3 transformedPosition)
        {
            transformedPosition = pose.position + (pose.rotation * position);
        }

        #endregion

        #region QUATERNION

        public static Quaternion RelativeTo(in this Quaternion rotation, in Quaternion relativeToRotation)
        {
            RelativeTo(in rotation, in relativeToRotation, out var relativeRotation);

            return relativeRotation;
        }

        public static void RelativeTo(in this Quaternion rotation, in Quaternion relativeToRotation, out Quaternion relativeRotation)
        {
            var parentRotationInverse = Quaternion.Inverse(relativeToRotation);

            relativeRotation = parentRotationInverse * rotation;
        }

        public static EulerAnglesRotation ToEulerAngles(in this Quaternion q, EulerAnglesSequence sequence)
        {
            var xx = q.x * q.x;
            var xy = q.x * q.y;
            var xz = q.x * q.z;
            var xw = q.x * q.w;
            var yy = q.y * q.y;
            var yz = q.y * q.z;
            var yw = q.y * q.w;
            var zz = q.z * q.z;
            var zw = q.z * q.w;

            float angle11, angle12, angle13;
            float angle22;

            if (sequence == EulerAnglesSequence.XYX)
            {
                var cos = 1 - 2 * (yy + zz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(xy + zw, yw - xz);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(xy - zw, xz + yw);
                angle22 = -angle12;
            }
            else if (sequence == EulerAnglesSequence.XYZ)
            {
                var sin = 2 * (xz + yw);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xw - yz), 1 - 2 * (xx + yy));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (zw - xy), 1 - 2 * (yy + zz));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.XZX)
            {
                var cos = 1 - 2 * (yy + zz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(xz - yw, xy + zw);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(xz + yw, zw - xy);
                angle22 = -angle12;
            }
            else if (sequence == EulerAnglesSequence.XZY)
            {
                var sin = 2 * (zw - xy);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (yz + xw), 1 - 2 * (xx + zz));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xz + yw), 1 - 2 * (yy + zz));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.YXY)
            {
                var cos = 1 - 2 * (xx + zz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(xy - zw, yz + xw);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(xy + zw, xw - yz);
                angle22 = -angle12;
            }
            else if (sequence == EulerAnglesSequence.YXZ)
            {
                var sin = 2 * (xw - yz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xz + yw), 1 - 2 * (xx + yy));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xy + zw), 1 - 2 * (xx + zz));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.YZX)
            {
                var sin = 2 * (xy + zw);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (yw - xz), 1 - 2 * (yy + zz));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xw - yz), 1 - 2 * (xx + zz));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.YZY)
            {
                var cos = 1 - 2 * (xx + zz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(yz + xw, zw - xy);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(yz - xw, xy + zw);
                angle22 = -angle12;
            }
            else if (sequence == EulerAnglesSequence.ZXY)
            {
                var sin = 2 * (yz + xw);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (zw - xy), 1 - 2 * (xx + zz));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (yw - xz), 1 - 2 * (xx + yy));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.ZXZ)
            {
                var cos = 1 - 2 * (xx + yy);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(xz + yw, xw - yz);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(xz - yw, yz + xw);
                angle22 = -angle12;
            }
            else if (sequence == EulerAnglesSequence.ZYX)
            {
                var sin = 2 * (yw - xz);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(2 * (xy + zw), 1 - 2 * (yy + zz));
                angle12 = Mathf.Abs(sin) > 1 ? Mathf.Sign(sin) * 90 : Mathf.Rad2Deg * Mathf.Asin(sin);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(2 * (yz + xw), 1 - 2 * (xx + yy));
                angle22 = Mathf.Sign(angle12) * 180 - angle12;
            }
            else if (sequence == EulerAnglesSequence.ZYZ)
            {
                var cos = 1 - 2 * (xx + yy);

                angle11 = Mathf.Rad2Deg * Mathf.Atan2(yz - xw, xz + yw);
                angle12 = Mathf.Abs(cos) > 1 ? 90 - Mathf.Sign(cos) * 90 : Mathf.Rad2Deg * Mathf.Acos(cos);
                angle13 = Mathf.Rad2Deg * Mathf.Atan2(yz + xw, yw - xz);
                angle22 = -angle12;
            }
            else
            {
                throw new ArgumentException("Specified Euler angles sequence is not supported.");
            }

            return new EulerAnglesRotation(
                new Angles(angle11, angle12, angle13),
                new Angles(
                    -Mathf.Sign(angle11) * 180 + angle11,
                    angle22,
                    -Mathf.Sign(angle13) * 180 + angle13),
                sequence);
        }

        #endregion

        #endregion
    }
}
