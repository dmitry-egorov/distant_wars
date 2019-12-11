using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

[CreateAssetMenu(fileName = "Faction", menuName = "DW/Faction", order = 1)]
public class Faction : ScriptableObject
{
    public Color Color;

    public int Index;

    public Team Team;

    void OnEnable() => Index = Factions.AddAndGetIndex(this);

    public static List<Faction> Factions => factions != null ? factions : factions = new List<Faction>();
    private static List<Faction> factions;
}