using UnityEngine;
using static ButtonTypes;

public static class Util
{
    public static Vector3 TileSize;
    public static Vector3 WorldTopLeftPos;

    public static Vector3 GetWorldPosCentered(int x, int y)
    {
        return new Vector3(TileSize.x * (x + 0.5f) + WorldTopLeftPos.x, -TileSize.y * (y + 0.5f) + WorldTopLeftPos.y);
    }

    public static Vector3 GetWorldPosCentered(GridPoint gp) => GetWorldPosCentered(gp.X, gp.Y);

    public static Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(TileSize.x * x + WorldTopLeftPos.x, -TileSize.y * y + WorldTopLeftPos.y);
    }

    public static Vector3 GetWorldPos(GridPoint gp) => GetWorldPos(gp.X, gp.Y);

    public static GridPoint GetTilePosFromCentered(float x, float y)
    {
        return new GridPoint(Mathf.RoundToInt((x - WorldTopLeftPos.x) / TileSize.x - 0.5f),
                             -Mathf.RoundToInt((y - WorldTopLeftPos.y) / TileSize.y + 0.5f));
    }

    public static GridPoint GetTilePosFromCentered(Vector3 pos) => GetTilePosFromCentered(pos.x, pos.y);

    public static LevelName GetLevelName(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.StartLevel_1_1:
                return LevelName.Level_1_1;
            case ButtonType.StartLevel_1_2:
                return LevelName.Level_1_2;
            case ButtonType.StartLevel_1_3:
                return LevelName.Level_1_3;
            case ButtonType.StartLevel_2_1:
                return LevelName.Level_2_1;
            case ButtonType.StartLevel_2_2:
                return LevelName.Level_2_2;
            case ButtonType.StartLevel_2_3:
                return LevelName.Level_2_3;
            case ButtonType.StartLevel_3_1:
                return LevelName.Level_3_1;
            case ButtonType.StartLevel_3_2:
                return LevelName.Level_3_2;
            case ButtonType.StartLevel_3_3:
                return LevelName.Level_3_3;
            case ButtonType.StartLevel_4_1:
                return LevelName.Level_4_1;
            case ButtonType.StartLevel_4_2:
                return LevelName.Level_4_2;
            case ButtonType.StartLevel_4_3:
                return LevelName.Level_4_3;
            case ButtonType.StartLevel_Savegame:
                return LevelName.Level_Savegame;
            default:
               return LevelName.None;
        }
    }

    public static LevelType GetLevelType(LevelName levelName)
    {
        switch (levelName)
        {
            case LevelName.Level_1_1:
            case LevelName.Level_1_2:
            case LevelName.Level_1_3:
            case LevelName.Level_4_1:
            case LevelName.Level_4_2:
            case LevelName.Level_4_3:
                return LevelType.Forest;
            case LevelName.Level_2_1:
            case LevelName.Level_2_2:
            case LevelName.Level_2_3:
                return LevelType.Village;
            case LevelName.Level_3_1:
            case LevelName.Level_3_2:
            case LevelName.Level_3_3:
                return LevelType.City;
            default:
                return LevelType.Forest;
        }
    }

    public static bool HasMovedSignificantly(Vector3 start, Vector3 end)
    {
        return (end - start).magnitude > 0.05;
    }

    public static GridPoint MoveGridPoint(GridPoint origin, Direction direction, int distance = 1)
    {
        switch (direction)
        {
            case Direction.North:
                return new GridPoint(origin.X, origin.Y - distance);
            case Direction.East:
                return new GridPoint(origin.X + distance, origin.Y);
            case Direction.South:
                return new GridPoint(origin.X, origin.Y + distance);
            case Direction.West:
                return new GridPoint(origin.X - distance, origin.Y);
        }
        return origin;
    }

    public static Direction TurnLeft(this Direction direction) => (Direction)(((int)direction + 3) % 4);
    public static Direction TurnRight(this Direction direction) => (Direction)(((int)direction + 1) % 4);

    public static TowerBase TowerBase(this GameObject go) => go.GetComponent<TowerBase>();
    public static EnemyBase EnemyBase(this GameObject go) => go.GetComponent<EnemyBase>();

    public static bool RunsOnDesktop
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsEditor
                || Application.platform == RuntimePlatform.WindowsPlayer
                || Application.platform == RuntimePlatform.OSXEditor
                || Application.platform == RuntimePlatform.OSXPlayer;
        }
    }

    public static bool RunsOnAndroid => Application.platform == RuntimePlatform.Android;
}
