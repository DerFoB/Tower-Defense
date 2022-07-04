using UnityEngine;

public class Virus_2 : Virus
{
    [SerializeField]
    private float SpeedBuff;

    [SerializeField]
    private float SpeedBuffDisappearTime;

    private GridPoint CurrentGP;

    protected override void OnPreUpdate()
    {
        // Save current Grid Point
        this.CurrentGP = Util.GetTilePosFromCentered(this.transform.position);
    }

    protected override void OnPostUpdate()
    {
        var gp = Util.GetTilePosFromCentered(this.transform.position);

        // Grid Points changed?
        if(gp != this.CurrentGP)
        {
            LevelBuilder.GetInstance().Tiles[this.CurrentGP].RemoveSpeedModifierAfter(this.SpeedBuff, this.SpeedBuffDisappearTime);
            LevelBuilder.GetInstance().Tiles[gp].AddSpeedModifier(this.SpeedBuff, null);
        }
    }

    protected sealed override bool OnDeath()
    {
        if (base.OnDeath())
        {
            LevelBuilder.GetInstance().Tiles[Util.GetTilePosFromCentered(this.transform.position)].RemoveSpeedModifier(this.SpeedBuff);
            return true;
        }
        return false;
    }

}
