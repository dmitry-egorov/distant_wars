using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class ProjectilesManager : RequiredSingleton<ProjectilesManager>
{
    [Header("Settings")]
    public int SpriteSize = 8;
    public MeshRenderer SpritesRenderer;

    [Header("Data")]
    public List<Vector3> Positions;
    public List<Vector3> Directions;
    public List<float> Speeds;
    public List<int> Damages;
    public Mesh SpritesMesh;
}
