namespace Day10_02;

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