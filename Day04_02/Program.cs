using System.Linq;
using System.Text;

namespace Day04_02;

public class Program
{
    public static void Main()
    {
        Game game = new("input.txt");
        Console.WriteLine(game);
        Console.WriteLine(game.CountGains(game.Cards));
    }
}

public class Game
{
    public Dictionary<int, int> Map { get; set; }
    public List<Card> Cards { get; set; } = new();

    public Game(string fileName)
    {
        FillMap(fileName);
        CreateCards();
        ComputeGains();
    }

    public int CountGains(List<Card> cards)
    {
        int total = 0;

        foreach (Card card in cards)
        {
            if (card.Gains.Count > 0)
            {
                total += CountGains(card.Gains);
            }

            total++;
        }

        return total;
    }

    private void FillMap(string fileName)
    {
        Map = File.ReadAllLines("input.txt").Select(line =>
        {
            int cardId = int.Parse(line.Split(':')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Last(x => !string.IsNullOrEmpty(x)));
            string[] cardParts = line.Split(':')[1].Split('|');
            var winningNumbers = cardParts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            var cardNumbers = cardParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            var matches = cardNumbers.Intersect(winningNumbers).ToList();

            return new {key = cardId, wins = matches.Count};
        }).ToDictionary(x => x.key, x => x.wins);
    }

    private void CreateCards()
    {
        foreach ((int key, int value) in Map)
        {
            Cards.Add(
                new Card(key, value, this));
        }
    }

    private void ComputeGains()
    {
        foreach (Card card in Cards)
        {
            card.ComputeGains();
        }
    }

    public Card GetCardById(int id) => Cards.FirstOrDefault(x => x.Id == id);

    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (Card card in Cards)
        {
            sb.Append(card);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public class Card
{
    public int Id { get; init; }
    public int Wins { get; init; }

    public Game Game { get; init; }

    public List<Card> Gains { get; set; } = new();

    public Card(int id, int wins, Game game)
    {
        Id = id;
        Wins = wins;
        Game = game;
    }

    public void ComputeGains()
    {
        if (Wins == 0) return;
            
        for (int i = 1; i <= Wins; i++)
        {
            Card gainCard = Game.GetCardById(Id + i);

            if (gainCard != null)
            {
                Gains.Add(gainCard);
            }
        }
    }

    public override string ToString()
    {
        return $"{Id} => {Wins} (Gains={string.Join(',',Gains.Select(x => x.Id))})\r\n";
    }
}