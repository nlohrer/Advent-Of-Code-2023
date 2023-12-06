using Spectre.Console;

namespace AdventOfCode
{
    public class Day05 : BaseDay
    {
        private readonly string _input;

        public Day05()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            //List<long> locations = ApplyMaps(_input, GetSeedsPart01);
            List<SRange> locations = Apply2(_input, GetSeedsPart01new);
            long solution = locations.Select(range => range.Min).Min();
            return new ValueTask<string>(solution.ToString());
        }

        // Brute force solution that took 67 minutes to run
        public override ValueTask<string> Solve_2()
        {
            //List<long> locations = ApplyMaps(_input, GetSeedsPart02);
            List<SRange> locations = Apply2(_input, GetSeedsPart03);
            long solution = locations.Select(range => range.Min).Min();
            return new ValueTask<string>(solution.ToString());
        }

        public static List<SRange> Apply2(string input, Func<string, List<SRange>> GetSeeds)
        {
            string[] split = input.Split("\n\n");
            List<SRange> seeds = GetSeeds(input);
            var maps = split.Skip(1)
                .Select(parts => parts.Split('\n').Skip(1)
                    .Where(str => str != "")
                    .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                    .ToList())
                .ToList();
            for (int i = 0; i < maps.Count; i++)
            {
                seeds = seeds.Select(seed => CalculatePossible(seed, maps[i])).Aggregate((seeds1, seeds2) => seeds1.Concat(seeds2).ToList()).ToList();
            }
            return seeds;
        }

        public static List<long> ApplyMaps(string input, Func<string, List<long>> GetSeeds)
        {
            string[] split = input.Split("\n\n");
            List<long> seeds = GetSeeds(input);
            List<Func<long, long>> maps = split.Skip(1)
                .Select(parts => parts.Split('\n').Skip(1)
                    .Where(str => str != "")
                    .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                    .ToList())
                .Select(GetMap).ToList();
            Func<long, long> totalMap = maps
                .Aggregate((Func<long, long> map1, Func<long, long> map2) =>
                    (long source) => map2(map1(source)));
            List<long> locations = new();
            long min = long.MaxValue;
            foreach (long seed in seeds)
            {
                long location = totalMap(seed);
                if (location < min)
                    min = location;
            }
            //List<long> locations = seeds.Select(seed => totalMap(seed)).ToList();
            return new List<long> { min };
        }

