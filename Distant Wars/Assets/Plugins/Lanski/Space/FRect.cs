using UnityEngine;

namespace Plugins.Lanski.Space
{
    public struct Rect
    {
        public Rect(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }
        
        public Vector2 min;
        public Vector2 max;

        public bool intersects(in Rect o) => !(max.x < o.min.x || max.y < o.min.y || o.max.x < min.x || o.max.y < min.y);
        public bool intersects(in Vector2 omin, in Vector2 omax) => !(max.x < omin.x || max.y < omin.y || omax.x < min.x || omax.y < min.y);
    }
}