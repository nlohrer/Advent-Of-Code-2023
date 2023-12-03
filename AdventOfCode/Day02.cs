namespace AdventOfCode
{
    public class Day02 : BaseDay
    {
        private readonly string _input;
        public static Dictionary<string, int> maxima = new() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

        public Day02()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            int count = CountValidGames(_input);
            return new ValueTask<string>(count.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            int sum = SumUpMinima(_input);
            return new ValueTask<string>(sum.ToString());
        }

        public static int CountValidGames(string input)
        {
            bool[] validityArray = input
                .Split('\n')
                .Select(CheckForValidMaxima)
                .ToArray();

            int count = 0;
            for (int i = 1; i < validityArray.Length + 1; ++i)
            {
                if (validityArray[i - 1])
                {
                    count += i;
                }
            }
            return count;
        }

        public static bool CheckForValidMaxima(string game)
        {
            if (game == "") return false;
            string subGames = game.Substring(game.IndexOf(':') + 1);
            foreach (string subGame in subGames.Split(';'))
            {
                Dictionary<string, int> subGameValues = ParseSubGame(subGame);
                foreach (string color in maxima.Keys)
                {
                    if (subGameValues[color] > maxima[color])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static int SumUpMinima(string input)
        {
            int sum = input
                .Split('\n')
                .Select(DetermineMinima)
                .Select(minima => minima.Values.Aggregate(1, (x, y) => x * y))
                .Sum();
            return sum;
        }

        public static Dictionary<string, int> DetermineMinima(string game)
        {
            Dictionary<string, int> initialMaxima = new() { { "blue", 0 }, { "red", 0 }, { "green", 0 } };
            if (game == "") return initialMaxima;

            string subGames = game.Substring(game.IndexOf(':') + 1);
            foreach (string subGame in subGames.Split(';'))
            {
                Dictionary<string, int> subGameValues = ParseSubGame(subGame);
                foreach (string color in new string[] { "red", "blue", "green" })
                {
                    if (initialMaxima[color] < subGameValues[color])
                    {
                        initialMaxima[color] = subGameValues[color];
                    }
                }
            }

            return initialMaxima;
        }

        public static Dictionary<string, int> ParseSubGame(string subGame)
        {
            Dictionary<string, int> parsedSubgames = subGame
                .Split(',')
                .Select(ParseSingleColor)
                .ToDictionary();
            foreach (string color in new string[] { "red", "blue", "green" })
            {
                if (!parsedSubgames.ContainsKey(color))
                {
                    parsedSubgames[color] = 0;
                }
            }
            return parsedSubgames;
        }

        public static ValueTuple<string, int> ParseSingleColor(string subGame)
        {
            string[] cubes = subGame.Trim().Split(' ');
            if (int.TryParse(cubes[0], out int amount))
            {
                string color = cubes[1];
                return (color, amount);
            }
            throw new ArgumentException();
        }
    }

    public class Day02Tests
    {
        string _testInput = """
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
            """;

        [Theory]
        [InlineData(" 3 blue", "blue", 3)]
        [InlineData(" 4 red", "red", 4)]
        [InlineData(" 6 blue", "blue", 6)]
        public void ParseSingleColorString(string subGame, string expectedColor, int expectedAmount)
        {
            ValueTuple<string, int> parsedSubgame = Day02.ParseSingleColor(subGame);
            Assert.Equal((expectedColor, expectedAmount), parsedSubgame);
        }

        [Fact]
        public void ParseSubGame()
        {
            string subGame = " 3 blue, 4 red";
            var expectedDictionary = new Dictionary<string, int> { { "blue", 3 }, { "green", 0 }, { "red", 4 } };
            Assert.Equal(expectedDictionary, Day02.ParseSubGame(subGame));
        }

        [Fact]
        public void CheckTestInputForPossibleGames()
        {
            bool[] values = _testInput
                .Split(Environment.NewLine)
                .Select(Day02.CheckForValidMaxima)
                .ToArray();
            bool[] expected = [true, true, false, false, true];
            Assert.Equal(expected, values);
        }

        [Fact]
        public void CheckTestInputCount()
        {
            int count = Day02.CountValidGames(_testInput);
            Assert.Equal(8, count);
        }

        [Fact]
        public void CheckMinimaForSingleGame()
        {
            string input = "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red";
            Dictionary<string, int> expected = new() { { "green", 13 }, { "red", 20 }, { "blue", 6 } };
            Dictionary<string, int> maxima = Day02.DetermineMinima(input);
            Assert.Equal(expected, maxima);
        }

        [Fact]
        public void SumUpTestInput()
        {
            int sum = Day02.SumUpMinima(_testInput);
            Assert.Equal(2286, sum);
        }
    }
}
