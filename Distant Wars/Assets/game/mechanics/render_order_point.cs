using Plugins.Lanski;
using UnityEngine;

public class render_order_point : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying)
            return;
        
        var /* local player */ lp = LocalPlayer.Instance;
        var /* order point  */ op = OrderPoint.Instance;
        var /* order mesh renderer */ omr = op.MeshRenderer;
        var /* order line renderer */ olr = op.LineRenderer;

        var bu = lp.UnitsInTheCursorBox;

        if 
        (
            !lp.CursorIsABox 
            && bu.TryGetFirstItem(out var /* unit */ u) 
            && u.IssuedOrder.is_move(out var /* move order */ tp)
        )
        {
            /* strategic camera  */ var sc  = StrategicCamera.Instance;
            /* screen to world   */ var s2w = sc.ScreenToWorldTransform;
            /* orders z          */ var z   = op.Z;

            // render point
            {
                /* size */ var s = op.PointSize;

                omr.transform.position = tp.xy(z);
                omr.transform.localScale = s2w.apply_to_scalar(s).v3();
                omr.enabled = true;                
            }

            // render line
            {
                var /* line width */ w = s2w.apply_to_scalar(op.LineWidth);

                olr.SetPosition(0, u.Position.xy(z));
                olr.SetPosition(1, tp.xy(z));

                olr.startWidth = w;
                olr.endWidth = w;

                olr.enabled = true;
            }
        }
        else
        {
            omr.enabled = false;
            olr.enabled = false;
        }
    }
}