internal class init_camera : IMassiveMechanic
{
    public void _()
    {
        var sc = StrategicCamera.Instance;
        foreach(var c in sc.Cameras)
        {
            c.eventMask = 0;
        }
    }
}