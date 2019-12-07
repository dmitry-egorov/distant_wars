using UnityEngine;

internal class resize_vision_texture : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var t = map.VisionTexture;
        var res = Camera.main.pixelRect;
        var w = (int)res.width;
        var h = (int)res.height;

        if (t == null || t.width != w || t.height != h)
        {
            if (t != null)
            {
                t.Release();
            }

            t = RenderTexture.GetTemporary(w, h, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
            t.filterMode = FilterMode.Point;
            t.anisoLevel = 0;
            Shader.SetGlobalTexture(_visionTex, t);
            map.VisionTexture = t;
            map.VisionCamera.targetTexture = t;
        }
    }

    static readonly int _visionTex = Shader.PropertyToID("_VisionTex");
}