
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
            throw new NotImplementedException();
        }

        public static int CalculatePoints(string line)
        {
            var lists = GetListsFromLine(line);
            List<int> winning = lists[0], having = lists[1];
            int count = (from winningNumber in winning
                        from havingNumber in having
                        where winningNumber == havingNumber
                        select winningNumber).Count();
            if (count == 0)
                return 0;
            return Enumerable.Repeat(1, count - 1).Aggregate(1, (x, y) => x * 2);
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
    }
}
