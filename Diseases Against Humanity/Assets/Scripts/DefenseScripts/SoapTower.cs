using Assets.Scripts.DefenseScripts;
using UnityEngine;
using static Sounds;

public class SoapTower : SpeedEffectTower
{
    [SerializeField]
    private GameObject SoapTileEffectPrefab;

    [SerializeField]
    private Animator BaseAnimator;

    [SerializeField]
    private Animator PusherAnimator;

    public override bool IgnoreDirection => false;

    // Animation looks a lot better if slowness is applied with some time offset
    private const float AnimationTimeOffset = 6 / 12f;

    public override int Index => 2;

    public override bool IsItem => false;

    protected override void SetAffectedTiles()
    {
        var frontPoint = Util.MoveGridPoint(this.LocationOnMap, this.Direction, 1);
        this.AffectedTiles.Add(frontPoint);
        this.AffectedTiles.Add(Util.MoveGridPoint(frontPoint, this.Direction.TurnLeft(), 1));
        this.AffectedTiles.Add(Util.MoveGridPoint(frontPoint, this.Direction.TurnRight(), 1));
    }

    protected override void Shoot()
    {
        SoundManager.GetInstance().PlaySFX(Sound.SoapSplat);
        this.BaseAnimator.SetTrigger("Start Animation");
        this.PusherAnimator.SetTrigger("Start Animation");

        foreach(var gp in this.AffectedTiles)
        {
            var ses = Instantiate(SoapTileEffectPrefab, this.transform.parent).GetComponent<SoapEffectScript>();
            ses.Init(gp, this.EffectDuration.Value + AnimationTimeOffset);
        }
        Invoke(nameof(this.ApplyEffect), AnimationTimeOffset);
    }
}
