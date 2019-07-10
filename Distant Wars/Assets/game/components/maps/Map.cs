using Plugins.Lanski;
using UnityEngine;

public class Map : RequiredSingleton<Map>
{
    [Header("Common")]
    public TextAsset HeightMap;
    public int Width = 1025;
    public int Height = 1025;
    
    [Header("Visual")]
    public Color TopLandColor;
    public Color BottomLandColor;
    public Color ShallowSeaColor;
    public Color DeepSeaColor;
    public Color ShoreHighColor;
    public Color ShoreLowColor;
    [Range(0, 2)] public float LandColorGamma;
    [Range(0, 2)] public float ShoreColorGamma;
    [Range(0, 1)] public float HeightRange;
    [Range(0, 1)] public float SeaLevel;
    [Range(0, 1)] public float ShoreWidth;
    [Range(0, 0.01f)] public float ShoreBlend;
    [Range(0, 0.01f)] public float SeaBlend;
    [Range(0, 1)] public float LinesIntensity;
    [Range(0, 1)] public float LinesSecondaryIntensity;
    [Range(0, 100)] public float LinesBands;
    [Range(0,  10)] public float LinesSecondaryBands;
    [Range(0, 10)] public float LineWidth;
    [Range(0, 1)] public float LineStrength;
    [Range(0, 1)] public float ShadowIntensity;

    public void UpdateMaterialParameters()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.sharedMaterial;
        if (material == null)
        {
            material = renderer.sharedMaterial = new Material(Shader.Find("DW/Map"));
            material.name = "Height Map";
        }

        var texture = material.GetTexture(_mainTex) as Texture2D;
        if (HeightMap != null)
        {
            if (texture == null)
            {
                texture = new Texture2D(Width, Height, TextureFormat.R16, false, true)
                {
                    name = "Height Map"
                };
            }

            texture.LoadRawTextureData(HeightMap.bytes);
            texture.filterMode = FilterMode.Point;
        }
        else
        {
            texture = null;
        }

        material.SetTexture(_mainTex, texture);
        material.SetColor(_topLandColor, TopLandColor);
        material.SetColor(_bottomLandColor, BottomLandColor);
        material.SetColor(_shallowSeaColor, ShallowSeaColor);
        material.SetColor(_deepSeaColor, DeepSeaColor);
        material.SetColor(_shoreHighColor, ShoreHighColor);
        material.SetColor(_shoreLowColor, ShoreLowColor);
        material.SetFloat(_landColorGamma, LandColorGamma);
        material.SetFloat(_shoreColorGamma, ShoreColorGamma);
        material.SetFloat(_heightRange, HeightRange);
        material.SetFloat(_seaLevel, SeaLevel);
        material.SetFloat(_shoreWidth, ShoreWidth);
        material.SetFloat(_shoreBlend, ShoreBlend);
        material.SetFloat(_seaBlend, SeaBlend);
        material.SetFloat(_linesIntensity, LinesIntensity);
        material.SetFloat(_linesSecondaryIntensity, LinesSecondaryIntensity);
        material.SetFloat(_linesBands, LinesBands);
        material.SetFloat(_linesSecondaryBands, LinesSecondaryBands);
        material.SetFloat(_lineWidth, LineWidth);
        material.SetFloat(_lineStrength, LineStrength);
        material.SetFloat(_shadowIntensity, ShadowIntensity);
    }
    
    static readonly int _mainTex = Shader.PropertyToID("_MainTex");
    static readonly int _topLandColor = Shader.PropertyToID("_TopLandColor");
    static readonly int _bottomLandColor = Shader.PropertyToID("_BottomLandColor");
    static readonly int _shallowSeaColor = Shader.PropertyToID("_ShallowSeaColor");
    static readonly int _deepSeaColor = Shader.PropertyToID("_DeepSeaColor");
    static readonly int _shoreHighColor = Shader.PropertyToID("_ShoreHighColor");
    static readonly int _shoreLowColor = Shader.PropertyToID("_ShoreLowColor");
    static readonly int _landColorGamma = Shader.PropertyToID("_LandColorGamma");
    static readonly int _shoreColorGamma = Shader.PropertyToID("_ShoreColorGamma");
    static readonly int _heightRange = Shader.PropertyToID("_HeightRange");
    static readonly int _seaLevel = Shader.PropertyToID("_SeaLevel");
    static readonly int _shoreWidth = Shader.PropertyToID("_ShoreWidth");
    static readonly int _shoreBlend = Shader.PropertyToID("_ShoreBlend");
    static readonly int _seaBlend = Shader.PropertyToID("_SeaBlend");
    static readonly int _linesIntensity = Shader.PropertyToID("_LinesIntensity");
    static readonly int _linesSecondaryIntensity = Shader.PropertyToID("_LinesSecondaryIntensity");
    static readonly int _linesBands = Shader.PropertyToID("_LinesBands");
    static readonly int _linesSecondaryBands = Shader.PropertyToID("_LinesSecondaryBands");
    static readonly int _lineWidth = Shader.PropertyToID("_LineWidth");
    static readonly int _lineStrength = Shader.PropertyToID("_LineStrength");
    static readonly int _shadowIntensity = Shader.PropertyToID("_ShadowIntensity");
}
