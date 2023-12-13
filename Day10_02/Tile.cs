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
    public bool IsInLoop { get; set; }

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

    public bool IsVertical => HasDirection(Directions.N) || HasDirection(Directions.S);
    public bool IsHorizontal => HasDirection(Directions.W) || HasDirection(Directions.E);

    public bool IsInner
    {
        get
        {
            if (IsInLoop) return false;

            Tile currentTile = this;
            string stateHistory = "0";

            //   01234 
            // 0 ╗═╔╗═
            // 1  ╔╝║╗
            // 2 ╬╝╚╚╗
            // 3 ║╔══╝
            // 4 ╚╝ ╚╝

            // traverse towards east (or whatever direction)
            // count how many times we cross the loop contour
            // if total is odd, we're inside
            // if total is even or zero, we're outside

            // debug
            if (this.X == 0 && this.Y == 0)
            {
                var foo = "bar";
            }

            while (true)
            {
                // We traverse horizontally
                currentTile = currentTile.East;

                // we are off the map
                if (currentTile == null)
                {
                    break;
                }

                if (currentTile.IsInLoop && currentTile.IsVertical)
                {
                    stateHistory += "1";
                }
                else
                {
                    stateHistory += "0";
                }
            }

            // Count the 1's. If its 0 or even, we're out
            int howManyWallsWereCrossed = stateHistory.ToCharArray().Count(c => c == '1');

            bool isOut = howManyWallsWereCrossed == 0 || howManyWallsWereCrossed % 2 == 0;

            return !isOut;
        }
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