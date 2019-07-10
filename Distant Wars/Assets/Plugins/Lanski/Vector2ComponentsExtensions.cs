using UnityEngine;

public static class Vector2ComponentsExtensions
{
    public static Vector2 xy(this Vector3 v3) => new Vector2(v3.x, v3.y);
    public static Vector3 xy(this Vector2 v2, float z) => new Vector3(v2.x, v2.y, z);
    public static Vector3 xy0(this Vector2 v2) => new Vector3(v2.x, v2.y, 0);
    public static Vector3 xy1(this Vector2 v2) => new Vector3(v2.x, v2.y, 1);
}