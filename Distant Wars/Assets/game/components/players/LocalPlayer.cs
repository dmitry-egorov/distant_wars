using System.Collections.Generic;
using Plugins.Lanski;
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
    public Rect ScreenCursorBox;
    public Rect WorldCursorBox;

    // Units under the cursor or the cursor box
    public List<Unit> UnitsInTheCursorBox;
    public List<Unit> PreviousUnitsInTheCursorBox;
    public List<Unit> SelectedUnits;
    public List<Unit> PreviouslySelectedUnits;
}
