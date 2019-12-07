using Plugins.Lanski;
using UnityEngine;

public class Map : RequiredSingleton<Map>
{
    [Header("Settings")]
    public TextAsset HeightMap;
    public int Width = 1025;
    public int Height = 1025;
    public float Scale = 8000;
    public Camera VisionCamera;
    public Camera DiscoveryCamera;
    public int DiscoveryTextureSize;

    [Header("State")]
    public Texture2D map_texture;
    public RenderTexture VisionTexture;
    public RenderTexture DiscoveryTexture;

    public void reload()
    {
        if (map_texture == null)
        {
            map_texture = new Texture2D(Width, Height, TextureFormat.R16, false, true)
            {
                name = "Height Map"
            };
        }

        if (HeightMap != null)
        {
            map_texture.LoadRawTextureData(HeightMap.bytes);
        }

        map_texture.filterMode = FilterMode.Point;
        map_texture.anisoLevel = 0;
        map_texture.Apply();

        Shader.SetGlobalTexture(_mapTex, map_texture);
    }

    public void initialize()
    {
        reload();

        transform.localScale = Scale.v3();
        map_data = HeightMap.bytes;
        cell_size = Scale / Width;
        width = Width;
        height = Height;
        scale = Scale;
    }

    public float slope(Vector2 position, Vector2 direction) => slope2(position, direction.normalized);

    public float slope2(Vector2 position, Vector2 normalized_direction)
    {
        var p = position;
        var nd = normalized_direction;

        var coord = coord_of(p);
        var offset = new Vector2Int(Mathf.RoundToInt(nd.x), Mathf.RoundToInt(nd.y));
        var ocoord = coord + offset;
        var h = height_at(coord);
        var ho = height_at(ocoord);

        return (ho - h) / cell_size;
    }

    public float height_at(Vector2 position)
    {
        var coord = coord_of(position);
        return height_at(coord);
    }

    public Vector2Int coord_of(Vector2 position)
    {
        var p = position;
        return new Vector2Int(width / 2 + Mathf.RoundToInt(p.x / cell_size), height / 2 + Mathf.RoundToInt(p.y / cell_size));
    }

    public float height_at(Vector2Int coord)
    {
        var x = coord.x;
        var y = coord.y;
        var w = width;
        var h = height;

        if (x < 0 || x >= width || y < 0 || y >= height) return 0;
        
        var i = 2 * (w * y + x);
        return ((map_data[i]) + (map_data[i + 1] << 8)) / (float)ushort.MaxValue;
    }

    private byte[] map_data;
    private float cell_size;
    private int width;
    private int height;
    private float scale;

    static readonly int _mapTex                  = Shader.PropertyToID("_MapTex");
    static readonly int _topLandColor            = Shader.PropertyToID("_TopLandColor");
    static readonly int _bottomLandColor         = Shader.PropertyToID("_BottomLandColor");
    static readonly int _shallowSeaColor         = Shader.PropertyToID("_ShallowSeaColor");
    static readonly int _deepSeaColor            = Shader.PropertyToID("_DeepSeaColor");
    static readonly int _shoreHighColor          = Shader.PropertyToID("_ShoreHighColor");
    static readonly int _shoreLowColor           = Shader.PropertyToID("_ShoreLowColor");
    static readonly int _seaColorGamma           = Shader.PropertyToID("_SeaColorGamma");
    static readonly int _landColorGamma          = Shader.PropertyToID("_LandColorGamma");
    static readonly int _shoreColorGamma         = Shader.PropertyToID("_ShoreColorGamma");
    static readonly int _heightRange             = Shader.PropertyToID("_HeightRange");
    static readonly int _seaLevel                = Shader.PropertyToID("_SeaLevel");
    static readonly int _shoreWidth              = Shader.PropertyToID("_ShoreWidth");
    static readonly int _shoreBlend              = Shader.PropertyToID("_ShoreBlend");
    static readonly int _seaBlend                = Shader.PropertyToID("_SeaBlend");
    static readonly int _linesIntensity          = Shader.PropertyToID("_LinesIntensity");
    static readonly int _linesSecondaryIntensity = Shader.PropertyToID("_LinesSecondaryIntensity");
    static readonly int _linesBands              = Shader.PropertyToID("_LinesBands");
    static readonly int _linesSecondaryBands     = Shader.PropertyToID("_LinesSecondaryBands");
    static readonly int _lineWidth               = Shader.PropertyToID("_LineWidth");
    static readonly int _lineStrength            = Shader.PropertyToID("_LineStrength");
    static readonly int _shadowIntensity         = Shader.PropertyToID("_ShadowIntensity");
    static readonly int _visionBrightness        = Shader.PropertyToID("_VisionBrightness"); 
    static readonly int _visionSaturation        = Shader.PropertyToID("_VisionSaturation");
    static readonly int _visionSharpness         = Shader.PropertyToID("_VisionSharpness");

}
