using System;

namespace Day01;

public class Program
{
	public static void Main()
	{
        Dictionary<string, int> words = new()
        {
            {"one", 1},
            {"two", 2},
            {"three", 3},
            {"four", 4},
            {"five", 5},
            {"six", 6},
            {"seven", 7},
            {"eight", 8},
            {"nine", 9},
            {"1", 1},
            {"2", 2},
            {"3", 3},
            {"4", 4},
            {"5", 5},
            {"6", 6},
            {"7", 7},
            {"8", 8},
            {"9", 9}
        };

        int total = 0;
		var lines = File.ReadAllLines("input.txt");

        foreach (var line in lines)
        {
            int? first = null;
            int? last = null;

            for (var i = 0 ; i < line.Length ; i++)
            {
                foreach(var key in words.Keys)
                {
                    var currentSubstring = line.Substring(i);
                    if (currentSubstring.IndexOf(key) == 0)
                    {                     
                        var currentValue = words[key];

                        if (first == null)
                        {
                            first = currentValue;
                        }
                        else
                        {
                            last = currentValue;
                        }
                    }
                }                
            }

            var lineTotalWord = first.Value.ToString();
            if (last.HasValue) lineTotalWord += last.Value.ToString();
            else lineTotalWord += first.Value.ToString();
            var lineTotal = int.Parse(lineTotalWord);

            total += lineTotal;
        }

        Console.WriteLine(total);
	}
}