[System.Diagnostics.DebuggerDisplay("{X}, {Y}")]
public struct GridPoint
{
    public int X { get; set; }
    public int Y { get; set; }

    public GridPoint(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static bool operator ==(GridPoint a, GridPoint b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(GridPoint a, GridPoint b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is GridPoint point &&
               X == point.X &&
               Y == point.Y;
    }

    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return $"{X}, {Y}";
    }
}
