#if UNITY_EDITOR
using UnityEngine;

internal class editor_setup_map_rendering : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        map.VisionCamera.enabled = false;
        map.DiscoveryCamera.enabled = false;

        Shader.DisableKeyword("SHOW_MAP_GRID");
        Shader.DisableKeyword("SHOW_SPACE_GRID");
        Shader.EnableKeyword("HIDE_MAP_VISION");
    }
}
#endif