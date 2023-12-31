﻿using UnityEngine;

public class prepare_adujsted_sprite_size : IMassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var cam = StrategicCamera.Instance;

        var ssize = ur.adjusted_sprite_size = ur.SpriteSize * (cam.ScreenResolution.y / 1080);

        Shader.SetGlobalFloat(units_size_id, ssize);
    }

    static readonly int units_size_id = Shader.PropertyToID("_UnitsSize");
}
