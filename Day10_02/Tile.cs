namespace Day10_02;

public abstract class Tile
{
    private readonly Map _map;
    private readonly char _char;

    public int X { get; set; }
    public int Y { get; set; }

    public Directions[] Connexions { get; set; } = Array.Empty<Directions>();
    public bool HasConnexions => Connexions.Length > 0;
    public virtual bool IsStartTile { get; init; }

    public string Id { get; set; }

    protected Tile(Map map, int x, int y, char c)
    {
        _map = map;
        _char = c;
        X = x;
        Y = y;
        Id = $"{X}.{Y}";
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

        return tileAtDirection != null && tileAtDirection.HasDirection(GetOppositeDirection(direction));
    }

    public bool IsNeighbourOf(Tile other)
    {
        return Neighbours.Select(x => x.Id).Contains(other.Id);
    }

    public bool HasConnectionTo(Tile other)
    {
        if (!IsNeighbourOf(other)) return false;

        Directions directionOfOther;
        if (North?.Id == other.Id) directionOfOther = Directions.N;
        else if (East?.Id == other.Id) directionOfOther = Directions.E;
        else if (South?.Id == other.Id) directionOfOther = Directions.S;
        else if (West?.Id == other.Id) directionOfOther = Directions.W;
        else throw new Exception("Can't determine direction of other");

        return HasConnectionTo(directionOfOther) && other.HasConnectionTo(GetOppositeDirection(directionOfOther));
    }

    private static Directions GetOppositeDirection(Directions direction)
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

    public Tile[] ConnectedNeighbours => Neighbours.Where(x => x != null && x.HasConnectionTo(this)).ToArray();

    public string Drawing => _char switch
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

    public override string ToString()
    {
        return $"[{X},{Y}] {Drawing}";
    }
}