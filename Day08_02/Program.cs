namespace Day08_02;

public class Program
{
    public static void Main()
    {
        Game game = new("input.txt");

        var results = game.Solve();

        var gameResult = Lcm(results);

        Console.WriteLine(gameResult);
    }

    public static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static long Lcm(long a, long b)
    {
        return (a / Gcd(a, b)) * b;
    }

    public static long Lcm(long[] numbers)
    {
        long buffer = numbers[0];
        for (int i = 1; i < numbers.Length; i++)
        {
            buffer = Lcm(buffer, numbers[i]);
        }

        return buffer;
    }

    public class Game
    {
        public char[] Directions { get; set; }
        public List<Node> Nodes { get; set; } = new();

        private Node _currentNode;

        public Game(string fileName)
        {
            ParseGameFile(fileName);
        }

        public long[] Solve()
        {
            var startNodes = Nodes.Where(x => x.IsStartNode).ToArray();

            long[] results = new long[startNodes.Length];

            for (var n = 0; n < startNodes.Length; n++)
            {
                long moves = 0;

                _currentNode = startNodes[n];

                for (long i = 0; true; i++)
                {
                    var currentDirection = Directions[i % Directions.Length];
                    MoveNext(currentDirection);
                    moves++;

                    if (_currentNode.IsEndNode)
                    {
                        break;
                    }
                }

                results[n] = moves;
            }

            return results;
        }

        private void MoveNext(char direction)
        {
            _currentNode = direction switch
            {
                'L' => _currentNode.Left,
                'R' => _currentNode.Right,
                _ => throw new Exception("Unknown direction")
            };
 }

        private void ParseGameFile(string input)
        {
            string[] lines = File.ReadAllLines(input);

            Directions = lines[0].ToCharArray();

            var nodesStrings = lines.Skip(2).ToList();

            Dictionary<string, string[]> nodeStrings = new();

            foreach (var nodeString in nodesStrings)
            {
                var segments = nodeString.Split('=',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var nodeId = segments[0];
                var leftRight = segments[1].Replace("(", "").Replace(")", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var left = leftRight[0];
                var right = leftRight[1];

                nodeStrings.Add(nodeId, new []{left, right});
                Nodes.Add(new Node(nodeId));
            }

            foreach ((string key, string[] value) in nodeStrings)
            {
                Node node = Nodes.First(x => x.Id == key);
                node.Left = Nodes.First(x => x.Id == value[0]);
                node.Right = Nodes.First(x => x.Id == value[1]);
            }
        }

        public override string ToString()
        {
            return string.Join(", ", Directions);
        }
    }

    public class Node
    {
        public string Id { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public bool IsDeadEndStreet => Id == Left.Id && Id == Right.Id;

        public bool IsStartNode => Id.EndsWith("A");
        public bool IsEndNode => Id.EndsWith("Z");

        public Node(string id)
        {
            Id = id;
        }

        public override string ToString() => $"{Id} ({Left.Id},{Right.Id}) {(IsStartNode ? "\ud83c\udfc1" : "")} {(IsEndNode ? "\ud83c\udfc6" : "")} {(IsDeadEndStreet ? "\u26d4" : "")}";
    }
}
