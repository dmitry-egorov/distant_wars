using Plugins.Lanski;
using Plugins.Lanski.Space;
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
    public Texture2D MapTexture;
    public RenderTexture VisionTexture;
    public RenderTexture DiscoveryTexture;

    public SpaceTransform2 world_to_map;
    public SpaceTransform2 map_to_world;
    public byte[] map_data;
    public float cell_size;
    public int width;
    public int height;
    public float ushort_z_scale;

    public void reload()
    {
        if (MapTexture == null)
        {
            MapTexture = new Texture2D(Width, Height, TextureFormat.R16, false, true)
            {
                name = "Height Map"
            };
        }

        if (HeightMap != null)
        {
            MapTexture.LoadRawTextureData(HeightMap.bytes);
        }

        MapTexture.filterMode = FilterMode.Point;
        MapTexture.anisoLevel = 0;
        MapTexture.Apply();

        Shader.SetGlobalTexture(_mapTex, MapTexture);
        Shader.SetGlobalFloat(map_scale_id, Scale);
    }

    public float slope_2d(Vector2 position, Vector2 normalized_direction)
    {
        var p = position;
        var nd = normalized_direction;

        var coord = coord_of(p);
        var offset = Vector2Int.RoundToInt(nd);
        var ocoord = coord + offset;
        var h  = z(coord);
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
        var w2m = world_to_map;
        return Vector2Int.FloorToInt(w2m.apply_to_point(p));
    }

    public float z(Vector2Int coord) => z(coord.x, coord.y);

    public float z(int x, int y)
    {
        var w = width;
        var h = height;

        if (x < 0 || x >= w || y < 0 || y >= h) return 0;
        
        var i = 2 * (w * y + x);
        return (map_data[i] + (map_data[i + 1] << 8)) * ushort_z_scale;
    }

    static readonly int _mapTex      = Shader.PropertyToID("_MapTex");
    static readonly int map_scale_id = Shader.PropertyToID("_MapScale");
}
