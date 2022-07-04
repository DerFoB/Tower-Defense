using System.Collections.Generic;
using UnityEngine;

public class MaskBarrier : CCTower
{
    [SerializeField]
    private int MaxHealth = 200;

    [SerializeField]
    private Sprite[] HealthStatusSprites;

    public int ActualHealth;

    public bool IsFullHealth => this.ActualHealth == this.MaxHealth;

    public override HashSet<GridPoint> AffectedTiles { get; protected set; }

    protected override float SpeedMultiplier => 0f; // Stop enemies

    protected override float? EffectDuration => null; // Forever (until Mask gets broken or deleted)

    public override bool IgnoreDirection => true;

    public override int Index => 7;

    public override bool IsItem => true;

    protected override void Init()
    {
        this.ActualHealth = this.MaxHealth;
        this.AffectedTiles = new HashSet<GridPoint>
        {
            this.LocationOnMap
        };

        base.Init();
        this.ApplyEffect();
    }

    protected override sealed void UpdateTarget()
    {
        var enemies = new List<EnemyBase>();
        foreach (GameObject enemyObj in this.GetAllEnemies())
        {
            // 0.71 = sqrt(2) / 2
            var enemy = enemyObj.GetComponent<EnemyBase>();
            if (Vector3.Distance(transform.position, enemyObj.transform.position) <= 0.71f * Util.TileSize.x && enemy.IsAttackable)
            {
                enemies.Add(enemy);
            }
        }

        enemies.ForEach(x => this.ActualHealth -= x.Damage);
        this.ActualHealth=  Mathf.Clamp(this.ActualHealth, 0, this.MaxHealth);
        if (this.ActualHealth <= 0)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        this.ReimbursementFactor = (float)this.ActualHealth / (float)this.MaxHealth;
        UpdateSprite();
    }

    protected void UpdateSprite()
    {
        int index = 0;
        float healthStatus = (float)this.ActualHealth / (float)this.MaxHealth;
        if (healthStatus < 0.2f)
        {
            index = 4;
        }
        else if (healthStatus < 0.4f)
        {
            index = 3;
        }
        else if (healthStatus < 0.6f)
        {
            index = 2;
        }
        else if (healthStatus < 0.8f)
        {
            index = 1;
        }

        this.GetComponent<SpriteRenderer>().sprite = this.HealthStatusSprites[index];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f * Util.TileSize.x);
    }
}
