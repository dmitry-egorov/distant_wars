using UnityEngine;

internal class resize_discovery_texture : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var t = map.DiscoveryTexture;
        var s = map.DiscoveryTextureSize;

        if (t == null || t.width != s)
        {
            if (t != null)
            {
                t.Release();
            }

            t = RenderTexture.GetTemporary(s, s, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
            t.filterMode = FilterMode.Point;
            t.anisoLevel = 0;
            Shader.SetGlobalTexture(_discoveryTex, t);
            map.DiscoveryTexture = t;
            map.DiscoveryCamera.targetTexture = t;
        }
    }

    static readonly int _discoveryTex = Shader.PropertyToID("_DiscoveryTex");
}