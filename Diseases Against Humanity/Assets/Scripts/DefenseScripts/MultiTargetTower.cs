using System.Collections.Generic;
using UnityEngine;

public abstract class MultiTargetTower : DamageTower
{
    [SerializeField]
    private int MinimumEnemiesInRange = 5;

    private bool ShouldShoot = false;

    protected List<EnemyBase> Targets;

    protected override void Init()
    {
        this.Targets = new List<EnemyBase>();
        base.Init();
    }

    protected override void UpdateTarget()
    {
        var targets = new List<EnemyBase>();
        var enemyCount = 0;
        foreach (var enemy in this.GetAllEnemies())
        {
            if (Vector3.Distance(enemy.transform.position, this.transform.position) <= this.Range)
            {
                var es = enemy.GetComponent<EnemyBase>();
                if (es.IsAttackable)
                {
                    targets.Add(es);
                    enemyCount++;
                }
            }
        }

        this.Targets = targets;
        this.ShouldShoot = enemyCount >= this.MinimumEnemiesInRange;
    }

    private void Update()
    {
        this.RemainingReloadDuration -= Time.deltaTime;
        if(this.RemainingReloadDuration <= 0f && ShouldShoot)
        {
            this.Shoot();
            this.RemainingReloadDuration = this.ReloadDuration;
        }
    }

    protected abstract void Shoot();
}
