using Plugins.Lanski;

public class update_unit_blinking : IMassiveMechanic
{
    public void _()
    {
        /* delta time     */ var dt = Game.Instance.DeltaTime;
        /* units registry */ var ur = UnitsRegistry.Instance;
        /* blinking hide time */ var bht = ur.DamageBlinkHideTime;
        /* blinking show time */ var bst = ur.DamageBlinkShowTime;
        /* blinking period    */ var bp  = bht + bst;
            
        var units = ur.all_units;
        var ucount = units.Count;
        /* damage blink time */ var blink_time = ur.DamageBlinkCount * (ur.DamageBlinkShowTime + ur.DamageBlinkHideTime);
        /* camera            */ var cam = StrategicCamera.Instance;
        /* screen rectangle  */ var screen_rect = cam.WorldScreen;
        /* adjasted sprite size */ var adj_sprite_size = (float)(ur.SpriteSize * (cam.ScreenResolution.y / 1080));
        /* screen rect offset to accomodate unit's size */ var offset = adj_sprite_size * MathEx.Root2;
        /* adjusted screen rectangle */ var adj_screen_rect = screen_rect.wider_by(offset);

        for (var i_unit = 0; i_unit < ucount; i_unit++)
        {
            var u = units[i_unit];
            var u_pos = u.position;
            
            if (adj_screen_rect.contains(u_pos))
            {
                /* old blinking time */ var old_bt = u.blink_time_remaining;
                if (u.has_received_damage_since_last_presentation)
                {
                    old_bt = u.blink_time_remaining = blink_time;
                    u.has_received_damage_since_last_presentation = false;
                }

                /* new blinking time */ var new_bt = old_bt - dt;
                // hide, when blinking and in hiding period
                if (new_bt > 0)
                {
                    u.blink_time_remaining = new_bt;
                    u.is_blinking = (new_bt % bp) > bst;
                }
                else 
                {
                    u.is_blinking = false;
                    if (old_bt != 0)
                    {
                        u.blink_time_remaining = 0;
                    }
                }
            }
            else
            {
                u.has_received_damage_since_last_presentation = false;
                u.blink_time_remaining = 0;
                u.is_blinking = false;
            }
        }
    }
}
