using UnityEngine;

namespace Plugins.Lanski.Space
{
    public struct UniformSpaceTransform2
    {
        public UniformSpaceTransform2(float multiplier, Vector2 offset)
        {
            this.multiplier = multiplier;
            this.offset = offset;
        }

        public Vector2 apply_to_point(Vector2 v) => v * multiplier + offset;
        public Vector2 apply_to_direction(Vector2 v) => v * multiplier;
        public float apply_to_scalar(float v) => v * multiplier;

        public readonly float multiplier;
        public readonly Vector2 offset;
    }
}