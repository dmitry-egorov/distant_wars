using UnityEngine;

internal class init_vision_texture : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var t = map.VisionTexture;
        var s = map.VisionTextureSize;

        if (t == null || t.width != s)
        {
            if (t != null)
            {
                t.Release();
            }

            t = RenderTexture.GetTemporary(s, s, 0, UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
            t.filterMode = FilterMode.Point;
            t.anisoLevel = 0;
            Shader.SetGlobalTexture(_visionTex, t);
            map.VisionTexture = t;
            map.VisionCamera.targetTexture = t;
            
            Debug.Log("Vision texture resized");
        }
    }

    static readonly int _visionTex = Shader.PropertyToID("_VisionTex");
}