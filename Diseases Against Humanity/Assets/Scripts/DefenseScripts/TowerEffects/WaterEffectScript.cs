using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffectScript : MonoBehaviour
{
    private GridPoint Location;
    private float SpeedEffect;
    private float Duration;


    public void Init(GridPoint position, float rotation, float duration, float speedEffect, float timeOffset)
    {
        this.Location = position;
        this.Duration = duration;
        this.SpeedEffect = speedEffect;
        this.transform.position = Util.GetWorldPosCentered(position);
        this.transform.rotation = Quaternion.Euler(0, 0, rotation);
        this.GetComponent<SpriteRenderer>().sortingOrder = Constants.ProjectileSortingOrder;
        Destroy(this.gameObject, duration + timeOffset);
        Invoke(nameof(this.ApplyEffect), timeOffset);
    }

    public void ApplyEffect()
    {
        LevelBuilder.GetInstance().Tiles[this.Location].AddSpeedModifier(this.SpeedEffect, this.Duration);
    }
}
