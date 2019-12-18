
internal class update_explosions : MassiveMechanic
{
    public void _()
    {
        var em = ExplosionsManager.Instance;
        var eposs = em.positions;
        var ertimes = em.remaining_times;
        var ecount = eposs.Count;

        var dt = Game.Instance.DeltaTime;

        for (var iexplosion = 0; iexplosion < ecount;)
        {
            ref var ertime = ref ertimes[iexplosion];
            var nertime = ertime - dt;

            if (nertime <= 0)
            {
                eposs.ReplaceWithLast(iexplosion);
                ertimes.ReplaceWithLast(iexplosion);

                ecount--;
            }
            else
            {
                ertime = nertime;
                iexplosion++;
            }
        }
    }
}
