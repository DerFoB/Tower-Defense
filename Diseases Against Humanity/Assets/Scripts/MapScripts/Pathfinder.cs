using System.Collections.Generic;
using System.Linq;

public class Waypoint
{
    public Waypoint[] Nexts { get; private set; }
    public Waypoint Next => Nexts[NextCounter++ % Nexts.Length];
    public bool HasNext => Nexts.Length > 0;
    public int NextCounter { get; private set; }

    public GridPoint Location { get; private set; }
    public bool IsCrossing { get; private set; }

    public Waypoint(GridPoint location, bool isCrossing)
    {
        this.Location = location;
        this.IsCrossing = isCrossing;
        SetNextWaypoints(null, true);
    }

    public void SetNextWaypoints(Waypoint[] nexts, bool resetCounter)
    {
        this.Nexts = nexts ?? new Waypoint[0];
        if (resetCounter) this.NextCounter = 0;
    }
}

public class PathInfo
{
    public Waypoint Start { get; private set; }
    public Waypoint End { get; private set; }

    public PathInfo(Waypoint start)
    {
        this.Start = start;
        this.End = FindEnd(start);
    }

    private Waypoint FindEnd(Waypoint wp)
    {
        while (wp.HasNext)
            wp = wp.Nexts[0];   // Do not use wp.Next as this will affect wp.NextCounter
        return wp;
    }

    public static PathInfo FromGridPoints(List<GridPoint> waypoints)
    {
        Waypoint[] nextWaypoints = null;
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            var wp = new Waypoint(waypoints[i], waypoints.Count(w => w == waypoints[i]) > 1);
            wp.SetNextWaypoints(nextWaypoints, true);
            nextWaypoints = new Waypoint[1] { wp };
        }
        return new PathInfo(nextWaypoints[0]);
    }
}

public class PathFinder
{
    private PathInfo PathInfo;

    public Waypoint NextWaypoint { get; private set; }
    public GridPoint TargetPoint => NextWaypoint.Location;
    public bool IsAtEnd => NextWaypoint == PathInfo.End;

    private HashSet<GridPoint> VisitedCrossings;
    public bool IsBelowCrossingBridge { get; private set; }
    public int TravelledTiles { get; set; }

    public PathFinder(PathInfo pathInfo, PathFinder pathFinder = null)
    {
        this.PathInfo = pathInfo;
        this.NextWaypoint = this.PathInfo.Start;
        this.TravelledTiles = pathFinder?.TravelledTiles ?? 0;
        this.VisitedCrossings = new HashSet<GridPoint>();
        if(pathFinder != null)
        {
            foreach (var vc in pathFinder.VisitedCrossings)
                this.VisitedCrossings.Add(vc);
        }
    }

    public void UpdateTargetPoint()
    {
        if (this.NextWaypoint.HasNext)
        {
            this.NextWaypoint = this.NextWaypoint.Next;
            this.TravelledTiles++;
            if (this.NextWaypoint.IsCrossing)
            {
                var alreadyVisited = this.VisitedCrossings.Contains(this.NextWaypoint.Location);
                if (!alreadyVisited)
                    this.VisitedCrossings.Add(this.NextWaypoint.Location);
                this.IsBelowCrossingBridge = !alreadyVisited;
            }
            else
            {
                this.IsBelowCrossingBridge = false;
            }
        }
    }
}