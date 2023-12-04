
namespace AdventOfCode
{
    public class Day04 : BaseDay
    {
        private readonly string[] _input;
        public Day04()
        {
            _input = File.ReadAllLines(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            int sum = _input
                .Select(CalculatePoints)
                .Sum();
            return new ValueTask<string>(sum.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            var edges = BuildGraph(_input);
            var found = new Dictionary<int, int>();

            int total = edges.Count;
            foreach (int card in edges.Keys)
                total += AmountGenerated(card, edges, found);

            return new ValueTask<string>(total.ToString());
        }

        public static int AmountGenerated(int card, Dictionary<int, List<int>> edges, Dictionary<int, int> found)
        {
            if (found.Keys.Contains(card))
                return found[card];
            int value;
            List<int> copies = edges[card];
            if (!copies.Any())
                value = 0;
            else
                value = copies.Count + copies.Select((int copy) => AmountGenerated(copy, edges, found)).Sum();
            found.Add(card, value);
            return value;
        }

        public static Dictionary<int, List<int>> BuildGraph(IEnumerable<string> input)
        {
            return input.Select((line, i) =>
            {
                int count = GetMatching(line);
                List<int> copies = Enumerable.Range(i + 2, count).ToList();
                return (i + 1, copies);
            }).ToDictionary();
        }

        public static int CalculatePoints(string line)
        {
            int count = GetMatching(line);
            if (count == 0)
                return 0;
            return Enumerable.Repeat(1, count - 1).Aggregate(1, (x, y) => x * 2);
        }

        public static int GetMatching(string line)
        {
            var lists = GetListsFromLine(line);
            List<int> winning = lists[0], having = lists[1];
            int count = (from winningNumber in winning
                         from havingNumber in having
                         where winningNumber == havingNumber
                         select winningNumber).Count();
            return count;
        }

        public static List<List<int>> GetListsFromLine(string line)
        {
            string onlyNums = line.Substring(line.IndexOf(':') + 1);
            return onlyNums
                .Split(" | ")
                .Select(nums => nums
                    .Trim()
                    .Split(' ')
                    .Where(number => !string.IsNullOrEmpty(number))
                    .Select(int.Parse)
                .ToList())
                .ToList();
        }
    }

    public class Day04Tests
    {
        string _testInput = """
            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
            """;

        [Fact]
        public void PartOneSum()
        {
            int sum = _testInput
                .Split(Environment.NewLine)
                .Select(Day04.CalculatePoints)
                .Sum();
            Assert.Equal(13, sum);
        }

        [Fact]
        public void CalculateTotal()
        {
            var edges = Day04.BuildGraph(_testInput.Split(Environment.NewLine));
            var found = new Dictionary<int, int>();

            int total = edges.Count;
            foreach (int card in edges.Keys)
            {
                total += Day04.AmountGenerated(card, edges, found);
            }

            Assert.Equal(30, total);
        }
    }
}
