namespace Day05_02;

public class Program
{
    public static void Main()
    {
        Game game = new("input.txt");

        long closestLocation = long.MaxValue;

        foreach (var seedRange in game.SeedRanges)
        {
            for (var seed = seedRange.Start; seed <= seedRange.End; seed++)
            {
                var location = game.GetLocationForSeed(seed);

                if (location < closestLocation)
                {
                    closestLocation = location;
                }
            }
        }

        Console.WriteLine(closestLocation);
    }
}

public static class GroupNames
{
    public static string SeedToSoil = "seed-to-soil";
    public static string SoilToFertilizer = "soil-to-fertilizer";
    public static string FertilizerToWater = "fertilizer-to-water";
    public static string WaterToLight = "water-to-light";
    public static string LightToTemperature = "light-to-temperature";
    public static string TemperatureToHumidity = "temperature-to-humidity";
    public static string HumidityToLocation = "humidity-to-location";

    public static List<string> AllGroups() => new() { SeedToSoil, SoilToFertilizer, FertilizerToWater, WaterToLight, LightToTemperature, TemperatureToHumidity, HumidityToLocation };
}

public class Game
{
    public List<MapGroup> Groups { get; set; } = new();

    public List<MapRange> SeedRanges { get; set; } = new();

    public MapGroup GetGroupByName(string name) => Groups.First(x => x.Name == name);

    public Game(string fileName)
    {
        ParseData(fileName);
    }

    public long GetLocationForSeed(long seed)
    {
        long current = seed;

        foreach (MapGroup group in Groups)
        {
            current = group.ToDestination(current);
        }

        return current;
    }

    private void ParseData(string fileName)
    {
        var lines = File.ReadAllLines(fileName);

        var seedsWithRanges = lines[0]
            .Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToList();

        for (int i = 0; i < seedsWithRanges.Count; i += 2)
        {
            var seed = seedsWithRanges[i];
            var length = seedsWithRanges[i + 1];

            SeedRanges.Add(new MapRange(seed, length));
        }

        MapGroup currentGroup = null;

        for (int i = 2; i < lines.Length; i++)
        {
            var line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
            {
                Groups.Add(currentGroup);

                continue;
            }

            if (GroupNames.AllGroups().Any(line.StartsWith))
            {
                string groupName = GroupNames.AllGroups().First(line.StartsWith);

                currentGroup = new MapGroup(groupName);

                continue;
            }

            var data = line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse)
                .ToArray();

            currentGroup.AddMap(data);
        }

        Groups.Add(currentGroup);
    }
}

public class MapGroup
{
    public string Name { get; set; }
    public List<Map> Maps { get; set; } = new();

    public MapGroup(string name)
    {
        Name = name;
    }

    public void AddMap(long[] mapData)
    {
        Maps.Add(new Map(mapData));
    }

    public long ToDestination(long source)
    {
        return Maps.FirstOrDefault(x => x.IsInRange(source))?.ToDestination(source) ?? source;
    }
}

public class Map
{
    public MapRange SourceRange { get; init; }
    public MapRange DestinationRange { get; init; }

    public long Offset { get; init; }

    public Map(long[] mapData) :
        this(mapData[1], mapData[0], mapData[2]) { }

    public Map(long sourceStart, long destinationStart, long length)
    {
        SourceRange = new(sourceStart, length);
        DestinationRange = new(destinationStart, length);
        Offset = destinationStart - sourceStart;
    }

    public bool IsInRange(long source) => SourceRange.IsInRange(source);

    public long ToDestination(long source)
    {
        if (!IsInRange(source)) return -1;

        return source + Offset;
    }
}

public class MapRange
{
    public long Start { get; init; }
    public long Length { get; init; }
    public long End => Start + Length - 1;

    public MapRange(long start, long length)
    {
        Start = start;
        Length = length;
    }

    public bool IsInRange(long input)
    {
        return Start <= input && input <= End;
    }
}