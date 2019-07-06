using UnityEngine;

public static class Vector2ComponentsExtensions
{
    public static Vector2 xy(this Vector3 v3) => new Vector2(v3.x, v3.y);
}