using UnityEngine;

namespace Plugins.Lanski
{
    public static class rays
    {
        public static bool try_intersect_sphere(
            /* ray origin    */ Vector3 ro, 
            /* ray direction */ Vector3 rd,
            /* sphere origin */ Vector3 so, 
            /* sphere radius */ float   r,
            /* intersection multiplier */ out float t
        )
        {
            var oc = ro - so;
            var b = Vector3.Dot(oc, rd);
            var c2 = Vector3.Dot(oc, oc) - r.sqr();
            var h = b.sqr() - c2;
            if (h < 0.0f)
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