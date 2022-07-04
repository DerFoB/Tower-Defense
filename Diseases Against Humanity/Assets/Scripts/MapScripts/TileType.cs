public enum TileType
{
    WayStraightHorizontal,
    WayStraightVertical,
    BridgeStraightHorizontalTopBottom,
    BridgeStraightHorizontalBottomTop,
    BridgeStraightVerticalLeftRight,
    BridgeStraightVerticalRightLeft,
    WayCurveLeftTop,
    WayCurveLeftBottom,
    WayCurveRightTop,
    WayCurveRightBottom,
    WayCrossHorizontal,
    WayCrossVertical,
    Buildable,
    RiverStraightLeftRight,
    RiverStraightRightLeft,
    RiverStraightTopBottom,
    RiverStraightBottomTop,
    RiverCurveLeftTop,
    RiverCurveLeftBottom,
    RiverCurveTopLeft,
    RiverCurveTopRight,
    RiverCurveRightTop,
    RiverCurveRightBottom,
    RiverCurveBottomLeft,
    RiverCurveBottomRight,
    Start,
    End,
    EndBackground,
}

public static class TileTypeHelper
{
    public static readonly TileType[] StraightWayTiles = { TileType.WayStraightHorizontal, TileType.WayStraightVertical };
    public static readonly TileType[] StraightRiverTiles = { TileType.RiverStraightBottomTop, TileType.RiverStraightTopBottom, TileType.RiverStraightLeftRight, TileType.RiverStraightRightLeft };
}

public enum ObstacleType
{
    Building,
    Rock,
    Tree,
    Lake,
    Bush,
}
