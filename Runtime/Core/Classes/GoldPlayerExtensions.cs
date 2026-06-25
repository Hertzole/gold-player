using UnityEngine;

namespace Hertzole.GoldPlayer
{
    public static class GoldPlayerExtensions
    {
        public static bool IsNaN(this float x)
        {
            return float.IsNaN(x);
        }

        public static bool IsNaN(this Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }

        public static bool IsNaN(this Quaternion q)
        {
            return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
        }
    }
}
