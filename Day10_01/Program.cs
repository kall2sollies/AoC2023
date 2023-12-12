using System.Text;

namespace Day10_01;

public class Program
{
    public static void Main()
    {
        Map map = new(File.ReadAllLines("input.txt"));
        Console.WriteLine(map);

        Tile currentTile = map.StartTile;

        int steps = 0;
        while (true)
        {
            currentTile = currentTile.Next();
            steps++;
            if (currentTile.IsStartTile) break;
        }

        var halfSteps = steps / 2;

        Console.WriteLine(steps);
    }
}

public class Map
{
    public Tile[,] Tiles { get; private set; }
    public List<Tile> TileList { get; private set; } = new();
    public int Height { get; private set; }
    public int Width { get; private set; }

    public Tile StartTile => TileList.First(x => x.IsStartTile);

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
                TileList.Add(tile);
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(Tiles[x, y]);
            }

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}

public static class TileFactory
{
    public static Tile CreateTile(char c, Map map, int x, int y)
    {
        return c switch
        {
            '.' => new Ground(map, x, y, c),
            'F' => new Pipe(map, x, y, c, new[] {Directions.S, Directions.E}),
            '-' => new Pipe(map, x, y, c, new[] {Directions.W, Directions.E}),
            '7' => new Pipe(map, x, y, c, new[] {Directions.W, Directions.S}),
            '|' => new Pipe(map, x, y, c, new[] {Directions.N, Directions.S}),
            'J' => new Pipe(map, x, y, c, new[] {Directions.N, Directions.W}),
            'L' => new Pipe(map, x, y, c, new[] {Directions.N, Directions.E}),
            'S' => new Pipe(map, x, y, c, new[] {Directions.N, Directions.E, Directions.S, Directions.W}) {IsStartTile = true},
            _ => throw new Exception("Unknown tile")
        };
    }
}

public abstract class Tile
{
    private readonly Map _map;
    private readonly char _char;

    public int X { get; set; }
    public int Y { get; set; }

    public Directions[] Connexions { get; set; }
    public bool HasConnexions => Connexions.Length > 0;
    public virtual bool IsStartTile { get; init; }

    public bool IsLastColumn => X == _map.Width - 1;
    public bool IsLastRow => Y == _map.Height - 1;

    public Guid Id { get; set; }

    protected Tile(Map map, int x, int y, char c)
    {
        _map = map;
        _char = c;
        X = x;
        Y = y;
        Id = Guid.NewGuid();
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

    public bool HasDirection(Directions direction) => Connexions.Contains(direction);

    public bool HasConnectionTo(Directions direction)
    {
        if (!HasConnexions) return false;

        if (!HasDirection(direction)) return false;

        Tile tileAtDirection = GetTileAt(direction);

        if (tileAtDirection == null) return false;

        return tileAtDirection.HasDirection(GetOppositeDirection(direction));
    }

    public bool IsNeighbourOf(Tile other)
    {
        return Neighbours.Select(x => x.Id).Contains(other.Id);
    }

    public bool HasConnectionTo(Tile other)
    {
        if (!IsNeighbourOf(other)) return false;

        Directions[] directions = {Directions.N, Directions.E, Directions.W, Directions.S};
        return directions.Any(d => HasConnectionTo(d) && other.HasConnectionTo(GetOppositeDirection(d)));
    }

    private Directions GetOppositeDirection(Directions direction)
    {
        return direction switch
        {
            Directions.N => Directions.S,
            Directions.E => Directions.W,
            Directions.S => Directions.N,
            Directions.W => Directions.E,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public Tile[] GetConnectedNeighbours()
    {
        return Neighbours.Where(x => x != null && x.HasConnectionTo(this)).ToArray();
    }

    public Tile Next()
    {
        if (HasConnectionTo(Directions.N)) return GetTileAt(Directions.N);
        if (HasConnectionTo(Directions.E)) return GetTileAt(Directions.E);
        if (HasConnectionTo(Directions.S)) return GetTileAt(Directions.S);
        if (HasConnectionTo(Directions.W)) return GetTileAt(Directions.W);
        return null;
    }

    public override string ToString()
    {
        return _char switch
        {
            '.' => " ",
            'F' => "\u2554",
            '-' => "\u2550",
            '7' => "\u2557",
            '|' => "\u2551",
            'J' => "\u255d",
            'L' => "\u255a",
            'S' => "\u256c",
            _ => throw new Exception("Unknown tile")
        };
    }
}

public class Ground : Tile
{
    public Ground(Map map, int x, int y, char c) : base(map, x, y, c)
    {

    }
}

public class Pipe : Tile
{
    public Pipe(Map map, int x, int y, char c, Directions[] connexions) : base(map, x, y, c)
    {
        Connexions = connexions;
    }
}

public enum Directions
{
    N, E, S, W
}
