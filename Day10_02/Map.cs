using System.Text;

namespace Day10_02;

public class Map
{
    public Tile[,] Tiles { get; private set; }
    public List<Tile> TileList { get; } = new();
    public List<Tile> LoopTiles { get; } = new();
    public int Height { get; private set; }
    public int Width { get; private set; }

    public Tile StartTile => TileList.First(x => x.IsStartTile);

    private readonly string[] _lines;

    public Map(string[] lines)
    {
        _lines = lines;
        BuildMap();
        ComputeLoop();
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

    private void ComputeLoop()
    {
        Tile currentTile = StartTile;
        currentTile.IsInLoop = true;

        LoopTiles.Add(currentTile);

        while (true)
        {
            Tile[] connectedNeighbours = currentTile.ConnectedNeighbours;
            Tile nextTile = connectedNeighbours.FirstOrDefault(x => !LoopTiles.Select(x => x.Id).Contains(x.Id));

            if (nextTile == null)
            {
                break;
            }

            nextTile.IsInLoop = true;
            LoopTiles.Add(nextTile);
            currentTile = nextTile;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(Tiles[x, y].Drawing);
            }

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }
}