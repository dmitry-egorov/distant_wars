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
            /* orders z          */ var z  = op.Z;
            /* size              */ var s  = op.PointSize;
            /* units registry    */ var ur = UnitsRegistry.Instance;
            /* units world scale */ var us = ur.WorldScale;

            // render point
            {
                omr.transform.position = tp.xy(z);
                omr.transform.localScale = (s * us).v3();
                omr.enabled = true;                
            }

            // render line
            {
                var /* strategic camera */ sc = StrategicCamera.Instance;
                var /* line width */ w = op.LineWidth * sc.SizeProportion;

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