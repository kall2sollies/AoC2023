namespace Day10_01;

public class Program
{
    public static void Main()
    {

    }
}

public class Map
{
    public Tile[,] Tiles { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    private readonly string[] _lines;

    public Map(string[] lines)
    {
        _lines = lines;
        BuildMap();
    }

    private void BuildMap()
    {
        Height = _lines.Length;
        Width = _lines[0].Length;

        Tiles = new Tile[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            string line = _lines[y];
            for (int x = 0; x < Width; x++)
            {
                char c = line[x];

                Tile tile = TileFactory.CreateTile(c, this, x, y);

                Tiles[x, y] = tile;
            }
        }
    }
}

public static class TileFactory
{
    public static Tile CreateTile(char c, Map map, int x, int y)
    {
        return c switch
        {
            '.' => new Ground(map, x, y),
            'F' => new Pipe(map, x, y, new[] {Directions.S, Directions.E}),
            '-' => new Pipe(map, x, y, new[] {Directions.W, Directions.E}),
            '7' => new Pipe(map, x, y, new[] {Directions.W, Directions.S}),
            '|' => new Pipe(map, x, y, new[] {Directions.N, Directions.S}),
            'J' => new Pipe(map, x, y, new[] {Directions.N, Directions.W}),
            'L' => new Pipe(map, x, y, new[] {Directions.N, Directions.E}),
            'S' => new Pipe(map, x, y, Array.Empty<Directions>()) {IsStartTile = true},
            _ => throw new Exception("Unknown tile")
        };
    }
}

public abstract class Tile
{
    private readonly Map _map;

    public int X { get; set; }
    public int Y { get; set; }

    public Directions[] Connexions { get; set; }
    public bool HasConnexions => Connexions.Length > 0;
    public virtual bool IsStartTile { get; init; }

    public bool IsLastColumn => X == _map.Width - 1;
    public bool IsLastRow => Y == _map.Height - 1;

    protected Tile(Map map, int x, int y)
    {
        _map = map;
        X = x;
        Y = y;
    }

    public Tile GetRelativeTile(int xOffset, int yOffset)
    {
        if (X + xOffset >= _map.Width) return null;
        if (X + xOffset < 0) return null;
        if (Y + yOffset >= _map.Height) return null;
        if (Y + yOffset < 0) return null;

        return _map.Tiles[X + xOffset, Y + yOffset];
    }

    public Tile GetTileAt(Directions direction)
    {
        return direction switch
        {
            Directions.E => East,
            Directions.S => South,
            Directions.N => North,
            Directions.W => West,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public Tile North => GetRelativeTile(0, -1);
    public Tile East => GetRelativeTile(1, 0);
    public Tile South => GetRelativeTile(0, 1);
    public Tile West => GetRelativeTile(-1, 0);

    public Tile FirstOfNextLine => GetRelativeTile(-X, 1);

    public List<Tile> Neighbours
    {
        get
        {
            List<Tile> cells = new() { North, East, South, West };
            return cells.Where(x => x != null).ToList();
        }
    }
}

public class Ground : Tile
{
    public Ground(Map map, int x, int y) : base(map, x, y)
    {

    }
}

public class Pipe : Tile
{
    public Pipe(Map map, int x, int y, Directions[] connexions) : base(map, x, y)
    {
        Connexions = connexions;
    }
}

public enum Directions
{
    N, E, S, W
}
