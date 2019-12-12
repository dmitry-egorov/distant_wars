using UnityEngine;

namespace Plugins.Lanski.Space
{
    public struct FRect
    {
        
        public FRect(float minx, float miny, float maxx, float maxy): this(new Vector2(minx, miny), new Vector2(maxx, maxy))
        {}

        public FRect(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public Vector2 min;
        public Vector2 max;

        public Vector2 get_center() => 0.5f * (min + max);

        public Vector2 get_size() => max - min;

        public bool intersects(in FRect o) => !(max.x < o.min.x || max.y < o.min.y || o.max.x < min.x || o.max.y < min.y);
        public bool intersects(in Vector2 omin, in Vector2 omax) => !(max.x < omin.x || max.y < omin.y || omax.x < min.x || omax.y < min.y);

        public FRect wider_by( float o) => new FRect(min - new Vector2(o, o), max + new Vector2(o, o));
    }
}