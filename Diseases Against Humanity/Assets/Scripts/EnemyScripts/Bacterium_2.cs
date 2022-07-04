using UnityEngine;

public class Bacterium_2 : Bacterium
{
    [SerializeField]
    private int _PepegaSpawnCount = 3;

    public int PepegaSpawnCount => _PepegaSpawnCount;

    [SerializeField]
    private GameObject PepegaPrefab;

    protected sealed override bool OnDeath()
    {
        // do the dying stuff...
        if (base.OnDeath())
        {
            // spawn Pepegas (only if bacterium died this frame!)
            for (int i = 0; i < this.PepegaSpawnCount; i++)
            {
                var pepega = Instantiate(this.PepegaPrefab).GetComponent<EnemyBase>();
                pepega.Init(new PathFinder(new PathInfo(this.PathFinder.NextWaypoint), this.PathFinder), LevelBuilder.GetInstance().Map);
            }
            return true;
        }
        return false;
    }
}
