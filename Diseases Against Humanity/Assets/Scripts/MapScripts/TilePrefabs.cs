using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TilePrefabs : ISerializationCallbackReceiver
{
    [Header("Tiles")]
    [SerializeField] public GameObject[] WayStraightHorizontal;
    [SerializeField] public GameObject[] WayStraightVertical;
    [SerializeField] public GameObject[] BridgeStraightHorizontalTopBottom;
    [SerializeField] public GameObject[] BridgeStraightHorizontalBottomTop;
    [SerializeField] public GameObject[] BridgeStraightVerticalLeftRight;
    [SerializeField] public GameObject[] BridgeStraightVerticalRightLeft;
    [SerializeField] public GameObject[] WayCurveLeftTop;
    [SerializeField] public GameObject[] WayCurveLeftBottom;
    [SerializeField] public GameObject[] WayCurveRightTop;
    [SerializeField] public GameObject[] WayCurveRightBottom;
    [SerializeField] public GameObject[] WayCrossHorizontal;
    [SerializeField] public GameObject[] WayCrossVertical;
    [SerializeField] public GameObject[] Buildable;
    [SerializeField] public GameObject[] RiverStraightLeftRight;
    [SerializeField] public GameObject[] RiverStraightRightLeft;
    [SerializeField] public GameObject[] RiverStraightTopBottom;
    [SerializeField] public GameObject[] RiverStraightBottomTop;
    [SerializeField] public GameObject[] RiverCurveLeftTop;
    [SerializeField] public GameObject[] RiverCurveLeftBottom;
    [SerializeField] public GameObject[] RiverCurveTopLeft;
    [SerializeField] public GameObject[] RiverCurveTopRight;
    [SerializeField] public GameObject[] RiverCurveRightTop;
    [SerializeField] public GameObject[] RiverCurveRightBottom;
    [SerializeField] public GameObject[] RiverCurveBottomLeft;
    [SerializeField] public GameObject[] RiverCurveBottomRight;
    [SerializeField] public GameObject[] Start;
    [SerializeField] public GameObject[] End;
    [SerializeField] public GameObject[] EndBackground;

    [Header("Obstacles")]
    [SerializeField] public GameObject[] Building;
    [SerializeField] public GameObject[] Rock;
    [SerializeField] public GameObject[] Tree;
    [SerializeField] public GameObject[] Lake;
    [SerializeField] public GameObject[] Bush;


    private Dictionary<TileType, GameObject[]> TilePrefabsPerType;
    private Dictionary<ObstacleType, GameObject[]> ObstaclePrefabsPerType;

    public TilePrefabs()
    {
        TilePrefabsPerType = new Dictionary<TileType, GameObject[]>();
        ObstaclePrefabsPerType = new Dictionary<ObstacleType, GameObject[]>();
    }

    public void OnBeforeSerialize()
    { }

    public void OnAfterDeserialize()
    {
        TilePrefabsPerType.Clear();
        ObstaclePrefabsPerType.Clear();
        foreach (TileType tt in Enum.GetValues(typeof(TileType)))
        {
            var f = typeof(TilePrefabs).GetField(tt.ToString());
            TilePrefabsPerType.Add(tt, (f?.GetValue(this) as GameObject[]) ?? new GameObject[0]);
        }
        foreach (ObstacleType ot in Enum.GetValues(typeof(ObstacleType)))
        {
            var f = typeof(TilePrefabs).GetField(ot.ToString());
            ObstaclePrefabsPerType.Add(ot, (f?.GetValue(this) as GameObject[]) ?? new GameObject[0]);
        }
    }

    public void CheckTilesAvailable()
    {
        foreach (TileType tt in Enum.GetValues(typeof(TileType)))
        {
            if (TilePrefabsPerType[tt].Length == 0)
                Debug.LogWarning("Tile type " + tt.ToString() + " has no tile-prefabs!");
        }
        foreach (ObstacleType ot in Enum.GetValues(typeof(ObstacleType)))
        {
            if (ObstaclePrefabsPerType[ot].Length == 0)
                Debug.LogWarning("Obstacle type " + ot.ToString() + " has no prefabs!");
        }
    }

    public GameObject[] this[TileType tt] => TilePrefabsPerType[tt];
    public GameObject this[TileType tt, int idx] => TilePrefabsPerType[tt][idx];

    public GameObject[] this[ObstacleType ot] => ObstaclePrefabsPerType[ot];
    public GameObject this[ObstacleType ot, int idx] => ObstaclePrefabsPerType[ot][idx];
}
