using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class BulletsManager : RequiredSingleton<BulletsManager>
{
    [Header("Settings")]
    public MeshRenderer SpritesRenderer;

    [Header("Data")]
    public List<Vector2> Positions;
    public List<Vector2> Velocities;
    public List<float> Damages;
    public Mesh SpritesMesh;
}
