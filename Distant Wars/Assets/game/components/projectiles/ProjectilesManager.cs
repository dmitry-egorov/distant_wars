using Plugins.Lanski;
using UnityEngine;

public class ProjectilesManager : RequiredSingleton<ProjectilesManager>
{
    [Header("Settings")]
    public int SpriteSize = 8;
    public float HitRadius = 2;
    public MeshRenderer SpritesRenderer;

    [Header("Data")]
    public LeakyList<Vector3> positions;
    public LeakyList<Vector2> prev_positions;
    public LeakyList<Vector3> directions;
    public LeakyList<float> speeds;
    public LeakyList<int> damages;

    public Mesh SpritesMesh;
}
