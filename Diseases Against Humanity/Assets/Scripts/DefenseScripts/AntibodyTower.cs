using static Sounds;

public class AntibodyTower : SingleTargetTower
{
    public override int Index => 0;

    public override bool IsItem => false;

    protected override void Shoot()
    {
        SoundManager.GetInstance().PlaySFX(Sound.Shoot);
        this.ShootingAnimator.SetTrigger("Start Animation");

        // Wait some time to better match shooting animation with acutal shooting the projectiles
        Invoke(nameof(this.SpawnProjectile), 2f / 12f);
    }
}