        public static List<long> GetSeedsPart01(string input)
        {
            string[] split = input.Split("\n\n");
            List<long> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).ToList();
            return seeds;
        }

        public static List<long> GetSeedsPart02(string input)
        {
            string[] split = input.Split("\n\n");
            List<long> seedList = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).ToList();
            List<long> seeds = new();
            for (int i = 0; i < seedList.Count; i += 2)
                for (long j = seedList[i]; j < seedList[i] + seedList[i + 1]; j++)
                    seeds.Add(j);
            return seeds;
        }

        public static List<SRange> GetSeedsPart01new(string input)
        {
            string[] split = input.Split("\n\n");
            List<SRange> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).Select(seed => new SRange(seed, seed)).ToList();
            return seeds;
        }

        public static List<SRange> GetSeedsPart03(string input)
        {
            string[] split = input.Split("\n\n");
            List<long> seedList = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).ToList();
            List<SRange> seeds = new();
            for (int i = 0; i < seedList.Count; i += 2)
            {
                SRange seed = new SRange(seedList[i], seedList[i] + seedList[i + 1]);
                seeds.Add(seed);
            }
            return seeds;
        }

        //public static List<Range> GetRangeMap(List<MapPart> mapParts)
        //{
        //    mapParts.ForEach(mapPart =>
        //    {
        //        Range originalRange = new Range()
        //    })
        //}

        public static Func<long, long> GetMap(List<MapPart> mapParts)
        {
            Func<long, long> map = (long source) =>
            {
                MapPart fitting = mapParts
                .Where(mapPart => mapPart.Source <= source && source < mapPart.Source + mapPart.Range)
                .FirstOrDefault(new MapPart(source, source, 0));
                return source - fitting.Source + fitting.Destination;
            };
            return map;
        }

        public static List<SRange> CalculatePossible(SRange interval, List<MapPart> ranges)
        {
            List<SRange> intersects = new();
            List<SRange> newRanges = new();

            foreach (MapPart mapPart in ranges)
            {
                SRange asRange = new(mapPart.Source, mapPart.Source + mapPart.Range);
                if (interval.Max < asRange.Min || asRange.Max < interval.Min)
                    continue;
                long difference = mapPart.Destination - mapPart.Source;
                newRanges.Add(interval.Intersection(asRange).Add(difference));
                intersects.Add(interval.Intersection(asRange));
            }
            if (!intersects.Any())
                return [interval];

            List<SRange> intersectsSorted = intersects.OrderBy(range => range.Min).ToList();
            List<SRange> staySame = new();

            for (int i = 0; i < intersectsSorted.Count - 1; i++)
            {
                SRange intersect1 = intersectsSorted[i], intersect2 = intersectsSorted[i + 1];
                SRange same = intersect1.GetBetween(intersect2);
                if (same.Min != same.Max)
                    newRanges.Add(same);
            }

            //newRanges = intersects.Union(staySame).ToList();
            long min = intersectsSorted[0].Min;
            long max = intersectsSorted[intersectsSorted.Count - 1].Max;

            if (interval.Min < min)
                newRanges.Add(new(interval.Min, min));
            if (interval.Max > max)
                newRanges.Add(new(max, interval.Max));
            return newRanges;
        }

    }

    public record MapPart(long Destination, long Source, long Range)
    {
        public static MapPart FromArray(string[] array)
        {
            long[] arr = array.Select(long.Parse).ToArray();
            return new MapPart(arr[0], arr[1], arr[2]);
        }
    }

    public record SRange(long Min, long Max)
    {
        public SRange Intersection(SRange b)
        {
            return new SRange(
                Min: this.Min > b.Min ? this.Min : b.Min,
                Max: this.Max < b.Max ? this.Max : b.Max
            );
        }

        public SRange GetBetween(SRange b)
        {
            return new SRange(
                Min: this.Max <= b.Min ? this.Max : b.Max,
                Max: this.Max <= b.Min ? b.Min : this.Min
                );
        }

        public SRange Add(long difference)
        {
            return new SRange(Min + difference, Max + difference);
        }
    }

    public class Day05Tests
    {
        [Fact]
        public void ApplyOnce()
        {
            string[] split = _testInput.Split("\n\n");
            List<long> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).ToList();
            List<Func<long, long>> maps = split.Skip(1)
               .Select(parts => parts.Split('\n').Skip(1)
                   .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                   .ToList())
               .Select(Day05.GetMap).ToList();
            Func<long, long> first = maps[0];
            List<long> soils = seeds.Select(first).ToList();

            Assert.Equal([81, 14, 57, 13], soils);
        }


        [Fact]
        public void Part01()
        {
            List<long> locations = Day05.ApplyMaps(_testInput, Day05.GetSeedsPart01);
            Assert.Equal([82, 43, 86, 35], locations);
        }

        [Fact]
        public void Part02()
        {
            List<long> locations = Day05.ApplyMaps(_testInput, Day05.GetSeedsPart02);
            // Assert.Equal(46, locations.Min());
        }

        [Fact]
        public void Part03()
        {
            List<SRange> finalRanges = Day05.Apply2(_testInput, Day05.GetSeedsPart03);
            long min = finalRanges.Select(range => range.Min).Min();
            Assert.Equal(46, min);
        }

        [Fact]
        public void TestRanges()
        {
            SRange initial = new SRange(50, 80);
            //SRange s1 = new(55, 65), s2 = new(68, 70), s3 = new(70, 74), s4 = new(78, 90);
            //List<SRange> ranges = [s1, s2, s3, s4];
            List<MapPart> mapParts = new() {
                new(0, 30, 4), new(58, 55, 10), new(48, 68, 2), new(100, 70, 4), new(58, 78, 2), new(120, 130, 5)
            };
            List<SRange> next = Day05.CalculatePossible(initial, mapParts);
            Assert.Equal(new HashSet<SRange> { new(50, 55), new(58, 68), new(65, 68), new(48, 50), new(100, 104), new(74, 78), new(58, 60) }, next.ToHashSet());
        }

        [Fact]
        public void TestRanges2()
        {
            SRange initial = new SRange(20, 30);

        }

        string _testInput = """
            seeds: 79 14 55 13

            seed-to-soil map:
            50 98 2
            52 50 48

            soil-to-fertilizer map:
            0 15 37
            37 52 2
            39 0 15

            fertilizer-to-water map:
            49 53 8
            0 11 42
            42 0 7
            57 7 4

            water-to-light map:
            88 18 7
            18 25 70

            light-to-temperature map:
            45 77 23
            81 45 19
            68 64 13

            temperature-to-humidity map:
            0 69 1
            1 0 69

            humidity-to-location map:
            60 56 37
            56 93 4
            """.ReplaceLineEndings("\n");
    }
}
