using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "DW/Faction", order = 1)]
public class Faction : ScriptableObject
{
    public Material DefaultSpriteMateral;

    public Material HighlightedSpriteMaterial
    {
        get
        {
            if (highlightedSpriteMaterial == null)
            {
                highlightedSpriteMaterial = new Material(DefaultSpriteMateral);
                highlightedSpriteMaterial.name = DefaultSpriteMateral.name + "Selected";
                highlightedSpriteMaterial.EnableKeyword("WHITE_ICON");
            }

            return highlightedSpriteMaterial;
        }
    }

    Material highlightedSpriteMaterial;
}