namespace Day09_01;

public class Program
{
    public static void Main()
    {
        List<long[]> sequences = File.ReadAllLines("input.txt")
            .Select(line => line.Split(" ").Select(long.Parse).ToArray())
            .ToList();

        long result = 0;

        foreach (long[] sequence in sequences)
        {
            result += FindNextValue(sequence);
        }

        Console.WriteLine(result);
    }

    private static long FindNextValue(long[] values)
    {
        long[] currentDifferences = values;

        //Console.WriteLine(string.Join(" ", currentDifferences));

        List<long[]> history = new();
        history.Add(currentDifferences);

        while (true)
        {
            currentDifferences = ComputeDifferences(currentDifferences);
            history.Add(currentDifferences);

            //Console.WriteLine(string.Join(" ", currentDifferences));

            if (currentDifferences.All(x => x == 0))
            {
                break;
            }
        }

        long currentLast = 0;

        for (int i = history.Count - 2; i > 0; i--)
        {
            currentLast += history[i].Last();
        }

        long result = currentLast + values.Last();

        //Console.WriteLine(result);

        return result;
    }

    private static long[] ComputeDifferences(long[] values)
    {
        long[] result = new long[values.Length - 1];

        for (int index = 0; index < values.Length - 1; index++)
        {
            result[index] = values[index + 1] - values[index];
        }

        return result;
    }
}
