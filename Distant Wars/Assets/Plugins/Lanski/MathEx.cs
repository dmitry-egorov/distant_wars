using UnityEngine;

namespace Plugins.Lanski
{
    public static class MathEx
    {
        public static float sqr(this float v) => v * v;
        public static Vector2 v2(this float v) => new Vector2(v, v);
    }
}