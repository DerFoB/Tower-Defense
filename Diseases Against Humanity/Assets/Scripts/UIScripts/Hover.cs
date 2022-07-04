using UnityEngine;

public class Hover : Singleton<Hover>
{
    private SpriteRenderer SpriteRenderer;

    private void Start()
    {
        this.SpriteRenderer = GetComponent<SpriteRenderer>();
        this.SpriteRenderer.sortingOrder = Constants.HoverSortingLayer;
        Deactivate();
    }

    private void Update()
    {
        if (this.IsActivated)
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }

    public bool IsActivated => this.SpriteRenderer.enabled;

    public void Activate(Sprite sprite)
    {
        this.SpriteRenderer.sprite = sprite;
        this.SpriteRenderer.enabled = true;
    }

    public void Deactivate()
    {
        this.SpriteRenderer.enabled = false;
        this.ResetRotation();
    }

    public void SetRotation(float angle)
    {
        this.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void ResetRotation() => SetRotation(0f);
}
