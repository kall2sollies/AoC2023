namespace Day06_01;

public class Program
{
    public static void Main()
    {
        var races = ParceRaceData("input.txt");

        long solution = 1;

        foreach (var race in races)
        {
            Console.WriteLine($"time={race.RaceTime}, distance={race.Distance}, solutions={string.Join(';', race.Solutions)}");

            solution *= race.Solutions.Count;
        }

        Console.WriteLine(solution);
    }

    private static List<Race> ParceRaceData(string fileName)
    {
        List<Race> races = new();

        var lines = File.ReadAllLines(fileName);

        var timeLine = lines[0];
        var distanceLine = lines[1];

        var times = timeLine.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        var distances = distanceLine.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

        for (var i = 0; i < times.Length; i++)
        {
            Race race = new(times[i], distances[i]);
            races.Add(race);
        }

        return races;
    }
}

public class Race
{
    public Race(double raceTime, double distance)
    {
        RaceTime = raceTime;
        Distance = distance;

        Solutions = ComputeBetterTimes();
    }

    public double RaceTime { get; init; }
    public double Distance { get; init; }

    public List<int> Solutions { get; set; }

    public List<double> ComputePolynomRoots()
    {
        var roots = new List<double>();

        var squareRootTerm = RaceTime * RaceTime - 4 * Distance;

        if (squareRootTerm < 0) return roots;

        var root1 = (RaceTime + Math.Sqrt(squareRootTerm)) / 2d;
        var root2 = (RaceTime - Math.Sqrt(squareRootTerm)) / 2d;

        roots.Add(Math.Min(root1, root2));
        roots.Add(Math.Max(root1, root2));

        return roots;
    }

    public List<int> ComputeBetterTimes()
    {
        var roots = ComputePolynomRoots();

        if (roots.Count < 2) throw new Exception($"No better distance than {Distance} for total time of {RaceTime}");

        return GetIntegersBetween(roots[0], roots[1]);
    }

    public static List<int> GetIntegersBetween(double left, double right)
    {
        var isLeftInteger = Math.Abs(left % 1) <= (double.Epsilon * 100);
        var isRightInteger = Math.Abs(right % 1) <= (double.Epsilon * 100);

        var leftInt = isLeftInteger ? (int) Math.Ceiling(left) + 1 :  (int) Math.Ceiling(left);
        var rightInt = isRightInteger ? (int) Math.Floor(right) - 1 : (int) Math.Floor(right);

        var integers = new List<int>();

        for (var i = leftInt; i <= rightInt; i++)
        {
            integers.Add(i);
        }

        return integers;
    }
}