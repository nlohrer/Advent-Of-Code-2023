namespace AdventOfCode
{
    public class Day07 : BaseDay
    {
        private readonly string _input;
        public static char[] cardTypes = ['1', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];

        public Day07()
        {
            _input = File.ReadAllText(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var parsed = Day07.ParseInput(_input);
            int total = parsed.OrderBy(hand => hand.GetTotalValue()).Select((hand, index) => (index + 1) * hand.bid).Sum();
            return new ValueTask<string>(total.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Hand> ParseInput(string input)
        {
            return input
                .Split('\n')
                .Where(str => str.Any())
                .Select(line =>
                {
                    var split = line.Split(' ');
                    return new Hand(split[0], int.Parse(split[1]));
                });

        }
    }


    public record Hand(string cards, int bid)
    {
        public int[] GetAmounts()
        {
            int[] amounts = new int[Day07.cardTypes.Length];
            foreach (char card in cards)
            {
                int rank = Array.IndexOf(Day07.cardTypes, card);
                amounts[rank]++;
            }
            return amounts;
        }

        public static int GetValue(int[] amounts)
        {
            int multiplier = 10000000;
            if (amounts.Any(amount => amount == 5))
                return 7 * multiplier;
            if (amounts.Any(amount => amount == 4))
                return 6 * multiplier;
            if (amounts.Any(amount => amount == 3))
                if (amounts.Any(amount => amount == 2))
                    return 5 * multiplier;
                else
                    return 4 * multiplier;

            for (int i = 0; i < amounts.Length; i++)
                for (int j = i + 1; j < amounts.Length; j++)
                    if (amounts[i] == 2 && amounts[j] == 2)
                        return 3 * multiplier;

            if (amounts.Any(amount => amount == 2))
                return 2 * multiplier;
            return 1 * multiplier;
        }

        public int GetTotalValue()
        {
            int[] amounts = this.GetAmounts();
            int kindValue = GetValue(amounts);
            int inherentValue = 0;
            for (int i = 0; i < cards.Length; i++)
                inherentValue += Array.IndexOf(Day07.cardTypes, cards[i]) * (int)Math.Round(Math.Pow(14, (cards.Length - i - 1)));
            return kindValue + inherentValue;
        }
    }

    public class Day07Tests()
    {
        string _testInput = """
            32T3K 765
            T55J5 684
            KK677 28
            KTJJT 220
            QQQJA 483
            """.ReplaceLineEndings("\n");

        [Fact]
        public void Solve()
        {
            var parsed = Day07.ParseInput(_testInput);
            int total = parsed.OrderBy(hand => hand.GetTotalValue()).Select((hand, index) => (index + 1) * hand.bid).Sum();
            Assert.Equal(6440, total);
        }
    }
}
