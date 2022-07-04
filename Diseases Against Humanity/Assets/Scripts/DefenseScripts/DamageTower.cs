using UnityEngine;

public abstract class DamageTower : TowerBase
{
    [Header("Attributes")]
    [SerializeField]
    protected int Damage;

    [SerializeField]
    private float TileRange;

    protected float Range;


    [SerializeField]
    protected float ReloadDuration;

    [SerializeField]
    protected float RotationSpeed;

    protected float RemainingReloadDuration;

    protected override void Init()
    {
        this.Range = Util.TileSize.x * this.TileRange;
        base.Init();
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, this.Range);
    }
}
