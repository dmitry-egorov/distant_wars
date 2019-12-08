using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class ProjectilesManager : RequiredSingleton<ProjectilesManager>
{
    [Header("Settings")]
    public MeshRenderer SpritesRenderer;

    [Header("Data")]
    public List<Vector3> Positions;
    public List<Vector3> Directions;
    public List<float> Speeds;
    public List<float> Damages;
    public Mesh SpritesMesh;
}
