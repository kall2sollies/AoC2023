using System;

namespace Day01_01;

public class Program
{
	public static void Main()
	{
        int total = 0;
		var lines = File.ReadAllLines("input.txt");

        foreach (var line in lines)
        {
            List<int> digits = new();

            for (var i = 0 ; i < line.Length ; i++)
            {
                char c = line[i];
                if (c >= '0' && c <= '9')
                {
                    var digit = int.Parse(c.ToString());
                    digits.Add(digit);
                }
            }

            var first = digits.First();
            var last = digits.Last();

            var number = int.Parse(first.ToString() + last.ToString());

            total += number;
        }

        Console.WriteLine(total);
	}
}