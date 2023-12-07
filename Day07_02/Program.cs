using System.Text;

namespace Day07_02;

public class Program
{
    public static void Main()
    {
        Game game = new("input.txt");
        Console.WriteLine(game.ComputeResult());
    }

    public class Game
    {
        public List<Hand> Hands { get; set; } = new();

        public Game(string fileName)
        {
            string[] lines = File.ReadAllLines("input.txt");

            Array.ForEach(lines, line =>
            {
                string[] lineSplit = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var handString = lineSplit[0];
                var bid = int.Parse(lineSplit[1]);
                Hands.Add(new Hand(handString, bid));
            });
        }

        public Hand[] GetOrderedHands() => Hands
            .OrderBy(hand => hand.Value)
            .ThenBy(hand => hand.Cards[0])
            .ThenBy(hand => hand.Cards[1])
            .ThenBy(hand => hand.Cards[2])
            .ThenBy(hand => hand.Cards[3])
            .ThenBy(hand => hand.Cards[4])
            .ToArray();

        public int ComputeResult()
        {
            int result = 0; 

            Hand[] orderedHands = GetOrderedHands();

            for (int i = 1; i <= orderedHands.Length; i++)
            {
                result += i * orderedHands[i - 1].Bid;
            }

            return result;
        }

    }

    public class Card : IComparable<Card>, IEquatable<Card>
    {
        public char Face { get; init; }
        public int Value { get; init; }

        public Card(char face)
        {
            Face = face;
            Value = Values[face];
        }

        private static readonly Dictionary<char, int> Values = new()
        {
            {'J', 1},
            {'2', 2},
            {'3', 3},
            {'4', 4},
            {'5', 5},
            {'6', 6},
            {'7', 7},
            {'8', 8},
            {'9', 9},
            {'T', 10},
            {'Q', 12},
            {'K', 13},
            {'A', 14}
        };

        public int CompareTo(Card other)
        {
            return Value.CompareTo(other.Value);
        }

        public override string ToString() => Face.ToString();

        public bool Equals(Card other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Face == other.Face;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            return Face.GetHashCode();
        }

        public static bool operator ==(Card left, Card right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !Equals(left, right);
        }
    }

    public class Hand
    {
        public Card[] Cards { get; init; }

        public HandType HandType { get; init; }

        public int Value { get; init; }

        public int Bid { get; set; }

        public Hand(string hand, int bid) : this(hand)
        {
            Bid = bid;
        }

        public Hand(string hand)
        {
            Cards = hand.ToCharArray().Select(x => new Card(x)).ToArray();

            HandType = ComputeHandType();

            Value = (int) HandType;
        }

        private HandType ComputeHandType()
        {
            string handString = string.Join("", Cards.Select(x => x.Face));

            if (handString == "JJJJJ") return HandType.FiveOfAKind;

            Dictionary<char, List<char>> groups = handString.ToLookup(x => x).ToDictionary(x => x.Key, x => x.ToList());

            if (groups.ContainsKey('J'))
            {
                char mostRepeatedFaceExceptJoker = groups.Where(x => x.Key != 'J').MaxBy(x => x.Value.Count).Key;

                handString = handString.Replace('J', mostRepeatedFaceExceptJoker);

                groups = handString.ToLookup(x => x).ToDictionary(x => x.Key, x => x.ToList());
            }

            HandType result;

            if (groups.Count == 5) result = HandType.HighCard;
            else if (groups.Count == 1) result = HandType.FiveOfAKind;
            else if (groups.Any(x => x.Value.Count == 4)) result = HandType.FourOfAKind;
            else if (groups.Any(x => x.Value.Count == 3) && groups.Any(x => x.Value.Count == 2)) result = HandType.FullHouse;
            else if (groups.Any(x => x.Value.Count == 3)) result = HandType.ThreeOfAKind;
            else if (groups.Count(x => x.Value.Count == 2) == 2) result = HandType.TwoPairs;
            else result = HandType.OnePair;

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            Array.ForEach(Cards, x => sb.Append(x));
            sb.Append($" {HandType} ({Value})");
            return sb.ToString();
        }
    }

    public enum HandType
    {
        HighCard = 1,
        OnePair = 2,
        TwoPairs = 3,
        ThreeOfAKind = 4,
        FullHouse = 5,
        FourOfAKind = 6,
        FiveOfAKind = 7
    }
}
