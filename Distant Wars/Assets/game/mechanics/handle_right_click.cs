using UnityEngine;

public class handle_right_click : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;

        var /* local player */  lp = LocalPlayer.Instance;

        if (!lp.RightMouseButtonIsDown) return;

        var /* selected units*/ su = lp.SelectedUnits;
        var /* target */         t = lp.WorldMousePosition;

        foreach (var u in su)
        {
            u.issue_move_order(t);
        }
    }
}