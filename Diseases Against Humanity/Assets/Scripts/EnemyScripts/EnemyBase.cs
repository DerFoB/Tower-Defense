using System.Collections;
using UnityEngine;
using static Sounds;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField]
    protected float BaseMovementSpeed = 1f;

    [SerializeField]
    protected int Hitpoints = 1;

    [SerializeField]
    protected int KillReward = 1;

    [SerializeField]
    protected int ResearchKillReward = 0;

    [SerializeField]
    public int Damage = 1;

    [SerializeField]
    protected float WobbleFactor = 0.15f;

    protected float WobbleFactorInTiles;

    protected SpriteRenderer SpriteRenderer;

    protected Vector3 TargetWorldPoint;

    protected  PathFinder PathFinder;

    public int TravelledTiles => PathFinder.TravelledTiles;

    public bool IsAttackable { get; protected set; } = true;

    protected bool IsDead = false;

    public abstract bool IsVirus { get; }

    // one random for all enemies, this prevents them from starting with the same value
    protected static readonly System.Random Random = new System.Random();

    public void Init(PathFinder pathFinder, Transform parent)
    {
        this.WobbleFactorInTiles = this.WobbleFactor * Util.TileSize.x;
        this.PathFinder = pathFinder;
        this.transform.position = Util.GetWorldPosCentered(pathFinder.TargetPoint);
        this.transform.SetParent(parent);
        this.TargetWorldPoint = this.transform.position;
        this.SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        this.SpriteRenderer.sortingOrder = Constants.EnemySortingOrder;
    }

    protected virtual void Init() { }

    private void OnDestroy()
    {
        this.Destroy();
        WaveManager.GetInstance().KilledEnemiesInWave++;
    }

    protected virtual void Destroy() { }

    public void Hit(int damage)
    {
        this.Hitpoints -= damage;
        if (this.Hitpoints <= 0)
        {
            OnDeath();
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        this.OnPreUpdate();
        var dir = TargetWorldPoint - transform.position;
        var speedMultiplier = this.CalculateTrueSpeed();
        var stepSize = this.BaseMovementSpeed * Time.deltaTime * speedMultiplier;
        if (dir.magnitude <= stepSize)
        {
            transform.position = TargetWorldPoint;
            if (this.PathFinder.IsAtEnd)
            {
                // We are at the end now!
                OnReachEnd();
            }
            else
            {
                var wasBelowCrossingBridge = this.PathFinder.IsBelowCrossingBridge;
                this.PathFinder.UpdateTargetPoint();
                this.TargetWorldPoint = Util.GetWorldPosCentered(this.PathFinder.TargetPoint).Randomize(this.WobbleFactorInTiles, this.WobbleFactorInTiles, 0, Random);
                if (wasBelowCrossingBridge || this.PathFinder.IsBelowCrossingBridge)
                {
                    StartCoroutine(FadeTo(0.1f, 0.25f));
                    this.IsAttackable = false;
                }
                else
                {
                    StartCoroutine(FadeTo(1f, 0.25f));
                    this.IsAttackable = true;
                }
            }
        }
        else
        {
            transform.position += dir.normalized * stepSize;
        }
        this.OnPostUpdate();
    }

    protected virtual float CalculateTrueSpeed()
    {
        return this.IsAttackable ? LevelBuilder.GetInstance().Tiles[Util.GetTilePosFromCentered(transform.position)].SpeedModifier : 1f;
    }

    protected virtual void OnPreUpdate() { }
    protected virtual void OnPostUpdate() { }

    protected virtual void OnReachEnd()
    {
        GameManager.GetInstance().DecreaseHealth(this.Damage);
        Destroy(this.gameObject);
    }

    protected virtual bool OnDeath()
    {
        // Avoid multiple hits in the same frame to cause multiple deaths (only works if this method is not called multiple times at once (we assume that) because no locking implemented)
        if (this.IsDead) return false;

        this.IsDead = true;
        SoundManager.GetInstance().PlaySFX(this.IsVirus ? Sound.BlopVirus : Sound.BlopBakterie);

        // Important: First destroy then notify GameManager!
        Destroy(this.gameObject);
        GameManager.GetInstance().EnemyDied(this.KillReward, this.ResearchKillReward);
        return true;
    }

    protected IEnumerator FadeTo(float value, float time)
    {
        float alpha = this.SpriteRenderer.color.a;
        for (float t = 0; t < 1.0f; t += Time.deltaTime / time)
        {
            var c = new Color(1f, 1f, 1f, Mathf.Lerp(alpha, value, t));
            this.SpriteRenderer.color = c;
            yield return null;
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = this.IsAttackable ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
