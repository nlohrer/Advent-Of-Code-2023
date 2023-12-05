
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
            List<long> locations = ApplyMaps(_input);
            long solution = locations.Min();
            return new ValueTask<string>(solution.ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            throw new NotImplementedException();
        }

        public static List<long> ApplyMaps(string _input)
        {
            string[] split = _input.Split("\n\n");
            List<long> seeds = split[0].Substring(split[0].IndexOf(':') + 2).Split(' ').Select(long.Parse).ToList();
            List<Func<long, long>> maps = split.Skip(1)
                .Select(parts => parts.Split('\n').Skip(1)
                    .Where(str => str != "")
                    .Select(mapPart => MapPart.FromArray(mapPart.Split(' ')))
                    .ToList())
                .Select(GetMap).ToList();
            Func<long, long> totalMap = maps
                .Aggregate((Func<long, long> map1, Func<long, long> map2) => 
                    (long source) => map2(map1(source)));
            List<long> locations = seeds.Select(seed => totalMap(seed)).ToList();
            return locations;
        }

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

    }

    public record MapPart(long Destination, long Source, long Range)
    {
        public static MapPart FromArray(string[] array)
        {
            long[] arr = array.Select(long.Parse).ToArray();
            return new MapPart(arr[0], arr[1], arr[2]);
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
            List<long> locations = Day05.ApplyMaps(_testInput);
            Assert.Equal([82, 43, 86, 35], locations);
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
