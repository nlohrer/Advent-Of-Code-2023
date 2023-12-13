
namespace AdventOfCode
{
    public class Day09 : BaseDay
    {
        private readonly string _input;

        public Day09()
        {
            _input = File.ReadAllText(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            int sum = _input.Split('\n').Where(line => line != "").Select(line => SolveParts(line, true)).Sum();
            return new ValueTask<string>(sum.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            int sum = _input.Split('\n').Where(line => line != "").Select(line => SolveParts(line, false)).Sum();
            return new ValueTask<string>(sum.ToString());
        }

        public static int SolveParts(string line, bool useLast)
        {
            List<List<int>> sequences = new() { line.Split(' ').Where(str => str != "").Select(int.Parse).ToList() };
            List<int> next = sequences[0];
            while (!next.TrueForAll(num => num == 0))
            {
                next = GenerateNextSequence(next);
                sequences.Add(next);
            }
            sequences.Reverse();

            int last = 0;
            foreach(var sequence in sequences)
            {
                if (useLast)
                    last = sequence.Last() + last;
                else
                    last = sequence.First() - last;
            }

            return last;
        }

        public static List<int> GenerateNextSequence(List<int> sequence)
        {
            List<int> next = new();
            for (int i = 0; i < sequence.Count - 1; ++i)
            {
                next.Add(sequence[i + 1] - sequence[i]);
            }
            return next;
        }
    }

    public class Day09Tests
    {
        string _testInput = """
            0 3 6 9 12 15
            1 3 6 10 15 21
            10 13 16 21 30 45
            """.ReplaceLineEndings("\n");

        [Fact]
        public void Part1()
        {
            var result = _testInput.Split('\n').Select(line => Day09.SolveParts(line, true)).ToList();
            Assert.Equal([18, 28, 68], result);
        }

        [Fact]
        public void Part2()
        {
            var result = _testInput.Split('\n').Select(line => Day09.SolveParts(line, false)).ToList();
            Assert.Equal([-3, 0, 5], result);
        }
    }
}
