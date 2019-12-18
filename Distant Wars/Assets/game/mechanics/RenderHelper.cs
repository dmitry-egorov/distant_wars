using System.Collections.Generic;

public static class RenderHelper
{
    public static void add_quad(this List<int> vt, int i)
    {
        vt.Add(i * 4 + 0);
        vt.Add(i * 4 + 1);
        vt.Add(i * 4 + 2);
        vt.Add(i * 4 + 1);
        vt.Add(i * 4 + 3);
        vt.Add(i * 4 + 2);
    }
}
