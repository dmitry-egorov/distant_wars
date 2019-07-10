using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class LocalPlayer : RequiredSingleton<LocalPlayer>
{
    [Header("Settings")]
    public float CursorBoxMinDistance;

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
    
    public Rect ScreenCursorBox;
    public Rect WorldCursorBox;
    
    public bool SelectionChanged => selection_changed;
    
    // Units under the cursor or the cursor box
    public List<Unit> UnitsUnderTheCursorBox;
    public List<Unit> SelectedUnits;

    public void reset_selection_changed() => selection_changed = false;

    public void select_units(List<Unit> bu)
    {
        selection_changed = true;
        
        var /* selected units */ su = SelectedUnits;
        su.Clear();
        foreach (var u in bu) su.Add(u);
    }
    
    bool selection_changed;
}
