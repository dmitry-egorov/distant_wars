using Plugins.Lanski;
using UnityEngine;

public class ExplosionsManager : RequiredSingleton<ExplosionsManager>
{
    [Header("Settings")]
    public float ExplosionDuration;
    public float ExplosionSize;
    public MeshRenderer SpritesRenderer;

    [Header("State")]
    public LeakyList<Vector3> positions;
    public LeakyList<float> remaining_times;

    public Mesh SpritesMesh;
}
