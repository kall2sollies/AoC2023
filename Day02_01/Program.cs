using System;

namespace Day01_01;

public class Program
{
	public static void Main()
	{
        Dictionary<string, int> _quantities = new()
        {
            {"green", 13},
            {"blue", 14},
            {"red", 12}
        };

        var lines = File.ReadAllLines("input.txt");

        int total = 0;

        foreach (var line in lines)
        {
            var gameId = int.Parse(line.Split(':').First().Split(' ').Last());

            var sets = line.Split(':').Last().Split(';');

            var isGameValid = true;

            foreach(var set in sets)
            {
                var setArrangement = set.Split(',');

                foreach(var arrangement in setArrangement)
                {
                    var arrangementDetails = arrangement.Trim().Split(' ');

                    var quantity = int.Parse(arrangementDetails[0]);
                    var color = arrangementDetails[1];

                    var maxForThisColor = _quantities[color];
                    if (quantity > maxForThisColor)
                    {
                        isGameValid = false;
                        break;
                    }
                }

                if (!isGameValid)
                {
                    break;
                }
            }

            if (isGameValid)       
            {
                total += gameId;
            }
        }

        Console.WriteLine(total);
	}
}