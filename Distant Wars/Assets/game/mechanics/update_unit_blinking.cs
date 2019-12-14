using Plugins.Lanski;

public class update_unit_blinking : MassiveMechanic
{
    public void _()
    {
        /* delta time     */ var dt = Game.Instance.DeltaTime;
        /* units registry */ var ur = UnitsRegistry.Instance;
        /* blinking hide time */ var bht = ur.DamageBlinkHideTime;
        /* blinking show time */ var bst = ur.DamageBlinkShowTime;
        /* blinking period    */ var bp  = bht + bst;
            
        var units = ur.Units;
        var ucount = units.Count;
        /* damage blink time */ var bt = ur.DamageBlinkCount * (ur.DamageBlinkShowTime + ur.DamageBlinkHideTime);
        /* camera            */ var cam = StrategicCamera.Instance;
        /* screen rectangle  */ var rscreen = cam.WorldScreen;
        /* adjasted sprite size */ var ssize = (float)(ur.SpriteSize * (cam.ScreenResolution.y / 1080));
        /* screen rect offset to accomodate unit's size */ var offset = ssize * MathEx.Root2;
        /* adjusted screen rectangle */ var arscreen = rscreen.wider_by(offset);

        for (var iunit = 0; iunit < ucount; iunit++)
        {
            var u = units[iunit];

            if (arscreen.contains(u.Position))
            {
                var obt = u.BlinkTimeRemaining;
                if (u.ReceivedDamageSinceLastPresentation)
                {
                    obt = u.BlinkTimeRemaining = bt;
                    u.ReceivedDamageSinceLastPresentation = false;
                }

                /* new blinking time */ var nbt = obt - dt;
                // hide, when blinking and in hiding period
                if (nbt > 0)
                {
                    u.BlinkTimeRemaining = nbt;
                    u.IsBlinking = (nbt % bp) > bst;
                }
                else 
                {
                    u.IsBlinking = false;
                    if (obt != 0)
                    {
                        u.BlinkTimeRemaining = 0;
                    }
                }
            }
            else
            {
                u.ReceivedDamageSinceLastPresentation = false;
                u.BlinkTimeRemaining = 0;
                u.IsBlinking = false;
            }
        }
    }
}
