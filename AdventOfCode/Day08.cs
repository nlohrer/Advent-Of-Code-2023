
namespace AdventOfCode
{
    public class Day08 : BaseDay
    {
        private readonly string _input;

        public Day08()
        {
            _input = File.ReadAllText(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var graph = BuildGraph(_input);
            long count = TraverseGraph(graph, _input);
            return new ValueTask<string>(count.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var graph = BuildGraph(_input);
            long count = TraverseGraphPart2(graph, _input);
            return new ValueTask<string>(count.ToString());
        }

        public static long TraverseGraphPart2(Dictionary<string, (string left, string right)> graph, string input)
        {
            long count = 0;
            string[] split = input.Split('\n');

            string directions = split[0];
            char direction;

            IEnumerable<string> current = graph.Keys.Where(key => key[2] == 'A').ToArray();

            var cycles = new long[current.Count()];
            var counts = new Dictionary<string, long>();

            for (int i = 0; current.Count() > 0; i = (i + 1) % directions.Length)
            {
                direction = directions[i];
                if (direction == 'L')
                    current = current.Select(node => graph[node].left).ToArray();
                else
                    current = current.Select(node => graph[node].right).ToArray();
                count++;
                foreach (string node in current)
                    if (!counts.Keys.Contains(node) && node[2] == 'Z')
                    {
                        counts.Add(node, count);
                        current = current.Where(nod => nod != node);
                    }
            }
            return counts.Values.Aggregate(lcm);
        }

        public static long gcd(long a, long b)
        {
            return b == 0 ? a : gcd(b, a % b);
        }

        public static long lcm(long a, long b)
        {
            return (a / gcd(a, b)) * b;
        }

        public static long TraverseGraph(Dictionary<string, (string left, string right)> graph, string input)
        {
            long count = 0;
            string[] split = input.Split('\n');

            string directions = split[0];
            char direction;

            string current = "AAA";

            for (int i = 0; current != "ZZZ"; i = (i + 1) % directions.Length)
            {
                direction = directions[i];
                if (direction == 'L')
                    current = graph[current].left;
                else
                    current = graph[current].right;
                count++;
            }
            return count;
        }

        public static Dictionary<string, (string left, string right)> BuildGraph(string input)
        {
            return input.Split("\n").Where(line => line.Any()).Skip(1)
                .Select(line =>
                {
                    var split = line.Split(' ');
                    string key = split[0];
                    string left = split[2].Substring(1, 3);
                    string right = split[3].Substring(0, 3);
                    return (key, (left, right));
                }).ToDictionary();
        }
    }

    public class Day08Tests
    {
        string _testInput = """
            LLR

            AAA = (BBB, BBB)
            BBB = (AAA, ZZZ)
            ZZZ = (ZZZ, ZZZ)
            """.ReplaceLineEndings("\n");

        string _testInput2 = """
            RL

            AAA = (BBB, CCC)
            BBB = (DDD, EEE)
            CCC = (ZZZ, GGG)
            DDD = (DDD, DDD)
            EEE = (EEE, EEE)
            GGG = (GGG, GGG)
            ZZZ = (ZZZ, ZZZ)
            """.ReplaceLineEndings("\n");

        [Fact]
        public void Solve1()
        {
            var graph = Day08.BuildGraph(_testInput);
            long count = Day08.TraverseGraph(graph, _testInput);
            Assert.Equal(6, count);
        }

        [Fact]
        public void Solve1SecondInput()
        {
            var graph = Day08.BuildGraph(_testInput2);
            long count = Day08.TraverseGraph(graph, _testInput2);
            Assert.Equal(2, count);
        }

        string _testInputPart2 = """
            LR

            11A = (11B, XXX)
            11B = (XXX, 11Z)
            11Z = (11B, XXX)
            22A = (22B, XXX)
            22B = (22C, 22C)
            22C = (22Z, 22Z)
            22Z = (22B, 22B)
            XXX = (XXX, XXX)
            """.ReplaceLineEndings("\n");

        [Fact]
        public void SolvePart2()
        {
            var graph = Day08.BuildGraph(_testInputPart2);
            long count = Day08.TraverseGraphPart2(graph, _testInputPart2);
            Assert.Equal(6, count);
        }
    }
}
