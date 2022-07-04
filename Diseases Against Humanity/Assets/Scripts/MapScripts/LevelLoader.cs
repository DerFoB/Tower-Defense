using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class LevelLoader
{
    public int HorizontalTileCount { get; private set; }
    public int VerticalTileCount { get; private set; }

    public int MarginLeft { get; private set; }
    public int MarginTop { get; private set; }
    public int MarginRight { get; private set; }
    public int MarginBottom { get; private set; }

    public int? RandomSeed { get; private set; }

    private TileType[,] Tiles;

    public List<Obstacle> Obstacles { get; private set; }

    public PathInfo PathInfo { get; private set; }

    public PointsInfo PointsInfo { get; private set; }

    public TileType this[int x, int y]
    {
        get => Tiles[x, y];
        private set
        {
            if (x < 0)
                throw new Exception("x < 0");
            if (x >= HorizontalTileCount)
                throw new Exception("x >= HorizontalTileCount");
            if (y < 0)
                throw new Exception("y < 0");
            if (y >= VerticalTileCount)
                throw new Exception("y >= VerticalTileCount");
            Tiles[x, y] = value;
        }
    }


    public LevelLoader(string levelName)
    {
        var textData = Resources.Load("Level/" + levelName) as TextAsset;
        var textArray = textData.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < textArray.Length; i++)
        {
            textArray[i] = textArray[i].Trim(' ', '\r', '\n').Trim(' ', '\r', '\n');
        }
        var textLinesList = new List<string>();
        foreach (var line in textArray)
        {
            if (!string.IsNullOrWhiteSpace(line)) // Ignore empty lines
                textLinesList.Add(line);
        }

        var textLines = textArray.ToArray();


        var dimensionsString = textLines[0].Split('x', ' ', ',');
        HorizontalTileCount = int.Parse(dimensionsString[0].Trim());
        VerticalTileCount = int.Parse(dimensionsString[1].Trim());
        MarginLeft = int.Parse(dimensionsString[2].Trim());
        MarginTop = int.Parse(dimensionsString[3].Trim());
        MarginRight = int.Parse(dimensionsString[4].Trim());
        MarginBottom = int.Parse(dimensionsString[5].Trim());

        Tiles = new TileType[HorizontalTileCount, VerticalTileCount];
        Obstacles = new List<Obstacle>();
        for (int x = 0; x < HorizontalTileCount; x++)
        {
            for (int y = 0; y < VerticalTileCount; y++)
            {
                this[x, y] = TileType.Buildable;
            }
        }

        PointsInfo = new PointsInfo();

        int lineNo = 1;
        while (lineNo < textLines.Length)
        {
            ParseLevelText(textLines, ref lineNo);
        }
    }

    internal bool IsInsideBuildableRect(GridPoint p)
    {
        return p.X >= MarginLeft && p.X < HorizontalTileCount - MarginRight
            && p.Y >= MarginTop && p.Y < VerticalTileCount - MarginBottom;
    }

    private void ParseLevelText(string[] textLines, ref int lineNo)
    {
        var line = textLines[lineNo++];
        switch (line)
        {
            case "# River":
                ParseRiver(textLines, ref lineNo);
                Debug.Log("Parsed River");
                break;
            case "# Path":
                ParsePath(textLines, ref lineNo);
                Debug.Log("Parsed Path");
                break;
            case "# Obstacles":
                ParseObstacles(textLines, ref lineNo);
                Debug.Log("Parsed Obstacles");
                break;
            case "# Fixes":
                ParseFixes(textLines, ref lineNo);
                Debug.Log("Parsed Fixes");
                break;
            case "# Random":
                ParseRandom(textLines, ref lineNo);
                Debug.Log("Parsed Random");
                break;
            case "# Research Points":
                ParseResearchPoints(textLines, ref lineNo);
                Debug.Log("Parsed Research Points");
                break;
            case "# Research Points Multiplier":
                ParseResearchPointsMultiplier(textLines, ref lineNo);
                Debug.Log("Parsed Research Points Multiplier");
                break;
        }
    }

    private void ParseRiver(string[] textLines, ref int lineNo)
    {
        while (lineNo < textLines.Length && !string.IsNullOrWhiteSpace(textLines[lineNo]) && !textLines[lineNo].StartsWith("#"))
        {
            var dirpath = ParseDirectionPath(textLines[lineNo++]);
            var p = dirpath.Item1;
            var lastDir = dirpath.Item2[0];
            foreach (var dir in dirpath.Item2)
            {
                var tt = TileType.Buildable;
                if (dir == lastDir)
                    tt = GetStraightRiverTileType(dir);
                else
                    tt = GetCurveRiverTileType(dir, lastDir);
                this[p.X, p.Y] = tt;
                p = GetNextGridPos(p, dir);
                lastDir = dir;
            }
            this[p.X, p.Y] = GetStraightRiverTileType(lastDir);
        }
    }

    private GridPoint GetNextGridPos(GridPoint p, Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return new GridPoint(p.X, p.Y - 1);
            case Direction.East:
                return new GridPoint(p.X + 1, p.Y);
            case Direction.South:
                return new GridPoint(p.X, p.Y + 1);
            case Direction.West:
                return new GridPoint(p.X - 1, p.Y);
        }
        return p;
    }

    private void ParsePath(string[] textLines, ref int lineNo)
    {
        var path = textLines[lineNo++];
        var dirpath = ParseDirectionPath(path);
        var waypoints = new List<GridPoint>();

        var p = dirpath.Item1;
        waypoints.Add(p);

        var lastDir = dirpath.Item2[0];
        foreach (var dir in dirpath.Item2)
        {
            var tt = Tiles[p.X, p.Y];
            if (dir == lastDir)
                tt = GetStraightWayTileType(dir, tt);
            else
                tt = GetCurveWayTileType(dir, lastDir);
            this[p.X, p.Y] = tt;
            p = GetNextGridPos(p, dir);
            waypoints.Add(p);
            lastDir = dir;
        }

        Tiles[dirpath.Item1.X, dirpath.Item1.Y] = TileType.Start;
        Tiles[p.X, p.Y] = TileType.End;
        PathInfo = PathInfo.FromGridPoints(waypoints);
    }

    private void ParseObstacles(string[] textLines, ref int lineNo)
    {
        var l = ParseTileChanges<ObstacleType>(textLines, ref lineNo);
        foreach (var o in l)
        {
            Obstacles.Add(new Obstacle(o.Item1, o.Item2, o.Item3));
        }
    }

    private void ParseFixes(string[] textLines, ref int lineNo)
    {
        var l = ParseTileChanges<TileType>(textLines, ref lineNo);
        foreach (var f in l)
        {
            this[f.Item1.X, f.Item1.Y] = f.Item2;
        }
    }

    private void ParseRandom(string[] textLines, ref int lineNo)
    {
        if (int.TryParse(textLines[lineNo], out var seed))
            this.RandomSeed = seed;
        else if (!string.IsNullOrWhiteSpace(textLines[lineNo]))
            this.RandomSeed = textLines[lineNo].GetHashCode();
        else
            this.RandomSeed = null;
        lineNo++;
    }

    private void ParseResearchPoints(string[] textLines, ref int lineNo)
    {
        if (int.TryParse(textLines[lineNo], out var rp))
            this.PointsInfo.StartPoints = rp;
        else
            this.PointsInfo.StartPoints = 0;
        lineNo++;
    }

    private void ParseResearchPointsMultiplier(string[] textLines, ref int lineNo)
    {
        var values = textLines[lineNo++].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < values.Length; i++)
        {
            if (double.TryParse(values[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var x))
                switch (i)
                {
                    case 0:
                        this.PointsInfo.KillMultiplier = x;
                        break;
                    case 1:
                        this.PointsInfo.KillResearchMultiplier = x;
                        break;
                    case 2:
                        this.PointsInfo.WaveBonus = (int)x;
                        break;
                }
        }
    }

    private List<Tuple<GridPoint, T, int?>> ParseTileChanges<T>(string[] textLines, ref int lineNo)
    {
        var l = new List<Tuple<GridPoint, T, int?>>();
        while (lineNo < textLines.Length && !string.IsNullOrWhiteSpace(textLines[lineNo]) && !textLines[lineNo].StartsWith("#"))
        {
            var c = textLines[lineNo++].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var posStr = c[0].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var pos = new GridPoint(int.Parse(posStr[0]), int.Parse(posStr[1]));
            int? indexOverride = null;
            if (c.Length == 3)
                indexOverride = int.Parse(c[2]);
            l.Add(new Tuple<GridPoint, T, int?>(pos, (T)Enum.Parse(typeof(T), c[1]), indexOverride));
        }
        return l;
    }

    private Tuple<GridPoint, List<Direction>> ParseDirectionPath(string line)
    {
        var parts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        var start = parts[0].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        var sp = new GridPoint(int.Parse(start[0]), int.Parse(start[1]));
        var dirs = new List<Direction>();
        for (int p = 1; p < parts.Length; p++)
        {
            var part = parts[p];
            var dir = Direction.North;
            switch (part[0])
            {
                case 'N':
                    dir = Direction.North;
                    break;
                case 'E':
                    dir = Direction.East;
                    break;
                case 'S':
                    dir = Direction.South;
                    break;
                case 'W':
                    dir = Direction.West;
                    break;
            }
            if (!int.TryParse(part.Substring(1), out int count))
                count = 1;
            for (int i = 0; i < count; i++)
                dirs.Add(dir);
        }
        return new Tuple<GridPoint, List<Direction>>(sp, dirs);
    }

    #region Path Helpers

    private TileType GetStraightRiverTileType(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return TileType.RiverStraightBottomTop;
            case Direction.East:
                return TileType.RiverStraightLeftRight;
            case Direction.South:
                return TileType.RiverStraightTopBottom;
            case Direction.West:
                return TileType.RiverStraightRightLeft;
        }
        return TileType.Buildable;
    }

    private TileType GetCurveRiverTileType(Direction dir, Direction lastDir)
    {
        switch (lastDir)
        {
            case Direction.North:
                switch (dir)
                {
                    case Direction.East:
                        return TileType.RiverCurveBottomRight;
                    case Direction.West:
                        return TileType.RiverCurveBottomLeft;
                }
                break;
            case Direction.East:
                switch (dir)
                {
                    case Direction.North:
                        return TileType.RiverCurveLeftTop;
                    case Direction.South:
                        return TileType.RiverCurveLeftBottom;
                }
                break;
            case Direction.South:
                switch (dir)
                {
                    case Direction.East:
                        return TileType.RiverCurveTopRight;
                    case Direction.West:
                        return TileType.RiverCurveTopLeft;
                }
                break;
            case Direction.West:
                switch (dir)
                {
                    case Direction.North:
                        return TileType.RiverCurveRightTop;
                    case Direction.South:
                        return TileType.RiverCurveRightBottom;
                }
                break;
        }
        return TileType.Buildable;
    }

    private TileType GetStraightWayTileType(Direction dir, TileType tt)
    {
        bool bridge = TileTypeHelper.StraightRiverTiles.Contains(tt);
        bool cross = TileTypeHelper.StraightWayTiles.Contains(tt);
        switch (dir)
        {
            case Direction.North:
            case Direction.South:
                return bridge ? (tt == TileType.RiverStraightLeftRight ? TileType.BridgeStraightVerticalLeftRight : TileType.BridgeStraightVerticalRightLeft) : (cross ? TileType.WayCrossVertical : TileType.WayStraightVertical);
            case Direction.East:
            case Direction.West:
                return bridge ? (tt == TileType.RiverStraightTopBottom ? TileType.BridgeStraightHorizontalTopBottom : TileType.BridgeStraightHorizontalBottomTop) : (cross ? TileType.WayCrossHorizontal : TileType.WayStraightHorizontal);
        }
        return TileType.Buildable;
    }

    private TileType GetCurveWayTileType(Direction dir, Direction lastDir)
    {
        switch (lastDir)
        {
            case Direction.North:
                switch (dir)
                {
                    case Direction.East:
                        return TileType.WayCurveRightBottom;
                    case Direction.West:
                        return TileType.WayCurveLeftBottom;
                }
                break;
            case Direction.East:
                switch (dir)
                {
                    case Direction.North:
                        return TileType.WayCurveLeftTop;
                    case Direction.South:
                        return TileType.WayCurveLeftBottom;
                }
                break;
            case Direction.South:
                switch (dir)
                {
                    case Direction.East:
                        return TileType.WayCurveRightTop;
                    case Direction.West:
                        return TileType.WayCurveLeftTop;
                }
                break;
            case Direction.West:
                switch (dir)
                {
                    case Direction.North:
                        return TileType.WayCurveRightTop;
                    case Direction.South:
                        return TileType.WayCurveRightBottom;
                }
                break;
        }
        return TileType.Buildable;
    }

    #endregion
}
