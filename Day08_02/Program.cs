namespace Day08_02;

public class Program
{
    public static void Main()
    {
        Game game = new("input.txt");
        Console.WriteLine(game.Solve());
    }

    public class Game
    {
        public char[] Directions { get; set; }
        public List<Node> Nodes { get; set; } = new();

        private readonly Node[] CurrentNodes;

        public Game(string fileName)
        {
            ParseGameFile(fileName);

            CurrentNodes = Nodes.Where(x => x.IsStartNode).ToArray();
        }

        public long Solve()
        {
            long moves = 0;

            for (long i = 0; true; i++)
            {
                var currentDirection = Directions[i % Directions.Length];
                MoveNext(currentDirection);
                moves++;

                if (CurrentNodes.All(x => x.IsEndNode))
                {
                    break;
                }
            }

            return moves;
        }

        private void MoveNext(char direction)
        {
            for (var index = 0; index < CurrentNodes.Length; index++)
            {
                CurrentNodes[index] = direction switch
                {
                    'L' => CurrentNodes[index].Left,
                    'R' => CurrentNodes[index].Right,
                    _ => throw new Exception("Unknown direction")
                };
            }
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
