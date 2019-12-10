using UnityEngine;

namespace Plugins.Lanski.Space
{
    public struct SpaceTransform2
    {
        public SpaceTransform2(Vector2 multiplier, Vector2 offset)
        {
            this.multiplier = multiplier;
            this.offset = offset;
        }

        public Vector2 apply_to_point(Vector2 v) => v * multiplier + offset;
        public Vector2 apply_to_direction(Vector2 v) => v * multiplier;
        public SpaceTransform2 inverse() => new SpaceTransform2(multiplier.mul_invers(), -offset / multiplier);

        public readonly Vector2 multiplier;
        public readonly Vector2 offset;
    }
}