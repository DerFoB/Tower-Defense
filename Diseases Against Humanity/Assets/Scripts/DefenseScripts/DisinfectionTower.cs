using UnityEngine;
using static Sounds;

public class DisinfectionTower : MultiTargetTower
{
    [SerializeField]
    private GameObject DisinfectionCloudPrefab;

    [SerializeField]
    private Animator DisinfectionTowerAnimator;

    private const float DealDamageTimeOffset = 10f / 12f;
    private const float CloudDisappearTime = 3f;

    public override int Index => 3;

    public override bool IsItem => false;

    protected override void Shoot()
    {
        SoundManager.GetInstance().PlaySFX(Sound.DisinfectionSpray);
        this.DisinfectionTowerAnimator.SetTrigger("Start Animation");
        var dcs = Instantiate(this.DisinfectionCloudPrefab, this.transform.parent).GetComponent<DisinfectionCloudScript>();
        dcs.Init(this.LocationOnMap, CloudDisappearTime);
        Invoke(nameof(DealDamage), DealDamageTimeOffset);
    }

    private void DealDamage()
    {
        foreach (var enemy in this.Targets)
        {
            if (enemy != null && enemy.IsAttackable) // Sometimes enemy can die between Shooting and Dealing Damage
                enemy.Hit(this.Damage);
        }
    }
}
