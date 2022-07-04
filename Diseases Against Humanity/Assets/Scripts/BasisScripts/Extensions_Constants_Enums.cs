using UnityEngine;

public static class Extensions
{
    public static T SelectRandom<T>(this T[] ts, System.Random r)
    {
        return ts[r.Next(ts.Length)];
    }

    private static float NextFloat(this System.Random r, float min, float max)
    {
        return (float)(r.NextDouble() * (max - min)) + min;
    }

    public static Vector3 Randomize(this Vector3 pos, float xAmount, float yAmount, float zAmount, System.Random r)
    {
        return new Vector3(pos.x + r.NextFloat(-xAmount, xAmount), pos.y + r.NextFloat(-yAmount, yAmount), pos.z + r.NextFloat(-zAmount, zAmount));
    }
}

public static class Constants
{
    public const int MapTileSortingOrder = 0;
    public const int CrowdTileSortingOrder = 2;
    public const int TowerSortingOrder = 5;
    public const int TowerRotatingSortingOrder = 10;
    public const int ProjectileSortingOrder = 15;
    public const int EnemySortingOrder = 20;
    public const int ObstaclesSortingOrder = 30;
    public const int DarkeningSortingOrder = 40;
    public const int HoverSortingLayer = 50;

    public const string EnemyTag = "Enemy";
    public const string TowerTag = "Tower";
    public const string ItemTag = "Item";
}

public enum Direction : int
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}