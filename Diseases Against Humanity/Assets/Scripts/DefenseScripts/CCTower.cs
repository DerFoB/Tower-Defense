using Assets.Scripts.DefenseScripts;
using System.Collections.Generic;

public abstract class CCTower : DirectionDependentTower
{
    public abstract HashSet<GridPoint> AffectedTiles { get; protected set; }

    protected abstract float SpeedMultiplier { get; }
    protected abstract float? EffectDuration { get; }

    protected void ApplyEffect()
    {
        var tiles = LevelBuilder.GetInstance().Tiles;
        foreach(var gp in this.AffectedTiles)
        {
            tiles[gp].AddSpeedModifier(this.SpeedMultiplier, this.EffectDuration);
        }
    }

    protected override void Destroy()
    {
        // Remove speed modifier
        var tiles = LevelBuilder.GetInstance()?.Tiles;
        if (tiles == null) return; // Sometimes this happens when the game is closed
        foreach(var gp in this.AffectedTiles)
        {
            tiles[gp].RemoveSpeedModifier(this.SpeedMultiplier);
        }
    }
}
