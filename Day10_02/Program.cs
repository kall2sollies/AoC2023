namespace Day10_02;

public class Program
{
    public static void Main()
    {
        Map map = new(File.ReadAllLines("input.txt"));
        Console.WriteLine(map);

        var innerTiles = map.TileList.Where(x => x.IsInner).ToList();
        Console.WriteLine(innerTiles.Count);
    }
}