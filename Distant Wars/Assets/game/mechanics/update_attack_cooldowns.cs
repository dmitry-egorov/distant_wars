using UnityEngine;

internal class update_attack_cooldowns : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.Units;
        var dt = Time.deltaTime;
        foreach(var u in us)
        {
            var cd = u.AttackCountdown;
            u.AttackCountdown = cd > 0 ? cd - dt : 0;
        }
    }
}