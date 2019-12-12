using Plugins.Lanski;
using UnityEngine;

public class ProjectilesManager : RequiredSingleton<ProjectilesManager>
{
    [Header("Settings")]
    public int SpriteSize = 8;
    public float HitRadius = 2;
    public MeshRenderer SpritesRenderer;

    [Header("Data")]
    public LeakyList<Vector3> Positions;
    public LeakyList<Vector3> Directions;
    public LeakyList<float> Speeds;
    public LeakyList<int> Damages;
    public Mesh SpritesMesh;
}
