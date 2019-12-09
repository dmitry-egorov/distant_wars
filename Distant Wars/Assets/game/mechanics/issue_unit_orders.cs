public class issue_unit_orders : MassiveMechanic
{

    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        if (!lp.RightMouseButtonIsDown) 
            return;

        var /* selected units */ su = lp.SelectedUnits;
        var ucb = lp.UnitsInTheCursorBox;

        if (ucb.Count != 0)
        {
            // only single unit can be under the cursor when the right mouse button is pressed.
            // FEAT: attack multiple units
            // 7.12.19

            var /* target unit */ tu = ucb[0]; 

            //FEAT: friend/foe/neutral distinction
            if (tu.Faction != lp.Faction)
            {
                foreach (var u in su) u.IssuedOrder = Unit.Order.attack(tu);
            }
            else
            {
                foreach (var u in su) u.IssuedOrder = Unit.Order.guard(tu);
            }
            
            return;
        }

        // issue move order
        {
            var /* target position */ tp = lp.WorldMousePosition;

            foreach (var u in su)
                u.IssuedOrder = Unit.Order.move(tp);
        }
    }
}