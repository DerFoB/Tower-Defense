using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Virus_1 : Virus
{

    protected override float CalculateTrueSpeed()
    {
        // Virus 1 is immune against CC
        if (!this.IsAttackable) return 1f;

        // But not against Masks
        var tile = LevelBuilder.GetInstance().Tiles[Util.GetTilePosFromCentered(transform.position)];
        var modifier = tile.SpeedModifier;
        if (Mathf.Approximately(modifier, 0f))
            return 0f;

        // Do not get positive speed when standing before mask and getting hit by water
        if (modifier < 0f && tile.HasZeroSpeedModifier)
            return 0f;

        // Virus 1 gets nevertheless buffs
        return Mathf.Max(1f, modifier);
    }
}
