using UnityEngine;
using static Sounds;

public class SyringeTower : SingleTargetTower
{
    public override int Index => 1;

    public override bool IsItem => false;

    protected override void Shoot()
    {
        SoundManager.GetInstance().PlaySFX(Sound.Canon);
        this.ShootingAnimator.SetTrigger("Start Animation");
        // Wait some time to better match shooting animation with actual shooting the projectile
        Invoke(nameof(SpawnProjectile), 2f / 12f);
    }
}
