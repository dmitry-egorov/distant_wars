using UnityEngine;

public class Map : MonoBehaviour
{
    public TextAsset HeightMap;
    public Color TopLandColor;
    public Color BottomLandColor;
    public Color SeaColor;
    public Color DeepSeaColor;
    [Range(0, 2)] public float ColorGamma;
    [Range(0, 1)] public float HeightRange;
    [Range(0, 1)] public float SeaLevel;
    [Range(0, 0.01f)] public float SeaBlend;
    [Range(0, 1)] public float LinesIntensity;
    [Range(0, 100)] public float LinesBands;
    [Range(0, 0.05f)] public float LineWidth;
    [Range(0, 1)] public float LineStrength;
    [Range(0, 1)] public float ShadowRange;
    [Range(0, 1)] public float ShadowIntensity;
    
    static readonly int _mainTex = Shader.PropertyToID("_MainTex");
    static readonly int _topLandColor = Shader.PropertyToID("_TopLandColor");
    static readonly int _bottomLandColor = Shader.PropertyToID("_BottomLandColor");
    static readonly int _seaColor = Shader.PropertyToID("_SeaColor");
    static readonly int _deepSeaColor = Shader.PropertyToID("_DeepSeaColor");
    static readonly int _colorGamma = Shader.PropertyToID("_ColorGamma");
    static readonly int _heightRange = Shader.PropertyToID("_HeightRange");
    static readonly int _seaLevel = Shader.PropertyToID("_SeaLevel");
    static readonly int _seaBlend = Shader.PropertyToID("_SeaBlend");
    static readonly int _linesIntensity = Shader.PropertyToID("_LinesIntensity");
    static readonly int _linesBands = Shader.PropertyToID("_LinesBands");
    static readonly int _lineWidth = Shader.PropertyToID("_LineWidth");
    static readonly int _lineStrength = Shader.PropertyToID("_LineStrength");
    static readonly int _shadowRange = Shader.PropertyToID("_ShadowRange");
    static readonly int _shadowIntensity = Shader.PropertyToID("_ShadowIntensity");


    public void UpdateMaterialParameters()
    {
        var renderer = GetComponent<Renderer>();
        var material = renderer.sharedMaterial;
        if (material == null)
        {
            material = renderer.sharedMaterial = new Material(Shader.Find("DW/Map"));
            material.name = "Height Map";
        }

        var texture = (Texture2D)material.GetTexture(_mainTex);
        if (HeightMap != null)
        {
            if (texture == null)
            {
                texture = new Texture2D(1025, 1025, TextureFormat.R16, false, true)
                {
                    name = "Height Map"
                };
            }

            texture.LoadRawTextureData(HeightMap.bytes);
            texture.filterMode = FilterMode.Bilinear;
        }
        else
        {
            texture = null;
        }

        material.SetTexture(_mainTex, texture);
        material.SetColor(_topLandColor, TopLandColor);
        material.SetColor(_bottomLandColor, BottomLandColor);
        material.SetColor(_seaColor, SeaColor);
        material.SetColor(_deepSeaColor, DeepSeaColor);
        material.SetFloat(_colorGamma, ColorGamma);
        material.SetFloat(_heightRange, HeightRange);
        material.SetFloat(_seaLevel, SeaLevel);
        material.SetFloat(_seaBlend, SeaBlend);
        material.SetFloat(_linesIntensity, LinesIntensity);
        material.SetFloat(_linesBands, LinesBands);
        material.SetFloat(_lineWidth, LineWidth);
        material.SetFloat(_lineStrength, LineStrength);
        material.SetFloat(_shadowRange, ShadowRange);
        material.SetFloat(_shadowIntensity, ShadowIntensity);
    }
}
