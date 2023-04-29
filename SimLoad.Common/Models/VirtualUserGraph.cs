namespace SimLoad.Common.Models;

public class VirtualUserGraph
{
    public List<Point> Points { get; set; } = new();
}

public class Point
{
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point()
    {
        
    }

    public int X { get; set; }
    public int Y { get; set; }
}