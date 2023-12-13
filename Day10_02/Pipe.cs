namespace Day10_02;

public class Pipe : Tile
{
    public Pipe(Map map, int x, int y, char c, Directions[] connexions) : base(map, x, y, c)
    {
        Connexions = connexions;
    }
}