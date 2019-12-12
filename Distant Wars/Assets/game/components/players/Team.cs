using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "DW/Team", order = 2)]
public class Team : ScriptableObject
{
    public Color Color;

    public byte Mask;

    void OnEnable() => Mask = (byte)(1 << Teams.AddAndGetIndex(this));

    public static List<Team> Teams => teams != null ? teams : teams = new List<Team>();
    private static List<Team> teams;
}

