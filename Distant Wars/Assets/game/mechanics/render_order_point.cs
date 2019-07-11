using Plugins.Lanski;
using UnityEngine;

public class render_order_point : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying)
            return;
        
        var /* local player */ lp = LocalPlayer.Instance;
        var  /* order point */ op = OrderPoint.Instance;
        var /* order mesh renderer */ omr = op.MeshRenderer;
        var /* order line renderer */ olr = op.LineRenderer;

        var bu = lp.UnitsInTheCursorBox;

        if 
        (
            !lp.CursorIsABox 
            && bu.TryGetFirstItem(out var /* unit */ u) 
            && u.MoveTarget.TryGetValue(out var /* target */ t)
        )
        {
            var /* orders z */ z = op.Z;
            var /* size */ s = op.PointSize;
            var /* units registry */ ur = UnitsRegistry.Instance;
            var us = ur.WorldScale;

            // render point
            {
                omr.transform.position = t.xy(z);
                omr.transform.localScale = (s * us).v3();
                omr.enabled = true;                
            }

            // render line
            {
                var /* strategic camera */ sc = StrategicCamera.Instance;
                var /* line width */ w = op.LineWidth * sc.SizeProportion;

                olr.SetPosition(0, u.Position.xy(z));
                olr.SetPosition(1, t.xy(z));

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