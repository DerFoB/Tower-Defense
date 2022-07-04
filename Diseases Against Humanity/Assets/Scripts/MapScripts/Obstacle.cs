public class Obstacle
{
    public GridPoint Position { get; set; }
    public ObstacleType Type { get; set; }
    public int? IndexOverride { get; set; }

    public Obstacle(GridPoint pos, ObstacleType ot, int? indexOverride)
    {
        this.Position = pos;
        this.Type = ot;
        this.IndexOverride = indexOverride;
    }
}
