using UnityEngine;

public abstract class SingleTargetTower : DamageTower
{
    protected EnemyBase FocusedTarget;

    [Header("Tower Data")]
    [SerializeField]
    protected Transform RotationTransform;

    [SerializeField]
    protected Animator ShootingAnimator;

    [SerializeField]
    protected Transform ProjectileStartPoint;

    [SerializeField]
    protected GameObject ProjectilePrefab;

    [Header("Sorting Order")]
    [SerializeField]
    protected SpriteRenderer BaseSprite;

    [SerializeField]
    protected SpriteRenderer RotatingSprite;

    protected override void Init()
    {
        this.BaseSprite.sortingOrder = Constants.TowerSortingOrder;
        this.RotatingSprite.sortingOrder = Constants.TowerRotatingSortingOrder;
        base.Init();
    }

    protected override void UpdateTarget()
    {
        if (this.FocusedTarget != null) { return; }  // No update of target when already focused

        int highestTravelPoints = 0;
        GameObject bestFitTarget = null;

        foreach (GameObject enemy in this.GetAllEnemies())
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy <= this.Range)
            {
                var tt = enemy.GetComponent<EnemyBase>().TravelledTiles;
                if (highestTravelPoints < tt)
                {
                    bestFitTarget = enemy;
                    highestTravelPoints = tt;
                }
            }
        }

        this.FocusedTarget = bestFitTarget == null ? null : bestFitTarget.GetComponent<EnemyBase>();
    }

    private void Update()
    {
        this.RemainingReloadDuration -= Time.deltaTime;
        if (this.FocusedTarget == null) { return; }
        if (!this.FocusedTarget.IsAttackable)
        {
            this.FocusedTarget = null;
            return;
        }

        Vector3 d = FocusedTarget.transform.position - transform.position;
        if (d.magnitude > this.Range)
        {
            this.FocusedTarget = null;
            return;
        }

        Vector3 RotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * d;
        Quaternion TargetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: RotatedVectorToTarget);
        RotationTransform.rotation = Quaternion.RotateTowards(RotationTransform.rotation, TargetRotation, this.RotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(TargetRotation, RotationTransform.rotation) <= 5 && this.RemainingReloadDuration <= 0f && this.FocusedTarget.IsAttackable)
        {
            Shoot();
            this.RemainingReloadDuration = this.ReloadDuration;
        }
    }

    protected abstract void Shoot();

    protected void SpawnProjectile()
    {
        GameObject ProjectileGO = Instantiate(ProjectilePrefab, ProjectileStartPoint.position, ProjectileStartPoint.rotation);
        ProjectileScript Projectile = ProjectileGO.GetComponent<ProjectileScript>();

        if (Projectile != null)
        {
            Projectile.Seek(this.FocusedTarget, this.transform.parent, this.Damage);
        }
    }
}
