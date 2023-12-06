// See https://aka.ms/new-console-template for more information

using System;
using System.Runtime.CompilerServices;

DateTime start = DateTime.Now;
var lines = System.IO.File.ReadAllLines("input.txt");

Int64 part1Answer = 0;
Int64[] seeds = null;
List<(Int64, Int64, Int64)> seedToSoilMap = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> soilToFertilizerMap = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> fertilizerToWaterMap = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> waterToLightMap = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> lightToTempMap = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> tempToHumidity = new List<(Int64, Int64, Int64)>();
List<(Int64, Int64, Int64)> humidityToLocation = new List<(Int64, Int64, Int64)>();
List<List<(Int64, Int64, Int64)>> allMaps = new List<List<(Int64, Int64, Int64)>>()
{
    seedToSoilMap,
    soilToFertilizerMap,
    fertilizerToWaterMap,
    waterToLightMap,
    lightToTempMap,
    tempToHumidity,
    humidityToLocation
};

List<(Int64, Int64, Int64)> currentMap = seedToSoilMap;
for (int lineNum = 0; lineNum < lines.Length; lineNum++)
{
    var line = lines[lineNum];
    if (line.StartsWith("seeds: "))
    {
        seeds = line.Substring(7, line.Length - 7).Split(' ').Select(s => Int64.Parse((s))).ToArray();
        continue;
    }

    if (line.StartsWith("seed-to-soil map"))
        currentMap = seedToSoilMap;
    if (line.StartsWith("soil-to-fertilizer map"))
        currentMap = soilToFertilizerMap;
    if (line.StartsWith("fertilizer-to-water map"))
        currentMap = fertilizerToWaterMap;
    if (line.StartsWith("water-to-light map"))
        currentMap = waterToLightMap;
    if (line.StartsWith("light-to-temperature map"))
        currentMap = lightToTempMap;
    if (line.StartsWith("temperature-to-humidity map"))
        currentMap = tempToHumidity;
    if (line.StartsWith("humidity-to-location map"))
        currentMap = humidityToLocation;

    var currentMaps = line.Split(" ");
    if (currentMaps.Length == 3)
    {
        currentMap.Add((Int64.Parse(currentMaps[0]), Int64.Parse(currentMaps[1]), Int64.Parse(currentMaps[2])));
    }
}

var ob = new object();
var minimum = Int64.MaxValue;
//Parallel.For(0, seeds.Length/2, (i ) =>  {
//        Int64 localMinimum = Int64.MaxValue;
//    for (Int64 j = 0; j <= seeds[i * 2 +1]; j++)
//    {
//        Int64 index = seeds[i * 2] + j;
//        foreach (var mapping in allMaps)
//        {
//            Int64 nextIndex = index;
//            foreach (var mapElement in mapping)
//            {
//                Int64 count = mapElement.Item3 - 1;
//                if (index >= mapElement.Item2 && index <= mapElement.Item2 + count)
//                {
//                    nextIndex = mapElement.Item1 + index - mapElement.Item2;
//                    break;
//                }
//            }

//            index = nextIndex;
//        }

//        localMinimum = Math.Min(index, localMinimum);
//    }

//    lock (ob)
//    {
//        minimum = Math.Min(localMinimum, minimum);
//    }
//});


// This is not how I implemented the problem. And it was faster to just brute force it, but this is much nicer.
for (int i = 0; i < seeds.Length; i += 2)
{
    List<(Int64 from, Int64 to)> ranges = new List<(long, long)>() { (seeds[i], seeds[i] + seeds[i + 1]) };
    foreach (var mapping in allMaps)
    {

        List<(Int64 from, Int64 to)> newRanges = new List<(long, long)>() { };
        foreach (var mapElement in mapping)
        {

            List<(Int64 from, Int64 to)> passedThroughRanges = new List<(long, long)>() { };
            foreach (var range in ranges)
            {
                Int64 count = mapElement.Item3 - 1;
                var rangeFrom = range.from;
                var rangeTo = range.to;
                var mappingFrom = mapElement.Item2;
                var mappingTo = mapElement.Item2 + count;
                if (mappingFrom > rangeTo)
                {
                    passedThroughRanges.Add(range);
                }
                else if (mappingTo < rangeFrom)
                {
                    passedThroughRanges.Add(range);
                }
                else
                {
                    // split the range
                    // get the bit before the mapping
                    var beforeFrom = Math.Min(mappingFrom, rangeFrom);
                    var beforeTo = Math.Min(mappingFrom - 1, rangeTo);
                    if (beforeFrom <= beforeTo)
                    {
                        passedThroughRanges.Add((beforeFrom, beforeTo));
                        rangeFrom = beforeTo + 1;
                    }

                    // get the bit after
                    var afterFrom = Math.Max(mappingTo + 1, rangeFrom);
                    var afterTo = Math.Max(mappingTo, rangeTo);
                    if (afterFrom <= afterTo)
                    {
                        passedThroughRanges.Add((afterFrom, afterTo));
                        rangeTo = afterFrom - 1;
                    }

                    // get the bit inside
                    if (rangeFrom <= rangeTo)
                    {
                        newRanges.Add((rangeFrom + mapElement.Item1 - mapElement.Item2,
                            rangeTo + mapElement.Item1 - mapElement.Item2));
                    }

                }
            }

            ranges = passedThroughRanges;
        }

        ranges.AddRange(newRanges);
    }

    var sorted = ranges.OrderBy(r => r.from);
    minimum = Math.Min(minimum, sorted.First().from);
}


DateTime end = DateTime.Now;
Console.WriteLine($"Part 2 answer = {minimum}");
Console.WriteLine($"Calculation took {end - start}");

