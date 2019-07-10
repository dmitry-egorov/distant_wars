using UnityEngine;

public static class Vector2Ex
{
    public static Vector2 Floor(this Vector2 v) => new Vector2(Mathf.Floor(v.x), Mathf.Floor(v.y));
    public static Vector2 Abs(this Vector2 v) => new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
}