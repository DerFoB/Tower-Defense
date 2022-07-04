using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private EnemyBase Target;

    [SerializeField]
    private float Speed;

    private int Damage;

    public void Seek(EnemyBase target, Transform parent, int damage)
    {
        this.Damage = damage;
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = Constants.ProjectileSortingOrder;
        this.transform.parent = parent;
        Target = target;
    }


    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 d = Target.transform.position - transform.position;
        float DistancePerSecond = Speed * Time.deltaTime;
        if (d.magnitude <= DistancePerSecond)
        {
            HitTarget();
            return;
        }

        transform.Translate(d.normalized * DistancePerSecond, Space.World);
    }

    void HitTarget()
    {
        Destroy(gameObject);
        Target.Hit(this.Damage);
    }
}
