using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public Obstacle Obstacle { get; private set; }
    public GridPoint OriginPoint => Obstacle.Position;
    public Vector2 CenterWorldPosition { get; private set; }


    [SerializeField]
    private int SizeX;
    [SerializeField]
    private int SizeY;

    public void Init(Obstacle o, Vector3 worldPosition, Transform parent)
    {
        this.Obstacle = o;
        transform.position = worldPosition;
        transform.SetParent(parent);

        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = Constants.ObstaclesSortingOrder;    // Make sure, obstacles do override normal tiles

        var size = sr.bounds.size;
        this.CenterWorldPosition = new Vector2(worldPosition.x + size.x / 2, worldPosition.y - size.y / 2);
    }

    public List<GridPoint> GetAffectedGridPoints()
    {
        var l = new List<GridPoint>();

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                l.Add(new GridPoint(this.OriginPoint.X + x, this.OriginPoint.Y + y));
            }
        }

        return l;
    }
}
