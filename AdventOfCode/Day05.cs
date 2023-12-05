
using System.Runtime.CompilerServices;

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
            List<uint> locations = ApplyMaps(_input, GetSeedsPart01);
            uint solution = locations.Min();
            return new ValueTask<string>(solution.ToString());
        }

        // Brute force solution that took 67 minutes to run
        public override ValueTask<string> Solve_2()
        {
            List<uint> locations = ApplyMaps(_input, GetSeedsPart02);
            uint solution = locations.Min();
            return new ValueTask<string>(solution.ToString());        }

        public static List<uint> ApplyMaps(string input, Func<string, List<uint>> GetSeeds)
        {
            string[] split = input.Split("\n\n");
            List<uint> seeds = GetSeeds(input);
            List<Func<uint, uint>> maps = split.Skip(1)
                .Select(parts => parts.Split('\n').Skip(1)
                    .Where(str => str != "")
                    .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                    .ToList())
                .Select(GetMap).ToList();
            Func<uint, uint> totalMap = maps
                .Aggregate((Func<uint, uint> map1, Func<uint, uint> map2) => 
                    (uint source) => map2(map1(source)));
            List<uint> locations = new();
            uint min = uint.MaxValue;
            foreach (uint seed in seeds)
            {
                uint location = totalMap(seed);
                if (location < min)
                    min = location;
            }
            //List<uint> locations = seeds.Select(seed => totalMap(seed)).ToList();
            return new List<uint> { min };
        }

        public static List<uint> GetSeedsPart01(string input)
        {
            string[] split = input.Split("\n\n");
            List<uint> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(uint.Parse).ToList();
            return seeds;
        }

        public static List<uint> GetSeedsPart02(string input)
        {
            string[] split = input.Split("\n\n");
            List<uint> seedList = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(uint.Parse).ToList();
            List<uint> seeds = new();
            for (int i = 0; i < seedList.Count; i += 2)
                for (uint j = seedList[i]; j < seedList[i] + seedList[i + 1]; j++)
                    seeds.Add(j);
            return seeds;
        }

        //public static List<Range> GetRangeMap(List<MapPart> mapParts)
        //{
        //    mapParts.ForEach(mapPart =>
        //    {
        //        Range originalRange = new Range()
        //    })
        //}

        public static Func<uint, uint> GetMap(List<MapPart> mapParts)
        {
            Func<uint, uint> map = (uint source) =>
            {
                MapPart fitting = mapParts
                .Where(mapPart => mapPart.Source <= source && source < mapPart.Source + mapPart.Range)
                .FirstOrDefault(new MapPart(source, source, 0));
                return source - fitting.Source + fitting.Destination;
            };
            return map;
        }

        public static List<SRange> CalculatePossible(SRange interval, List<SRange> ranges)
        {
            List<SRange> intersects = new();
            //List<SRange> newRanges = new();

            foreach (SRange range in ranges)
            {
                intersects.Add(interval.Intersection(range));
            }

            List<SRange> staySame = new();
            for (int i = 0; i < intersects.Count - 1; i++)
            {
                SRange intersect1 = intersects[i], intersect2 = intersects[i + 1];
                SRange same = intersect1.GetBetween(intersect2);
                if (same.Min != same.Max)
                    staySame.Add(same);
            }

            List<SRange> newRanges = intersects.Union(staySame).ToList();
            return newRanges;
        }

    }

    public record MapPart(uint Destination, uint Source, uint Range)
    {
        public static MapPart FromArray(string[] array)
        {
            uint[] arr = array.Select(uint.Parse).ToArray();
            return new MapPart(arr[0], arr[1], arr[2]);
        }
    }

    public record SRange(uint Min, uint Max)
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

        public SRange Add(uint difference)
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
            List<uint> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(uint.Parse).ToList();
            List<Func<uint, uint>> maps = split.Skip(1)
               .Select(parts => parts.Split('\n').Skip(1)
                   .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                   .ToList())
               .Select(Day05.GetMap).ToList();
            Func<uint, uint> first = maps[0];
            List<uint> soils = seeds.Select(first).ToList();

            Assert.Equal([81, 14, 57, 13], soils);
        }


        [Fact]
        public void Part01()
        {
            List<uint> locations = Day05.ApplyMaps(_testInput, Day05.GetSeedsPart01);
            Assert.Equal([82, 43, 86, 35], locations);
        }

        [Fact]
        public void Part02()
        {
            List<uint> locations = Day05.ApplyMaps(_testInput, Day05.GetSeedsPart02);
            // Assert.Equal(46, locations.Min());
        }

        [Fact]
        public void TestRanges()
        {
            SRange initial = new SRange(50, 80);
            SRange s1 = new(55, 65), s2 = new(68, 70), s3 = new(70, 74), s4 = new(78, 90);
            List<SRange> ranges = [s1, s2, s3, s4];
            List<SRange> next = Day05.CalculatePossible(initial, ranges);
            Assert.Equal(1, 2);
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
