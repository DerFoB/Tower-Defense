using UnityEngine;

public abstract class TowerBase : MonoBehaviour
{
    [SerializeField]
    public Sprite Thumbnail;

    [SerializeField]
    public int ConstructionCost;

    [SerializeField]
    public float ConstructionCostIncreaseFactor = 1f;

    [SerializeField]
    public float ReimbursementFactor = 0f;

    [SerializeField]
    protected float UpdateTargetFirstDelay = 0f;

    [SerializeField]
    protected float UpdateTargetInterval = 0.5f;

    public GridPoint LocationOnMap { get; protected set; }

    public abstract int Index { get; }

    public abstract bool IsItem { get; }

    public void Init(GridPoint gp, Transform parent)
    {
        this.LocationOnMap = gp;
        this.transform.position = Util.GetWorldPosCentered(gp);
        this.transform.parent = parent;
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = Constants.TowerSortingOrder;
        this.Init();
    }

    protected virtual void Init()
    {
        InvokeRepeating(nameof(this.UpdateTarget), this.UpdateTargetFirstDelay, this.UpdateTargetInterval);
    }

    protected void OnDestroy()
    {
        this.Destroy();
    }

    protected virtual void Destroy() { }

    protected virtual void UpdateTarget() { }

    protected GameObject[] GetAllEnemies()
    {
        return GameObject.FindGameObjectsWithTag(Constants.EnemyTag);
    }
}
