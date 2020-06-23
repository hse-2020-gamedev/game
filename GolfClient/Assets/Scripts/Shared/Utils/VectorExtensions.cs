using UnityEngine;

namespace Utils
{
    public static class Vector3Extensions
    {
        public static Vector2 xy(this Vector3 aVector)
        {
            return new Vector2(aVector.x,aVector.y);
        }
        public static Vector2 xz(this Vector3 aVector)
        {
            return new Vector2(aVector.x,aVector.z);
        }
        public static Vector2 yz(this Vector3 aVector)
        {
            return new Vector2(aVector.y,aVector.z);
        }
        public static Vector2 yx(this Vector3 aVector)
        {
            return new Vector2(aVector.y,aVector.x);
        }
        public static Vector2 zx(this Vector3 aVector)
        {
            return new Vector2(aVector.z,aVector.x);
        }
        public static Vector2 zy(this Vector3 aVector)
        {
            return new Vector2(aVector.z,aVector.y);
        }
    }

    public static class Vector2Extensions
    {
        public static float LookAt(this Vector2 position, Vector2 target)
        {
            return -Vector2.SignedAngle(Vector2.up, target - position);
        }
    }
}