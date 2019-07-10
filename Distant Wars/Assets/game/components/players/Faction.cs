using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Faction", menuName = "DW/Faction", order = 1)]
public class Faction : ScriptableObject
{
    [FormerlySerializedAs("SpriteMateral")] public Material SpriteMaterial;

    public Material DefaultSpriteMaterial
    {
        get
        {
            if (defaultSpriteMaterial == null)
            {
                defaultSpriteMaterial = new Material(SpriteMaterial);
                defaultSpriteMaterial.name = SpriteMaterial.name + "Default";
                defaultSpriteMaterial.EnableKeyword("DEFAULT");
            }

            return defaultSpriteMaterial;
        }
    }
    
    public Material HighlightedSpriteMaterial
    {
        get
        {
            if (highlightedSpriteMaterial == null)
            {
                highlightedSpriteMaterial = new Material(SpriteMaterial);
                highlightedSpriteMaterial.name = SpriteMaterial.name + "Highlihted";
                highlightedSpriteMaterial.EnableKeyword("HIGHLIGHTED");
            }

            return highlightedSpriteMaterial;
        }
    }
    
    public Material SelectedSpriteMaterial
    {
        get
        {
            if (selectedSpriteMaterial == null)
            {
                selectedSpriteMaterial = new Material(SpriteMaterial);
                selectedSpriteMaterial.name = SpriteMaterial.name + "Selected";
                selectedSpriteMaterial.EnableKeyword("SELECTED");
            }

            return selectedSpriteMaterial;
        }
    }

    Material defaultSpriteMaterial;
    Material highlightedSpriteMaterial;
    Material selectedSpriteMaterial;
}