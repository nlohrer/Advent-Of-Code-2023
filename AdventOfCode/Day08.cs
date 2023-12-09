
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
            int count = TraverseGraph(graph, _input);
            return new ValueTask<string>(count.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            throw new NotImplementedException();
        }

        public static int TraverseGraph(Dictionary<string, (string left, string right)> graph, string input)
        {
            int count = 0;
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


    public record Tree(string value, Tree left, Tree right);


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
            int count = Day08.TraverseGraph(graph, _testInput);
            Assert.Equal(6, count);
        }

        [Fact]
        public void Solve1SecondInput()
        {
            var graph = Day08.BuildGraph(_testInput2);
            int count = Day08.TraverseGraph(graph, _testInput2);
            Assert.Equal(2, count);
        }
    }
}
