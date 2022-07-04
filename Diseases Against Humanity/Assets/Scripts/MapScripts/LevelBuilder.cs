using Assets.Scripts.DefenseScripts;
using Assets.Scripts.MapScripts;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : Singleton<LevelBuilder>
{
    [SerializeField]
    private GameObject[] TilePrefabsGameObjects;
    private TilePrefabs TilePrefabs;

    [SerializeField]
    private GameObject EndTileGameObject;

    [SerializeField]
    private GameObject DarkeningGameObject;

    [SerializeField]
    private Transform _Map;
    public Transform Map => _Map;

    [SerializeField]
    private CameraMovement CameraMovement;

    public Dictionary<GridPoint, TileScript> Tiles { get; private set; }

    private System.Random Random;

    public PathInfo PathInfo { get; private set; }

    public EndTileScript EndTile { get; private set; }

    public void StartLevel(LevelName levelName)
    {
        bool loadSavegame = false;
        if (levelName == LevelName.None) {return; }
        if (levelName == LevelName.Level_Savegame)
        {
            if (Savegame.GetInstance().LevelName == 0)
            {
                levelName = LevelName.Level_1_1;
            }
            else
            {
                loadSavegame = true;
                levelName = Savegame.GetInstance().LevelName;
            }
        }

        this.Random = new System.Random();
        Tiles = new Dictionary<GridPoint, TileScript>();
        TilePrefabs = TilePrefabsGameObjects[(int)Util.GetLevelType(levelName)].GetComponent<TilePrefabsBehavior>().TilePrefabs;
        TilePrefabs.CheckTilesAvailable();
        Util.TileSize = TilePrefabs[TileType.Start, 0].GetComponent<SpriteRenderer>().sprite.bounds.size;
        var ll = CreateLevel(levelName.ToString());
        this.PathInfo = ll.PathInfo;
        GameManager.GetInstance().ResetOnLevelStart(ll.PointsInfo, levelName);

        if (loadSavegame)
        {
            PlaceTowers(Savegame.GetInstance().Towers);
            GameManager.GetInstance().UpdateItemCounts(Savegame.GetInstance().ItemCounts);
            GameManager.GetInstance().ResearchPoints = Savegame.GetInstance().ResearchPoints;
            GameManager.GetInstance().HealthPoints = Savegame.GetInstance().Health;
            WaveManager.GetInstance().CurrentWaveIndex = Savegame.GetInstance().Wave;
            WaveManager.GetInstance().IsWaiting = true;
            GameManager.GetInstance().SavegameLoaded();
        }
    }

    public void ExitLevel()
    {
        this.EndTile = null;
        DestroyOldTiles();
    }

    private LevelLoader CreateLevel(string levelName)
    {
        var ll = new LevelLoader(levelName);
        if (ll.RandomSeed.HasValue)
        {
            this.Random = new System.Random(ll.RandomSeed.Value);
        }

        Util.WorldTopLeftPos = Vector3.zero;
        for (var x = 0; x < ll.HorizontalTileCount; x++)
        {
            for (var y = 0; y < ll.VerticalTileCount; y++)
            {
                var p = new GridPoint(x, y);
                var t = CreateTile(ll[x, y], p, ll.IsInsideBuildableRect(p));
                Tiles.Add(p, t);
            }
        }

        foreach (var o in ll.Obstacles)
        {
            var points = CreateObstacle(o);
            foreach (var p in points)
            {
                Tiles[p].CanBuild = false;
            }
        }

        CreateDarkening(ll);

        var outerTopLeftPosition = Util.WorldTopLeftPos;
        var outerBottomRightPosition = Util.GetWorldPos(ll.HorizontalTileCount, ll.VerticalTileCount);
        var innerTopLeftPosition = Util.GetWorldPos(ll.MarginLeft, ll.MarginTop);
        var innerBottomRightPosition = Util.GetWorldPos(ll.HorizontalTileCount - ll.MarginRight, ll.VerticalTileCount - ll.MarginBottom);
        CameraMovement.SetCameraLimits(outerTopLeftPosition, outerBottomRightPosition, innerTopLeftPosition, innerBottomRightPosition);

        return ll;
    }

    private void DestroyOldTiles()
    {
        for (int i = 0; i < Map.childCount; i++)
        {
            Destroy(Map.GetChild(i).gameObject);
        }
    }

    private void CreateDarkening(LevelLoader ll)
    {
        if (ll.MarginTop > 0)
        {
            var top = Instantiate(this.DarkeningGameObject);
            top.transform.localPosition = new Vector3(Util.WorldTopLeftPos.x * Util.TileSize.x, Util.WorldTopLeftPos.y * Util.TileSize.y);
            top.transform.localScale = new Vector3(ll.HorizontalTileCount, ll.MarginTop);
            top.transform.SetParent(Map);
            top.GetComponent<SpriteRenderer>().sortingOrder = Constants.DarkeningSortingOrder;
        }

        if (ll.MarginBottom > 0)
        {
            var bottom = Instantiate(this.DarkeningGameObject);
            bottom.transform.localPosition = new Vector3(Util.WorldTopLeftPos.x * Util.TileSize.x, (Util.WorldTopLeftPos.y - ll.VerticalTileCount + ll.MarginBottom) * Util.TileSize.y);
            bottom.transform.localScale = new Vector3(ll.HorizontalTileCount, ll.MarginBottom);
            bottom.transform.SetParent(Map);
            bottom.GetComponent<SpriteRenderer>().sortingOrder = Constants.DarkeningSortingOrder;
        }

        if (ll.MarginLeft > 0)
        {
            var left = Instantiate(this.DarkeningGameObject);
            left.transform.localPosition = new Vector3(Util.WorldTopLeftPos.x * Util.TileSize.x, (Util.WorldTopLeftPos.y - ll.MarginTop) * Util.TileSize.y);
            left.transform.localScale = new Vector3(ll.MarginLeft, (ll.VerticalTileCount - ll.MarginBottom - ll.MarginTop));
            left.transform.SetParent(Map);
            left.GetComponent<SpriteRenderer>().sortingOrder = Constants.DarkeningSortingOrder;
        }

        if (ll.MarginRight > 0)
        {
            var right = Instantiate(this.DarkeningGameObject);
            right.transform.localPosition = new Vector3((Util.WorldTopLeftPos.x + ll.HorizontalTileCount - ll.MarginRight) * Util.TileSize.x, (Util.WorldTopLeftPos.y - ll.MarginTop) * Util.TileSize.y);
            right.transform.localScale = new Vector3(ll.MarginRight, ll.VerticalTileCount - ll.MarginBottom - ll.MarginTop);
            right.transform.SetParent(Map);
            right.GetComponent<SpriteRenderer>().sortingOrder = Constants.DarkeningSortingOrder;
        }
    }

    private TileScript CreateTile(TileType type, GridPoint p, bool isInsideBuildableRect)
    {
        TileScript ts = null;
        if (type == TileType.End)
        {
            var ets = Instantiate(this.EndTileGameObject, this.Map).GetComponent<EndTileScript>();
            ets.Prefabs = TilePrefabs[TileType.End];
            ts = ets;
            this.EndTile = ets;
            CreateTile(TileType.EndBackground, p, isInsideBuildableRect);
        }
        else
        {
            ts = Instantiate(TilePrefabs[type].SelectRandom(this.Random)).GetComponent<TileScript>();
        }

        ts.Init(p, Util.GetWorldPos(p.X, p.Y), Map, type == TileType.Buildable && isInsideBuildableRect, !isInsideBuildableRect, IsWay(type));
        return ts;
    }

    private bool IsWay(TileType type)
    {
        return type == TileType.BridgeStraightHorizontalBottomTop
            || type == TileType.BridgeStraightHorizontalTopBottom
            || type == TileType.BridgeStraightVerticalLeftRight
            || type == TileType.BridgeStraightVerticalRightLeft
            || type == TileType.WayCrossHorizontal
            || type == TileType.WayCrossVertical
            || type == TileType.WayCurveLeftBottom
            || type == TileType.WayCurveLeftTop
            || type == TileType.WayCurveRightBottom
            || type == TileType.WayCurveRightTop
            || type == TileType.WayStraightHorizontal
            || type == TileType.WayStraightVertical;
    }

    private List<GridPoint> CreateObstacle(Obstacle o)
    {
        GameObject gameObject;
        if (o.IndexOverride.HasValue)
            gameObject = TilePrefabs[o.Type][o.IndexOverride.Value];
        else
            gameObject = TilePrefabs[o.Type].SelectRandom(this.Random);

        var os = Instantiate(gameObject).GetComponent<ObstacleScript>();
        os.Init(o, Util.GetWorldPos(o.Position.X, o.Position.Y), Map);
        return os.GetAffectedGridPoints();
    }

    private void PlaceTowers(List<TowerInfo> towers)
    {
        var gm = GameManager.GetInstance();
        foreach (var tower in towers)
        {
            Tiles[tower.gp].CanBuild = false;
            gm.ChangeTowerCount(true, gm.TowerPrefabs[tower.type].TowerBase());

            var ts = Instantiate(gm.TowerPrefabs[tower.type]).GetComponent<TowerBase>();
            if (ts is DirectionDependentTower ddt && !ddt.IgnoreDirection)
            {
                ddt.Direction = (Direction)tower.direction;
            }
            ts.Init(tower.gp, Map);
            if (ts is MaskBarrier mb)
            {
                mb.ActualHealth = tower.hp;
            }
        }
    }
}

public enum LevelName
{
    None = int.MinValue,
    Level_1_1 = 1,
    Level_1_2 = 2,
    Level_1_3 = 3,
    Level_2_1 = 4,
    Level_2_2 = 5,
    Level_2_3 = 6,
    Level_3_1 = 7,
    Level_3_2 = 8,
    Level_3_3 = 9,
    Level_Savegame = 10,
    Level_4_1 = 11,
    Level_4_2 = 12,
    Level_4_3 = 13,
}

public enum LevelType
{
    Forest = 0,
    Village = 1,
    City = 2
}