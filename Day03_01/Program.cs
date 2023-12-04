using System.Text;

namespace Day03_01;

public class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines("input.txt");

        Map map = new(lines);

        Console.Write(map);
        Console.WriteLine();

        int result = map.Numbers.Where(x => x.HasSymbolAround).Sum(x => x.Value);
        Console.WriteLine($"Result: {result}");
    }
}

public class Map
{
    public Cell[,] Cells { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    public List<Number> Numbers { get; set; } = new();

    private readonly string[] _lines;

    public Map(string[] lines)
    {
        _lines = lines;
        BuildMap();
        ScanNumbers();
    }

    private void BuildMap()
    {
        Height = _lines.Length;
        Width = _lines[0].Length;

        Cells = new Cell[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            string line = _lines[y];
            for (int x = 0; x < Width; x++)
            {
                char c = line[x];

                Cell cell = new(x, y, c, this);

                Cells[x, y] = cell;
            }
        }
    }

    private void ScanNumbers()
    {
        Cell currentCell = Cells[0, 0];

        string buffer = string.Empty;
        Cell numberStart = null;

        while (true)
        {
            if (currentCell.IsDigit)
            {
                numberStart ??= currentCell.Clone();

                buffer += currentCell.ToString();

                if (currentCell.IsLastColumn)
                {
                    AppendNumber(numberStart, buffer);
                    numberStart = null;
                    buffer = string.Empty;
                }
            }
            else
            {
                if (numberStart != null)
                {
                    AppendNumber(numberStart, buffer);
                }

                numberStart = null;
                buffer = string.Empty;
            }

            if (currentCell.IsLastColumn && currentCell.IsLastRow) break;

            currentCell = currentCell.IsLastColumn ? currentCell.FirstOfNextLine : currentCell.East;
        }
    }

    private void AppendNumber(Cell start, string buffer)
    {
        Number number = new()
        {
            StartCell = start,
            Length = buffer.Length,
            Value = int.Parse(buffer)
        };

        Numbers.Add(number);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        Cell currentCell = Cells[0, 0];

        while (true)
        {
            sb.Append(currentCell);

            if (currentCell.IsLastColumn && currentCell.IsLastRow) break;

            if (currentCell.IsLastColumn)
            {
                sb.Append(Environment.NewLine);
                currentCell = currentCell.FirstOfNextLine;
            }
            else
            {
                currentCell = currentCell.East;
            }
        }

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendJoin(',', Numbers.Select(x => x));

        return sb.ToString();
    }
}

public class Number
{
    public Cell StartCell { get; set; }
    public int Length { get; set; }
    public int Value { get; set; }

    protected List<Cell> NumberCells
    {
        get
        {
            List<Cell> numberCells = new() {StartCell};
            for (int i = 1; i < Length; i++)
            {
                numberCells.Add(StartCell.GetRelativeCell(i, 0));
            }

            return numberCells;
        }
    }

    public bool HasSymbolAround
    {
        get
        {
            return NumberCells.Any(cell => cell.HasSymbolInNeighbourhood);
        }
    }

    public override string ToString()
    {
        string output = Value.ToString();

        if (HasSymbolAround)
        {
            output += " (*)";
        }

        return output;
    }
}

public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public char C { get; set; }

    private readonly Map _map;

    public bool IsDigit => C is >= '0' and <= '9';

    public bool IsSymbol => !IsDigit && C != '.';

    public bool IsLastColumn => X == _map.Width - 1;
    public bool IsLastRow => Y == _map.Height - 1;

    public Cell(int x, int y, char c, Map map)
    {
        X = x;
        Y = y;
        C = c;
        _map = map;
    }

    public Cell GetRelativeCell(int xOffset, int yOffset)
    {
        if (X + xOffset >= _map.Width) return null;
        if (X + xOffset < 0) return null;
        if (Y + yOffset >= _map.Height) return null;
        if (Y + yOffset < 0) return null;

        return _map.Cells[X + xOffset, Y + yOffset];
    }

    public Cell North => GetRelativeCell(0, -1);
    public Cell NorthEast => GetRelativeCell(1, -1);
    public Cell East => GetRelativeCell(1, 0);
    public Cell SouthEast => GetRelativeCell(1, 1);
    public Cell South => GetRelativeCell(0, 1);
    public Cell SouthWest => GetRelativeCell(-1, 1);
    public Cell West => GetRelativeCell(-1, 0);
    public Cell NorthWest => GetRelativeCell(-1, -1);
    public Cell FirstOfNextLine => GetRelativeCell(-X, 1);

    public List<Cell> Neighbours
    {
        get
        {
            List<Cell> cells = new() { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };
            return cells.Where(x => x != null).ToList();
        }
    }

    public bool HasSymbolInNeighbourhood
    {
        get
        {
            return Neighbours.Any(x => x.IsSymbol);
        }
    }

    public Cell Clone() => new (X, Y, C, _map);

    public override string ToString() => C.ToString();
}