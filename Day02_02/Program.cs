using System;

namespace Day02_02;

public class Program
{
	public static void Main()
	{
        

        var lines = File.ReadAllLines("input.txt");

        int total = 0;

        foreach (var line in lines)
        {
            var gameId = int.Parse(line.Split(':').First().Split(' ').Last());

            var sets = line.Split(':').Last().Split(';');

            Dictionary<string, int> greatestQuantityByColor = new()
            {
                {"green", 0},
                {"blue", 0},
                {"red", 0}
            };

            foreach(var set in sets)
            {
                var setArrangement = set.Split(',');

                foreach(var arrangement in setArrangement)
                {
                    var arrangementDetails = arrangement.Trim().Split(' ');

                    var quantity = int.Parse(arrangementDetails[0]);
                    var color = arrangementDetails[1];

                    if (quantity > greatestQuantityByColor[color])
                    {
                        greatestQuantityByColor[color] = quantity;
                    }
                }
            }

            var gamePower = greatestQuantityByColor["green"] * greatestQuantityByColor["red"] * greatestQuantityByColor["blue"];

            total += gamePower;
        }

        Console.WriteLine(total);
	}
}