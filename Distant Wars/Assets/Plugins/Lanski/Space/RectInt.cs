using UnityEngine;

namespace Plugins.Lanski.Space
{
    public struct IntRect
    {
        public IntRect(Vector2Int min, Vector2Int max)
        {
            Debug.Assert(min.x <= max.x && min.y <= max.y);

            this.min = min;
            this.max = max;
        }
        
        public Vector2Int min;
        public Vector2Int max;

        public bool intersects(in IntRect o) => !(max.x < o.min.x || max.y < o.min.y || o.max.x < min.x || o.max.y < min.y);
        public bool intersects(in Vector2Int omin, in Vector2Int omax) => !(max.x < omin.x || max.y < omin.y || omax.x < min.x || omax.y < min.y);
    }
}