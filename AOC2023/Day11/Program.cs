
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Utilities;

class Day11
{
    static int Main()
    {
        DoPart1("sample.txt");
        DoPart1("input.txt");

        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }


  


    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<string> map;
        HashSet<int> XCountsDouble = new HashSet<int>();
        HashSet<int> yCountsDouble = new HashSet<int>();

        List<(int, int)> locations = new List<(int, int)>();
        for (int i = 0; i < lines.Length; i++)
        {
            bool found = false;
            var line = lines[i];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    locations.Add((x, i));
                    found = true;
                }
            }

            if (!found)
            {
                yCountsDouble.Add(i);
            }
        }

        for (int x = 0; x < lines[0].Length; x++)
        {
            bool found = false;
                for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line[x] == '#')
                {
                    found = true;
                }
            }

            if (!found)
            {
                XCountsDouble.Add(x);
            }
        }

        for (int first = 0; first < locations.Count; first++)
        {
            var firstLocation = locations[first];
                for (int second = first + 1; second < locations.Count; second++)
            {
                var secondLocation = locations[second];
                solution += Math.Abs(firstLocation.Item1 - secondLocation.Item1);
                solution += Math.Abs(secondLocation.Item2 - firstLocation.Item2);
                for (int y = Math.Min(firstLocation.Item2, secondLocation.Item2); y < Math.Max(firstLocation.Item2, secondLocation.Item2); y++)
                {
                    if (yCountsDouble.Contains(y))
                    {
                        solution+= 1000000-1;
                    }
                }
                for (int x = Math.Min(firstLocation.Item1, secondLocation.Item1); x < Math.Max(firstLocation.Item1, secondLocation.Item1); x++)
                {
                    if (XCountsDouble.Contains(x))
                    {
                        solution+= 1000000 -1;
                    }
                }
            }
        }



        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;


        DateTime end = DateTime.Now;

        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

