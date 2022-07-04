using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{

    public bool IsBorder { get; set; }
    public bool CanBuild { get; set; }
    public bool IsWay { get; set; }

    public GridPoint GridPosition { get; private set; }
    public Vector2 CenterWorldPosition { get; private set; }

    public SpriteRenderer SpriteRenderer { get; private set; }

    private readonly List<float> SpeedModifiers = new List<float>();
    public float SpeedModifier { get; private set; }
    public bool HasZeroSpeedModifier => this.SpeedModifiers.Contains(0f);

    public virtual void Init(GridPoint gridPosition, Vector3 worldPosition, Transform parent, bool canBuild, bool isBorder, bool isWay)
    {
        this.CanBuild = canBuild;
        this.IsBorder = isBorder;
        this.IsWay = isWay;

        this.GridPosition = gridPosition;
        transform.position = worldPosition;
        transform.SetParent(parent);

        this.SpriteRenderer = GetComponent<SpriteRenderer>();
        this.SpriteRenderer.sortingOrder = Constants.MapTileSortingOrder;    // Make sure, map tiles are behind every other tile
        var size = this.SpriteRenderer.bounds.size;
        this.CenterWorldPosition = new Vector2(worldPosition.x + size.x / 2, worldPosition.y - size.y / 2);

        this.gameObject.AddComponent<BoxCollider2D>(); // Necessary for MouseOver and MouseExit Events

        UpdateSpeedModifier();
    }

    public void AddSpeedModifier(float speedMultiplicator, float? duration)
    {
        this.SpeedModifiers.Add(speedMultiplicator);
        this.UpdateSpeedModifier();

        // Null-values mean "forever"
        if (duration.HasValue)
        {
            RemoveSpeedModifierAfter(speedMultiplicator, duration.Value);
        }
    }

    public void RemoveSpeedModifier(float speedMultiplicator)
    {
        this.SpeedModifiers.Remove(speedMultiplicator);
        this.UpdateSpeedModifier();
    }

    private void UpdateSpeedModifier()
    {
        float speed = 1f;
        foreach (var speedModifier in SpeedModifiers)
        {
            if (speedModifier < 0f)
            {
                this.SpeedModifier = -1f;
                return;
            }
            else
            {
                speed *= speedModifier;
            }
        }

        this.SpeedModifier = speed;
    }

    public void RemoveSpeedModifierAfter(float value, float duration)
    {
        StartCoroutine(RemoveSpeedModifierAfterCoroutine(value, duration));
    }

    private IEnumerator RemoveSpeedModifierAfterCoroutine(float value, float duration)
    {
        yield return new WaitForSeconds(duration);
        RemoveSpeedModifier(value);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = CanBuild ? Color.green : Color.red;
        Gizmos.DrawWireCube(CenterWorldPosition, new Vector3(0.8f, 0.8f));
    }

    private void OnMouseOver()
    {
        if (this.IsBorder || GameManager.GetInstance().IsMouseOverBuildMenu) { return; } // border tiles have no interaction

        GameManager.GetInstance().HighlightTileOnMouseHover(this);
        if (this.CanBuild && GameManager.GetInstance().IsPlacingTower)
        {
            if (GetTowerActionInput())
            {
                GameManager.GetInstance().BuyTower(this.GridPosition);
            }
        }
        else if (!this.CanBuild && GameManager.GetInstance().IsDemolishingTower)
        {
            if (GetTowerActionInput())
            {
                GameManager.GetInstance().DemolishTower(this.GridPosition);
            }
        }
        else if (this.IsWay && GameManager.GetInstance().IsPlacingItem)
        {
            if (GetTowerActionInput())
            {
                GameManager.GetInstance().BuyTower(this.GridPosition);
            }
        }
    }

    private bool GetTowerActionInput()
    {
        bool input = Input.GetMouseButtonUp(0) | (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended);
        return input && !Util.HasMovedSignificantly(CameraMovement.GetInstance().PanStartPosition, CameraMovement.GetInstance().CurrentPosition);
    }

    private void OnMouseExit()
    {
        if (GameManager.GetInstance().IsPlacingTower || GameManager.GetInstance().IsDemolishingTower || GameManager.GetInstance().IsPlacingItem)
        {
            GameManager.GetInstance().RedoTileColoring(this);
        }
    }
}
