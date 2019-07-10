using UnityEngine;

namespace Plugins.Lanski
{
    public static class ScreenEx
    {
        public static Vector2Int Size => (Screen.width, Screen.height).v2();
    }
}