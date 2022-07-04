using Assets.Scripts.DefenseScripts;
using System;
using System.Collections;
using UnityEngine;
using static Sounds;

public class WaterTower : SpeedEffectTower
{
    [SerializeField]
    private GameObject WaterTileEffectPrefab;

    public override bool IgnoreDirection => false;

    // This makes animation way more realistic
    private const float TileTimeOffset = 4 / 12f;

    public override int Index => 4;

    public override bool IsItem => false;

    protected override void SetAffectedTiles()
    {
        this.AffectedTiles.Add(Util.MoveGridPoint(this.LocationOnMap, this.Direction, 1));
        this.AffectedTiles.Add(Util.MoveGridPoint(this.LocationOnMap, this.Direction, 2));
        this.AffectedTiles.Add(Util.MoveGridPoint(this.LocationOnMap, this.Direction, 3));
    }

    protected override void Shoot()
    {
        SoundManager.GetInstance().PlaySFX(Sound.WaterSplat);
        StartCoroutine(nameof(AddTiles));
    }

    private IEnumerator AddTiles()
    {
        foreach (var gp in this.AffectedTiles)
        {
            var wes = Instantiate(this.WaterTileEffectPrefab, this.transform.parent).GetComponent<WaterEffectScript>();
            wes.Init(gp, this.transform.rotation.eulerAngles.z - 90, this.EffectDuration.Value, this.SpeedMultiplier, TileTimeOffset);
            yield return new WaitForSeconds(TileTimeOffset);
        }
    }
}
