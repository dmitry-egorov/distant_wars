using UnityEngine;

public static class MouseEx
{
    public static Vector2 Position => Input.mousePosition.xy();

    public static Vector2 PositionDelta
    {
        get
        {
            var /* current frame */ cf = Time.frameCount;
            
            // handle initial state
            if 
            (
                last_frame == null || 
                last_position == null || 
                current_position == null || 
                last_frame.Value > cf
            )
            {
                current_position = last_position = Input.mousePosition.xy();
                last_frame = cf;
                return Vector2.zero;
            }

            var /* last frame */ lf = last_frame.Value;
            
            // handle frame change
            if (lf != cf)
            {
                last_frame = cf;
                var /* new position */ np = Position;
                last_position = lf == cf - 1 ? current_position.Value : np;
                current_position = np;
            }
            
            var /* last position */ lp = last_position.Value;
            var /* current position */ cp = current_position.Value;

            return cp - lp;
        }
    }


    private static Vector2? current_position;
    private static Vector2? last_position;
    private static int? last_frame;
}
