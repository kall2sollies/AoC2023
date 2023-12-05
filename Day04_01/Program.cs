using System.Text;

namespace Day04_01;

public class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines("input.txt");

        int total = 0;

        foreach (var line in lines)
        {
            string[] cardParts = line.Split(':')[1].Split('|');

            var winningNumbers = cardParts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList();
            var cardNumbers = cardParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x)).Select(x => int.Parse(x)).ToList();

            var matches = cardNumbers.Intersect(winningNumbers).ToList();

            total += (int)Math.Pow(2, matches.Count - 1);
        }

        Console.WriteLine(total);
    }
}