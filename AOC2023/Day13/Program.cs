
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Utilities;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

class Day13
{
    static int Main()
    {
        DoPart1("sample.txt");
        DoPart1("input.txt");

        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }

    class Map
    {
        public List<char[]> Elements = new List<char[]>();
        public List<int> HorizontalElements = new List<int>();
        public List<int> VerticalElements = new List<int>();

        public void CalculateElements()
        {
            foreach (var element in Elements)
            {
                int value = 0;
                for (int x = element.Length - 1; x >= 0; x--)
                {
                    value <<= 1;
                    if (element[x] == '#')
                    {
                        value |= 1;
                    }
                }
                HorizontalElements.Add(value);
            }

            for (int x = 0; x < Elements[0].Length; x++)
            {
                int value = 0;
                for (int y = Elements.Count - 1; y >= 0; y--)
                {
                    value <<= 1;
                    if (Elements[y][x] == '#')
                    {
                        value |= 1;
                    }
                }
                VerticalElements.Add(value);
            }
        }
    }


    private static int GetHorizontalPlane(Map map)
    {
        for (int x = 1; x < map.Elements[0].Length; x++)
        {
            bool allMatch = true;
            for (int y = 0; y < map.Elements.Count; y++)
            {
                int maxOffset = Math.Min(x, map.Elements[0].Length - x);
                for (int offset = 0; offset < maxOffset; offset++)
                {
                    var leftElem = map.Elements[y][x - offset - 1];
                    var rightElem = map.Elements[y][x + offset];
                    if (leftElem != rightElem)
                    {
                        allMatch = false;
                    }
                }

                if (!allMatch)
                {
                    break;
                }
            }

            if (allMatch)
            {
                // found a solution;
                return x;
            }
        }

        return -1;
    }

    private static int GetHorizontalPlaneWithSmudge(Map map, Range xRange, Range yRange)
    {
        for (int x = 1; x < map.Elements[0].Length; x++)
        {
            int misMatchCount = 0;
            for (int y = 0; y < map.Elements.Count; y++)
            {
                int maxOffset = Math.Min(x, map.Elements[0].Length - x);
                for (int offset = 0; offset < maxOffset; offset++)
                {
                    var leftElem = map.Elements[y][x - offset - 1];
                    var rightElem = map.Elements[y][x + offset];
                    if (leftElem != rightElem)
                    {
                        misMatchCount++;
                        //if (yRange.Contains(y) && (xRange.Contains(x - offset - 1) || xRange.Contains(x + offset)))
                        //{
                        //    misMatchCount++;
                        //}
                        //else
                        //{
                        //     Requires more than one smudge.
                        //     dirty way to bail out early
                        //    misMatchCount += 2; 
                        //}
                    }
                }

                if (misMatchCount > 1)
                {
                    break;
                }
            }

            if (misMatchCount == 1)
            {
                // found a solution;
                return x;
            }
        }

        return -1;
    }

    private static int GetVerticalPlaneWithSmudge(Map map, Range xRange, Range yRange)
    {
        for (int y = 1; y < map.Elements.Count; y++)
        {
            int misMatchCount = 0;
            for (int x = 0; x < map.Elements[0].Length; x++)
            {
                int maxOffset = Math.Min(y, map.Elements.Count - y);
                for (int offset = 0; offset < maxOffset; offset++)
                {
                    var leftElem = map.Elements[y - offset - 1][x];
                    var rightElem = map.Elements[y + offset][x];
                    if (leftElem != rightElem)
                    {
                        misMatchCount++;
                        //if (yRange.Contains(y) && (xRange.Contains(x - offset - 1) || xRange.Contains(x + offset)))
                        //{
                        //    misMatchCount++;
                        //}
                        //else
                        //{
                        //     Requires more than one smudge.
                        //     dirty way to bail out early
                        //    misMatchCount += 2;
                        //}
                    }
                }

                if (misMatchCount > 1)
                {
                    break;
                }
            }

            if (misMatchCount == 1)
            {
                // found a solution;
                return y;
            }
        }

        return -1;
    }

