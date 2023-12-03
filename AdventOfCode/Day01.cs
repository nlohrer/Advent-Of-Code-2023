namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;
    public static Dictionary<string, int> stringMappings = new()
    {
        {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4}, {"five", 5}, {"six", 6}, {"seven", 7 }, {"eight", 8}, {"nine", 9},
        {"1", 1}, {"2", 2}, {"3", 3}, {"4", 4}, {"5", 5}, {"6", 6}, {"7", 7}, {"8", 8}, {"9", 9}
    };
    public static List<string> validStrings = stringMappings.Keys.ToList();

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        int sum = _input
            .Split('\n')
            .Select(ParseLine)
            .Sum();
        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int sum = _input
            .Split('\n')
            .Where(line => line != "")
            .Select(ParseLinePartTwo)
            .Sum();
        return new ValueTask<string>(sum.ToString());
    }

    public static int ParseLine(string line)
    {
        if (line == "") return 0;
        IEnumerable<string> numbersInLine = line
             .ToCharArray()
             .Select(character => character.ToString())
             .Where(character => int.TryParse(character, out int _));
        int parsedNumber = int.Parse($"{numbersInLine.First()}{numbersInLine.Last()}");
        return parsedNumber;
    }

    public static int ParseLinePartTwo(string line)
    {
        Dictionary<int, string> substringFirstOccurrences = new();
        Dictionary<int, string> substringLastOccurrences = new();

        foreach (string validString in validStrings) {
            int firstOccurrenceIndex = line.IndexOf(validString);
            if (firstOccurrenceIndex >= 0)
            {
                substringFirstOccurrences.Add(firstOccurrenceIndex, validString);
            }
            int lastOccurrenceIndex = line.LastIndexOf(validString);
            if (lastOccurrenceIndex >= 0)
            {
                substringLastOccurrences.Add(lastOccurrenceIndex, validString);
            }
        }

        int lowestIndex = substringFirstOccurrences.Keys.Min();
        int highestIndex = substringLastOccurrences.Keys.Max();

        string combined = stringMappings.GetValueOrDefault(substringFirstOccurrences.GetValueOrDefault(lowestIndex)).ToString() + 
            stringMappings.GetValueOrDefault(substringLastOccurrences.GetValueOrDefault(highestIndex)).ToString();
        return int.Parse(combined);
    }
}

public class Day01Tests
{
    string _testInput = """
        1abc2
        pqr3stu8vwx
        a1b2c3d4e5f
        treb7uchet
        """;

    [Fact]
    public void FirstPartTest()
    {
        var parsed = _testInput.Split();
        int[] parsedLines = _testInput
            .Split(Environment.NewLine)
            .Select(Day01.ParseLine)
            .ToArray();
        IEnumerable<int> expected = [12, 38, 15, 77];
        Assert.Equal(expected, parsedLines);
    }

    [Fact]
    public void ParseLineCorrectly()
    {
        string input = "4nineeightseven2";
        int result = Day01.ParseLinePartTwo(input);

        Assert.Equal(42, result);
    }

    [Theory]
    [InlineData("two1nine", 29)]
    [InlineData("eightwothree", 83)]
    [InlineData("abcone2threexyz", 13)]
    [InlineData("xtwone3four", 24)]
    [InlineData("4nineeightseven2", 42)]
    [InlineData("zoneight234", 14)]
    [InlineData("7pqrstsixteen", 76)]
    public void ParseTestLinesCorrectly(string input, int expected)
    {
        int result = Day01.ParseLinePartTwo(input);
        Assert.Equal(expected, result);
    }
        

}
