
using System.Security.Cryptography;

namespace AdventOfCode
{
    public class Day10 : BaseDay
    {
        private readonly string _input;

        public Day10()
        {
            _input = File.ReadAllText(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            int solution = TraverseLoop(_input);
            return new ValueTask<string>(solution.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            throw new NotImplementedException();
        }

        public static char[][] ParseInput(string input)
        {
            return input.Split('\n').Where(line => line != "").Select(line => line.ToCharArray()).ToArray();
        }

        public static (int, int) GetFirstPipe(char[][] pipes, int i, int j)
        {
            char above = pipes[i - 1][j];
            if (above == '|' || above == 'F' || above == '7')
                return (i - 1, j);
            char below = pipes[i + 1][j];
            if (below == '|' || below == 'L' || below == 'J')
                return (i + 1, j);
            char left = pipes[i][j - 1];
            if (left == '-' || left == 'F' || left == 'L')
                return (i, j - 1);
            return (i, j + 1);
        }

        public static int TraverseLoop(string input)
        {
            char[][] pipes = ParseInput(input);
            (int a, int b) initialPosition = (-1, -1);
            for (int i = 0; i < pipes.Length; i++)
                for (int j = 0; j < pipes[i].Length; j++)
                    if (pipes[i][j] == 'S')
                        initialPosition = (i, j);

            (int, int) position = GetFirstPipe(pipes, initialPosition.a, initialPosition.b);
            int a = position.Item1;
            int b = position.Item2;
            char current = pipes[a][b];
            int count = 0;
            string from;
            if (a < initialPosition.a)
                from = "below";
            else if (a > initialPosition.a)
                from = "above";
            else if (b < initialPosition.b)
                from = "right";
            else
                from = "left";

            while (current != 'S')
            {
                if (current == 'L')
                    if (from == "right")
                        from = "below";
                    else
                        from = "left";
                else if (current == 'F')
                    if (from == "below")
                        from = "left";
                    else
                        from = "above";
                else if (current == '7')
                    if (from == "below")
                        from = "right";
                    else
                        from = "above";
                else if (current == 'J')
                    if (from == "above")
                        from = "right";
                    else
                        from = "below";
                else
                    from = from;

                switch (from)
                {
                    case "below":
                        a--;
                        break;
                    case "above":
                        a++;
                        break;
                    case "left":
                        b++;
                        break;
                    case "right":
                        b--;
                        break;
                }

                current = pipes[a][b];
                count++;
            }
            return count / 2 + 1;
        }
    }

    public class Day10Tests
    {
        string _testInput1 = """
            -L|F7
            7S-7|
            L|7||
            -L-J|
            L|-JF
            """.ReplaceLineEndings("\n");

        string _testInput2 = """
            7-F7-
            .FJ|7
            SJLL7
            |F--J
            LJ.LJ
            """.ReplaceLineEndings("\n");

        [Fact]
        public void Part1()
        {
            int first = Day10.TraverseLoop(_testInput1);
            int second = Day10.TraverseLoop(_testInput2);
            Assert.Equal(4, first);
            Assert.Equal(8, second);
        }
    }
}
