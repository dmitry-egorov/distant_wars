public class update_selection_box : IMassiveMechanic
{
    public void _()
    {
        var sb = SelectionBox.Instance;
        var lp = LocalPlayer.Instance;

        if (lp.IsDragging)
        {
            var /* position */  p = lp.WorldCursorBox.get_center();
            var /* scale */     s = lp.WorldCursorBox.get_size();
            var /* transform */ t = sb.transform;
            t.position = p.xy(t.position.z);
            t.localScale = s.xy1();
            sb.Renderer.enabled = true;
        }
        else
        {
            sb.Renderer.enabled = false;
        }
    }
}