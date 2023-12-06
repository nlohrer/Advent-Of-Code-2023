
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public class Day06 : BaseDay
    {
        private readonly string _input;

        public Day06()
        {
            _input = File.ReadAllText(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var timeDistance = ParseInput(_input);
            List<int> margins = timeDistance.Select(pair => GetRootDistance(pair.time, pair.distance)).ToList();
            int result = margins.Aggregate((x, y) => x * y);
            return new ValueTask<string>(result.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            throw new NotImplementedException();
        }

        public static List<(int time, int distance)> ParseInput(string input)
        {
            List<List<int>> lines = input.Split('\n')
                .Select(line => line.Substring(Regex.Match(line, @"\d").Index))
                .Select(line => line.Split(' ').Where(str => str != "").Select(int.Parse).ToList())
                .ToList();
            var timeDistancePairs = lines[0].Zip(lines[1]).ToList();
            return timeDistancePairs;
        }

        public static int GetRootDistance(int time, int distance)
        {
            Double discriminant = Math.Sqrt(1.0 * Math.Pow(time, 2) - 4.0 * distance);
            int lowerRoot = (int)Math.Floor((time - 1.0 * discriminant) / 2.0);
            return time - lowerRoot - lowerRoot - 1;
        }
    }

    public class  Day06Tests
    {
        string _testInput = """
            Time:      7  15   30
            Distance:  9  40  200
            """.ReplaceLineEndings("\n");

        [Fact]
        public void Part1()
        {
            var timeDistance = Day06.ParseInput(_testInput);
            List<int> margins = timeDistance.Select(pair => Day06.GetRootDistance(pair.time, pair.distance)).ToList();
            Assert.Equal([4, 8, 9], margins);
        }
    }
}
