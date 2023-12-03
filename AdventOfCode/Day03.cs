namespace AdventOfCode;

public class Day03 : BaseDay
{
    private static string _input;
    private static char[][] _schema;

    public Day03()
    {
        _input = File.ReadAllText(InputFilePath);
        _schema = _input
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
        int sum = GetGearRatios(_schema).Sum();
        return new ValueTask<string>(sum.ToString());
    }

    public static List<int> GetGearRatios(char[][] schematic)
    {
        Dictionary<ValueTuple<int, int>, HashSet<int>> gearsWithAdjacentNumbers = GetGearsWithAdjacentNumbers(schematic);
        var gearRatios = new List<int>();

        foreach (var gearWithAdjacentNumbers in gearsWithAdjacentNumbers)
        {
            HashSet<int> adjacent = gearWithAdjacentNumbers.Value;
            if (adjacent.Count == 2)
            {
                int gearRatio = adjacent.Aggregate(1, (x, y) => x * y);
                gearRatios.Add(gearRatio);
            }
        }

        return gearRatios;
    }

    public static Dictionary<ValueTuple<int, int>, HashSet<int>> GetGearsWithAdjacentNumbers(char[][] schematic)
    {
        Dictionary<ValueTuple<int, int>, HashSet<int>> gearsWithAdjacentNumbers = new();

        for (int i = 0; i < schematic.Length; i++)
        {
            int j = 0;
            while (j < schematic[i].Length)
            {
                if (char.IsNumber(schematic[i][j]))
                {
                    List<char> number = new();
                    HashSet<ValueTuple<int, int>> adjacentGears = new();
                    while (j < schematic[i].Length && char.IsNumber(schematic[i][j]))
                    {
                        number.Add(schematic[i][j]);
                        foreach (ValueTuple<int, int> adjacentGear in GetAdjacentGears(i, j, schematic))
                            adjacentGears.Add(adjacentGear);
                        j++;
                    }
                    int parsedNumber = int.Parse(new string(number.ToArray()));
                    foreach (ValueTuple<int, int> gear in adjacentGears)
                    {
                        if (gearsWithAdjacentNumbers.ContainsKey(gear))
                            gearsWithAdjacentNumbers[gear].Add(parsedNumber);
                        else
                            gearsWithAdjacentNumbers[gear] = new HashSet<int> { parsedNumber };
                    }
                }
                else
                    j++;
            }
        }
        return gearsWithAdjacentNumbers;
    }

    public static List<int> FindNumbersAdjacentToSymbols(char[][] schematic)
    {
        List<int> numbers = new();

        for (int i = 0; i < schematic.Length; i++)
        {
            int j = 0;
            while (j < schematic[i].Length)
            {
                if (char.IsNumber(schematic[i][j]))
                {
                    List<char> number = new();
                    bool adjacent = false;
                    while (j < schematic[i].Length && char.IsNumber(schematic[i][j]))
                    {
                        number.Add(schematic[i][j]);
                        adjacent |= AdjacentToSymbol(i, j, schematic);
                        j++;
                    }
                    if (adjacent)
                        numbers.Add(int.Parse(new string(number.ToArray())));
                }
                else
                    j++;
            }
        }
        return numbers;

        // Functional solution
        //IEnumerable<int> rowIndices = Enumerable.Range(0, schematic.Length),
        //    colIndices = Enumerable.Range(0, schematic[0].Length);

        //var found = (from i in rowIndices
        //        from j in colIndices
        //        where
        //             char.IsNumber(schematic[i][j])
        //             && AdjacentToSymbol(i, j, schematic)
        //        let lastIndex = findIndexOfLast(schematic[i], j)
        //        let number = new string(completeNumber(schematic[i], lastIndex).ToArray())
        //        select ((i, j), int.Parse(number)))
        //       .ToDictionary();

        //var indices = found.Keys.Where(tuple => !found.Keys.Contains((tuple.i, tuple.j - 1)));
        //return found.Where(tuple => indices.Contains(tuple.Key)).Select(tuple => tuple.Value).ToList();

        //int findIndexOfLast(char[] col, int j)
        //{
        //    if (j == col.Length - 1) return j;
        //    char following = col[j + 1];
        //    if (!char.IsNumber(following)) return j;
        //    else return findIndexOfLast(col, j + 1);
        //}

        //List<char> completeNumber(char[] col, int j)
        //{
        //    if (j == -1) return [];
        //    char current = col[j];
        //    if (!char.IsNumber(current)) return [];
        //    List<char> proceeding = completeNumber(col, j - 1);
        //    proceeding.Add(current);
        //    return proceeding;
        //}
    }

    public static HashSet<ValueTuple<int, int>> GetAdjacentGears(int row, int col, char[][] schematic)
    {
        IEnumerable<int> rows = Enumerable.Range(row - 1, 3), cols = Enumerable.Range(col - 1, 3);
        return (from i in rows
                from j in cols
                where
                    i >= 0 && j >= 0
                    && i < schematic.Length && j < schematic[i].Length
                    && schematic[i][j] == '*'
                select (i, j))
                .ToHashSet();
    }

    public static bool AdjacentToSymbol(int row, int col, char[][] schematic)
    {
        IEnumerable<int> rows = Enumerable.Range(row - 1, 3), cols = Enumerable.Range(col - 1, 3);

        return (from i in rows
                from j in cols
                where
                    i >= 0 && j >= 0
                    && i < schematic.Length && j < schematic[i].Length
                let current = schematic[i][j]
                where !char.IsDigit(current) && current != '.' && current != ' '
                select current
                ).Any();

        // Alternative:
        //Func<bool, bool, bool> or = (bool x, bool y) => x || y;
        //return rows
        //    .Select(i => i >= 0 && i < schematic.Length && cols
        //        .Select(j =>
        //        {
        //            if (j < 0 || j >= schematic[i].Length)
        //                return false;
        //            char current = schematic[i][j];
        //            return !char.IsDigit(current) && current != '.' && current != ' ';
        //        })
        //        .Aggregate(or))
        //    .Aggregate(or);
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

    [Fact]
    public void CheckForGearAdjacency()
    {
        int sum = Day03.GetGearRatios(_schema).Sum();
        Assert.Equal(467835, sum);
    }
}
