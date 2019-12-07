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
            if (u.AttackCooldownCountdown > 0)
            {
                u.AttackCooldownCountdown -= dt;
            }
        }
    }
}