using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Faction", menuName = "DW/Faction", order = 1)]
public class Faction : ScriptableObject
{
    [FormerlySerializedAs("SpriteMaterial")] public Material DefaultSpriteMaterial;
    public Material HighlightedSpriteMaterial;
    public Material SelectedSpriteMaterial;
}