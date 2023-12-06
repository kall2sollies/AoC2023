namespace Day06_02;

public class Program
{
    public static void Main()
    {
        var race = ParceRaceData("input.txt");

        //Console.WriteLine($"time={race.RaceTime}, distance={race.Distance}, solutions={string.Join(';', race.Solutions)}");

        long solution = race.Solutions.LongCount();

        Console.WriteLine(solution);
    }

    private static Race ParceRaceData(string fileName)
    {
        List<Race> races = new();

        var lines = File.ReadAllLines(fileName);

        var timeLine = lines[0];
        var distanceLine = lines[1];

        var time = long.Parse(timeLine.Split(':')[1].Replace(" ", string.Empty));
        var distance = long.Parse(distanceLine.Split(':')[1].Replace(" ", string.Empty));

        return new Race(time, distance);
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

    public List<long> Solutions { get; set; }

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

    public List<long> ComputeBetterTimes()
    {
        var roots = ComputePolynomRoots();

        if (roots.Count < 2) throw new Exception($"No better distance than {Distance} for total time of {RaceTime}");

        return GetIntegersBetween(roots[0], roots[1]);
    }

    public static List<long> GetIntegersBetween(double left, double right)
    {
        var isLeftInteger = Math.Abs(left % 1) <= (double.Epsilon * 100);
        var isRightInteger = Math.Abs(right % 1) <= (double.Epsilon * 100);

        var leftInt = isLeftInteger ? (long) Math.Ceiling(left) + 1 :  (long) Math.Ceiling(left);
        var rightInt = isRightInteger ? (long) Math.Floor(right) - 1 : (long) Math.Floor(right);

        var integers = new List<long>();

        for (var i = leftInt; i <= rightInt; i++)
        {
            integers.Add(i);
        }

        return integers;
    }
}