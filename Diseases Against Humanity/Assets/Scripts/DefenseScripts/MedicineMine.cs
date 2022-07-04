using System.Collections.Generic;
using UnityEngine;
using static Sounds;

public class MedicineMine : DamageTower
{
    [SerializeField]
    private float DetectionRange;

    [SerializeField]
    private int MinimumEnemiesForDetonation;

    [SerializeField]
    private Animator ExplosionAnimator;

    private bool AlreadyDetonated = false;

    public override int Index => 6;

    public override bool IsItem => true;

    protected override void UpdateTarget()
    {
        if (this.AlreadyDetonated) return;

        var enemiesInRange = new List<EnemyBase>();
        var enemiesDetected = 0;
        foreach (GameObject enemy in this.GetAllEnemies())
        {
            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= this.Range)
            {
                var es = enemy.GetComponent<EnemyBase>();
                if (es.IsAttackable)
                {
                    enemiesInRange.Add(es);
                    if (distance <= this.DetectionRange)
                        enemiesDetected++;
                }
            }

        }

        if (enemiesDetected >= this.MinimumEnemiesForDetonation)
        {
            this.AlreadyDetonated = true;
            SoundManager.GetInstance().PlaySFX(Sound.Explosion);
            enemiesInRange.ForEach(x => x.Hit(this.Damage));
            this.ExplosionAnimator.SetTrigger("Start Animation");
            GameObject.Destroy(this.gameObject, 3f);
        }
    }
}
