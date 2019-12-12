using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public class LocalPlayer : RequiredSingleton<LocalPlayer>
{
    [Header("Settings")]
    public float CursorBoxMinDistance;
    public Faction Faction;

    [Header("State")] 
    public Vector2 ScreenMousePosition;
    public Vector2 WorldMousePosition;

    public bool LeftMouseButtonIsDown;
    public bool LeftMouseButtonIsHeld;
    public bool LeftMouseButtonIsUp;
    public bool RightMouseButtonIsDown;
    public bool RightMouseButtonIsHeld;
    public bool RightMouseButtonIsUp;

    public bool IsDragging;
    public bool FinishedDragging;
    public Vector2 ScreenDragStartPosition;
    public Vector2 WorldDragStartPosition;
    
    public bool CursorIsABox;
    public FRect ScreenCursorBox;
    public FRect WorldCursorBox;

    // Units under the cursor or the cursor box
    public RefLeakyList<Unit> UnitsInTheCursorBox;
    public RefLeakyList<Unit> PreviousUnitsInTheCursorBox;
    public RefLeakyList<Unit> SelectedUnits;
    public RefLeakyList<Unit> PreviouslySelectedUnits;
}