    private static int GetVerticalPlane(Map map)
    {
        for (int y = 1; y < map.Elements.Count; y++)
        {
            bool allMatch = true;
            for (int x = 0; x < map.Elements[0].Length; x++)
            {
                int maxOffset = Math.Min(y, map.Elements.Count - y);
                for (int offset = 0; offset < maxOffset; offset++)
                {
                    var leftElem = map.Elements[y - offset - 1][x];
                    var rightElem = map.Elements[y + offset][x];
                    if (leftElem != rightElem)
                    {
                        allMatch = false;
                    }
                }

                if (!allMatch)
                {
                    break;
                }
            }

            if (allMatch)
            {
                // found a solution;
                return y;
            }
        }

        return -1;
    }

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Map> maps = new List<Map>();
        Map currentMap = null;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                if (currentMap != null)
                {
                    maps.Add(currentMap);
                    currentMap = null;
                }
                continue;
            }

            if (currentMap == null)
            {
                currentMap = new Map();
            }

            currentMap.Elements.Add(line.ToCharArray());

        }
        if (currentMap != null)
        {
            maps.Add(currentMap);
        }

        foreach (var map in maps)
        {
            map.CalculateElements();
            bool foundSymmetry = false;
            // look for horizontal reflection

            var horizontalPlane = GetHorizontalPlane(map);

            if (horizontalPlane >= 0)
            {
                solution += horizontalPlane;
                foundSymmetry = true;
            }
            else
            {
                var verticalPlane = GetVerticalPlane(map);
                if (verticalPlane >= 0)
                {
                    solution += verticalPlane * 100;

                    foundSymmetry = true;
                }
            }

            if (!foundSymmetry)
            {
                throw new Exception();
            }
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    private class Range
    {
        public int From;
        public int To;

        public bool Contains(int val)
        {
            return val >= From && val <= To;
        }
    }

    static void DoPart2(string inputFile)
    {
        int part = 2;

        DateTime start = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {1}");
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Map> maps = new List<Map>();
        Map currentMap = null;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                if (currentMap != null)
                {
                    maps.Add(currentMap);
                    currentMap = null;
                }
                continue;
            }

            if (currentMap == null)
            {
                currentMap = new Map();
            }

            currentMap.Elements.Add(line.ToCharArray());

        }
        if (currentMap != null)
        {
            maps.Add(currentMap);
        }

        foreach (var map in maps)
        {
            map.CalculateElements();
            bool foundSymmetry = false;
            // look for horizontal reflection

            var xRange = new Range() { From = 0, To = map.Elements[0].Length };
            var yRange = new Range() { From = 0, To = map.Elements.Count };
            var horizontalPlane = GetHorizontalPlane(map);

            if (horizontalPlane >= 0)
            {
                int maxOffset = Math.Min(horizontalPlane, map.Elements[0].Length - horizontalPlane);
                xRange.From = (horizontalPlane - maxOffset);
                xRange.To = (horizontalPlane + maxOffset - 1);
                foundSymmetry = true;
            }
            else
            {
                var verticalPlane = GetVerticalPlane(map);
                if (verticalPlane >= 0)
                {
                    int maxOffset = Math.Min(verticalPlane, map.Elements[0].Length - verticalPlane);
                    yRange.From = (verticalPlane - maxOffset );
                    yRange.To = (verticalPlane + maxOffset -1);

                    foundSymmetry = true;
                }
            }

            if (!foundSymmetry)
            {
                throw new Exception();
            }

            foundSymmetry = false;
            var horizontalPlaneCorrected = GetHorizontalPlaneWithSmudge(map, xRange, yRange);

            if (horizontalPlaneCorrected >= 0)
            {
                solution += horizontalPlaneCorrected;
                foundSymmetry = true;
            }
            else
            {
                var verticalPlaneCorrected = GetVerticalPlaneWithSmudge(map, xRange, yRange);
                if (verticalPlaneCorrected >= 0)
                {
                    solution += verticalPlaneCorrected * 100;

                    foundSymmetry = true;
                }
            }

            if (!foundSymmetry)
            {
                throw new Exception();
            }

        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");
        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

