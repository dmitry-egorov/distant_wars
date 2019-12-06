using Plugins.Lanski;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "DW/MapAsset", order = 1)]
public class MapAsset : ScriptableObject
{
    [Header("Common")]
    public TextAsset HeightMap;
    public int Width = 1025;
    public int Height = 1025;
    public float Scale = 8000;
    
    [Header("Visual")]
    public Color TopLandColor;
    public Color BottomLandColor;
    public Color ShallowSeaColor;
    public Color DeepSeaColor;
    public Color ShoreHighColor;
    public Color ShoreLowColor;
    [Range(0, 2)]     public float LandColorGamma;
    [Range(0, 2)]     public float ShoreColorGamma;
    [Range(0, 8)]     public float SeaColorGamma;
    [Range(0, 1)]     public float HeightRange;
    [Range(0, 1)]     public float SeaLevel;
    [Range(0, 1)]     public float ShoreWidth;
    [Range(0, 0.01f)] public float ShoreBlend;
    [Range(0, 0.01f)] public float SeaBlend;
    [Range(0, 1)]     public float LinesIntensity;
    [Range(0, 1)]     public float LinesSecondaryIntensity;
    [Range(0, 100)]   public float LinesBands;
    [Range(0, 10)]    public float LinesSecondaryBands;
    [Range(0, 10)]    public float LineWidth;
    [Range(0, 1)]     public float LineStrength;
    [Range(0, 1)]     public float ShadowIntensity;
    [Range(0, 1)]     public float VisionBrightness;
    [Range(0, 1)]     public float VisionSaturation;
    [Range(0, 16)]    public float VisionSharpness;

    public void OnValidate() 
    {
        Map.Instance.Reload();
    }
}

public class Map : RequiredSingleton<Map>
{
    public RenderTexture VisionTexture;
    public RenderTexture DiscoveryTexture;
    public MapAsset MapAsset;

    public float Scale => scale;

    public void Reload()
    {
        var ma = MapAsset;
        var renderer = GetComponent<Renderer>();
        var material = renderer.sharedMaterial;
        if (material == null)
        {
            material = renderer.sharedMaterial = new Material(Shader.Find("DW/Map"));
            material.name = "Height Map";
        }

        var texture = material.GetTexture(_mainTex) as Texture2D;
        if (ma.HeightMap != null)
        {
            if (texture == null)
            {
                texture = new Texture2D(ma.Width, ma.Height, TextureFormat.R16, false, true)
                {
                    name = "Height Map"
                };
            }

            texture.LoadRawTextureData(map_data);
            texture.filterMode = FilterMode.Point;
        }
        else
        {
            texture = null;
        }

        material.SetTexture(_mainTex, texture);
        material.SetTexture(_visionTex, VisionTexture);
        material.SetTexture(_discoveryTex, DiscoveryTexture);
        material.SetColor(_topLandColor, ma.TopLandColor);
        material.SetColor(_bottomLandColor, ma.BottomLandColor);
        material.SetColor(_shallowSeaColor, ma.ShallowSeaColor);
        material.SetColor(_deepSeaColor, ma.DeepSeaColor);
        material.SetColor(_shoreHighColor, ma.ShoreHighColor);
        material.SetColor(_shoreLowColor, ma.ShoreLowColor);
        material.SetFloat(_seaColorGamma, ma.SeaColorGamma);
        material.SetFloat(_landColorGamma, ma.LandColorGamma);
        material.SetFloat(_shoreColorGamma, ma.ShoreColorGamma);
        material.SetFloat(_heightRange, ma.HeightRange);
        material.SetFloat(_seaLevel, ma.SeaLevel);
        material.SetFloat(_shoreWidth, ma.ShoreWidth);
        material.SetFloat(_shoreBlend, ma.ShoreBlend);
        material.SetFloat(_seaBlend, ma.SeaBlend);
        material.SetFloat(_linesIntensity, ma.LinesIntensity);
        material.SetFloat(_linesSecondaryIntensity, ma.LinesSecondaryIntensity);
        material.SetFloat(_linesBands, ma.LinesBands);
        material.SetFloat(_linesSecondaryBands, ma.LinesSecondaryBands);
        material.SetFloat(_lineWidth, ma.LineWidth);
        material.SetFloat(_lineStrength, ma.LineStrength);
        material.SetFloat(_shadowIntensity, ma.ShadowIntensity);
        material.SetFloat(_visionBrightness, ma.VisionBrightness);
        material.SetFloat(_visionSaturation, ma.VisionSaturation);
        material.SetFloat(_visionSharpness, ma.VisionSharpness);
    }

    public void initialize()
    {
        var ma = MapAsset;

        transform.localScale = ma.Scale.v3();
        map_data = ma.HeightMap.bytes;
        cell_size = ma.Scale / ma.Width;
        width = ma.Width;
        height = ma.Height;
        scale = ma.Scale;
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

    static readonly int _mainTex                 = Shader.PropertyToID("_MainTex");
    static readonly int _visionTex               = Shader.PropertyToID("_VisionTex");
    static readonly int _discoveryTex            = Shader.PropertyToID("_DiscoveryTex");
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
