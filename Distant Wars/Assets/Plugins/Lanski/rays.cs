using UnityEngine;

namespace Plugins.Lanski
{
    public static class rays
    {
        public static bool try_intersect_sphere(
            /* ray origin    */ Vector3 ro,
            /* ray direction */ Vector3 rd,
            /* ray length    */ float   rl,
            /* sphere origin */ Vector3 so,
            /* sphere radius */ float   sr,
            out float t
        )
        {
            t = 0;
            
            /* delta to ray origin       */ var odelta = ro - so;
            /* distance to ray origin ^2 */ var odist2 = odelta.sqrMagnitude;
            if (odist2 > (rl + sr).sqr())
                return false;

            var c2 = odist2 - sr.sqr();
            var b  = Vector3.Dot(odelta, rd);
            var h  = b.sqr() - c2;
            if (h < 0.0f)
                return false;

            h = Mathf.Sqrt(h);
            t = -b - h;
            return t >= 0 && t <= rl;
        }
    }
}