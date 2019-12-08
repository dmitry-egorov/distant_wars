using UnityEngine;

namespace Plugins.Lanski
{
    public static class rays
    {
        public static bool try_intersect_sphere
        (
            Vector3 ro, // ray origin
            Vector3 rd, // ray direction
            Vector3 so, // sphere origin
            float   r,  // sphere radius
            out float t // intersection multiplier
        )
        {
            var oc = ro - so;
            var b = Vector3.Dot(oc, rd);
            var c2 = Vector3.Dot(oc, oc) - r.sqr();
            var h = b.sqr() - c2;
            if (h < 0.0)
            {
                t = 0.0f;
                return false;
            }

            h = Mathf.Sqrt(h);
            t = -b - h;
            return true;
        }
    }
}