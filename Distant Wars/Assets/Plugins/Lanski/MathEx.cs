using UnityEngine;

namespace Plugins.Lanski
{
    public static class BitEx
    {
        public static void apply(this ref byte b, int mask) => b = (byte)(b | mask);
        public static bool contains(this ref byte b, int mask) => (b & mask) > 0;
        public static bool contains_bit(this ref byte b, int i) => (b & (1 << i)) > 0;
    }

    public static class MathEx
    {
        public static float frac(this float v) => v - Mathf.Round(v);
        public static float sqr(this float v) => v * v;
        public static Vector2 v2(this float v) => new Vector2(v, v);
        public static Vector3 v3(this float v) => new Vector3(v, v, v);

        public const float Root2 = 1.41421356237f;
    }
}