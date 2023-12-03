namespace AdventOfCode;

public class Day03 : BaseDay
{
    private static string _input;
    private static char[][] _schema;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
        _schema  = _input
        .Split('\n')
        .Where(line => line != "")
        .Select(line => line.ToArray())
        .ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        int sum = FindNumbersAdjacentToSymbols(_schema).Sum();
        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }

    public static List<int> FindNumbersAdjacentToSymbols(char[][] schematic)
    {
        List<int> numbers = new();
        bool adjacent;

        for (int i = 0; i < schematic.Length; i++)
        {
            int j = 0;
            while (j < schematic[i].Length)
            {
                if (int.TryParse(schematic[i][j].ToString(), out int firstDigit))
                {
                    List<char> number = new() { schematic[i][j] };
                    adjacent = AdjacentToSymbol(i, j, schematic);
                    j++;
                    while (j < schematic[i].Length && int.TryParse(schematic[i][j].ToString(), out int _))
                    {
                        number.Add(schematic[i][j]);
                        if (AdjacentToSymbol(i, j, schematic))
                        {
                            adjacent = true;
                        }
                        j++;
                    }
                    if (adjacent)
                    {
                        numbers.Add(int.Parse(new string(number.ToArray())));
                    }
                } else
                {
                    j++;
                }
            }
        }
        return numbers;
    }

    public static bool AdjacentToSymbol(int row, int col, char[][] schematic)
    {
        for (int currentRowIndex = row - 1; currentRowIndex < row + 2; currentRowIndex++)
        {
            if (currentRowIndex < 0 || currentRowIndex >= schematic.Length)
            {
                continue;
            }

            char[] currentRow = schematic[currentRowIndex];
            for (int currentColIndex = col - 1; currentColIndex < col + 2; currentColIndex++)
            {
                if (currentColIndex < 0 || currentColIndex >= currentRow.Length)
                {
                    continue;
                }
                char current = currentRow[currentColIndex];

                if (!int.TryParse(current.ToString(), out int _) && current != '.' && current != ' ')
                {
                    return true;
                }
            }
        }
        return false;
    }
}

public class Day03Tests
{
    static string _testInput = """
        467..114..
        ...*......
        ..35..633.
        ......#...
        617*......
        .....+.58.
        ..592.....
        ......755.
        ...$.*....
        .664.598..

        """;
    static char[][] _schema = _testInput
        .Split(Environment.NewLine)
        .Select(line => line.ToArray())
        .ToArray();

    [Fact]
    public void CheckForAdjacency()
    {
        List<ValueTuple<int, int>> validCoordinates = new();
        List<ValueTuple<int, int>> expected = new()
        {
            (0,2), (2,2), (2,3), (2,6), (2,7), (4,2), (6,4), (7,6), (9,2), (9,3), (9,5), (9,6)
        };

        for (int i = 0; i < _schema.Length; i++)
        {
            for (int j = 0; j < _schema[i].Length; j++)
            {
                if (int.TryParse(_schema[i][j].ToString(), out int _) && Day03.AdjacentToSymbol(i, j, _schema))
                {
                    validCoordinates.Add((i, j));
                }
            }
        }

        Assert.Equal(expected, validCoordinates);
    }

    [Fact]
    public void SumNumbersAdjacentToSymbols()
    {
        List<int> numbers = Day03.FindNumbersAdjacentToSymbols(_schema);
        int sum = numbers.Sum();
        int expected = 4361;
        Assert.Equal(expected, sum);
    }
}
