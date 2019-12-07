using UnityEngine;

public class issue_unit_orders : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying)
            return;

        var /* local player */  lp = LocalPlayer.Instance;

        if (!lp.RightMouseButtonIsDown) 
            return;

        var /* selected units */ su = lp.SelectedUnits;
        var ucb = lp.UnitsInTheCursorBox;

        if (ucb.Count != 0)
        {
            // only single unit can be under the cursor when the right mouse button is pressed.
            // FEAT: attack multiple units
            // 7.12.19

            var /* other unit */ ou = ucb[0]; 

            //TODO: friend/foe/neutral distinction
            if (ou.Faction != lp.Faction)
            {
                foreach (var u in su)
                {
                    u.MoveTarget = default;
                    u.GuardTarget = default;
                    u.AttackTarget = ou;
                }
            }
            else
            {
                foreach (var u in su)
                {
                    u.MoveTarget = default;
                    u.AttackTarget = default;
                    u.GuardTarget = ou;
                }
            }
            
            return;
        }

        // issue move order
        {
            var /* target position */ t = lp.WorldMousePosition;

            foreach (var u in su)
            {
                u.AttackTarget = default;
                u.GuardTarget = default;
                u.MoveTarget = t;
            }
        }
    }
}