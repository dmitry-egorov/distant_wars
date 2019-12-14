using Plugins.Lanski;
using UnityEngine;

public class Map : RequiredSingleton<Map>
{
    [Header("Settings")]
    public TextAsset HeightMap;
    public int Width = 1025;
    public int Height = 1025;
    public float Scale = 8000;
    public float ZScale = 1000;
    public float BoundingRadius = 6000;

    public Camera VisionCamera;
    public Camera DiscoveryCamera;
    public int VisionTextureSize;
    public int DiscoveryTextureSize;
    public bool TexturesReady;
    public MeshRenderer MapRenderer;

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
        Shader.SetGlobalFloat(map_scale_id, Scale);
    }

    public void initialize()
    {
        reload();

        MapRenderer.transform.localScale = Scale.v3();
        map_data = HeightMap.bytes;
        cell_size = Scale / Width;
        width = Width;
        height = Height;
        size = Scale;
        ushort_z_scale = ZScale / (float)ushort.MaxValue;
    }

    public float slope(Vector2 position, Vector2 direction) => slope2(position, direction.normalized);

    public float slope2(Vector2 position, Vector2 normalized_direction)
    {
        var p = position;
        var nd = normalized_direction;

        var coord = coord_of(p);
        var offset = new Vector2Int(Mathf.RoundToInt(nd.x), Mathf.RoundToInt(nd.y));
        var ocoord = coord + offset;
        var h = z(coord);
        var ho = z(ocoord);

        return (ho - h) / cell_size;
    }

    public Vector3 xyz(Vector2 xy) => xy.xy(z(xy));

    public float z(Vector2 position)
    {
        var coord = coord_of(position);
        return z(coord);
    }

    public Vector2Int coord_of(Vector2 position)
    {
        var p = position;
        return new Vector2Int(width / 2 + Mathf.RoundToInt(p.x / cell_size), height / 2 + Mathf.RoundToInt(p.y / cell_size));
    }

    public float z(Vector2Int coord) => z(coord.x, coord.y);

    public float z(int x, int y)
    {
        var w = width;
        var h = height;

        if (x < 0 || x >= width || y < 0 || y >= height) return 0;
        
        var i = 2 * (w * y + x);
        return (map_data[i] + (map_data[i + 1] << 8)) * ushort_z_scale;
    }

    private byte[] map_data;
    private float cell_size;
    private int width;
    private int height;
    private float size;
    private float ushort_z_scale;

    static readonly int _mapTex      = Shader.PropertyToID("_MapTex");
    static readonly int map_scale_id = Shader.PropertyToID("_MapScale");
}
