
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

class Day8
{
    static int Main()
    {
      //  DoPart1("sample.txt");
        DoPart1("input.txt");
     //   DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }


    class Node
    {
        public string From;
        public String Right;
        public String Left;
    }

    private static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private static long LowestCommonMultiple(long a, long b)
    {
        return a * b / GreatestCommonDivisor(a,b); 
    }

    static void DoPart1(string inputFile)
    {

        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        var directions = lines[0].Trim();
        Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.Split('=');
            if (parts.Length != 2)
                continue;
            var node = new Node();
            node.From = parts[0].Trim();
            node.Left = parts[1].Split(",")[0].Trim().Substring(1).Trim();
            node.Right = parts[1].Split(",")[1].Trim().Substring(0).Trim();
            node.Right = node.Right.Substring(0, node.Right.Length - 1);
            nodes[node.From] = node;
        }
        DateTime end = DateTime.Now;



        string location = "AAA";
        int steps = 0;
        do
        {
            location = directions[steps % directions.Length] == 'L' ? nodes[location].Left : nodes[location].Right;
            steps++;
        } while (!location.EndsWith("ZZZ"));
        Console.WriteLine($"Part 2 ({inputFile})- {steps}");

        Console.WriteLine($"Completed part 2 in {end - start}");
    }


    static void DoPart2(string inputFile)
    {
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        var directions = lines[0].Trim();
        Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.Split('=');
            if (parts.Length != 2)
                continue;
            var node = new Node();
            node.From = parts[0].Trim();
            node.Left = parts[1].Split(",")[0].Trim().Substring(1).Trim();
            node.Right = parts[1].Split(",")[1].Trim().Substring(0).Trim();
            node.Right = node.Right.Substring(0, node.Right.Length - 1);
            nodes[node.From] = node;
        }
        DateTime end = DateTime.Now;


        List<string> locations = nodes.Keys.Where(key => key.EndsWith("A")).ToList();

        int[] cycles = new int[locations.Count];
        for (int i = 0; i < locations.Count; i++)
        {
            string location = locations[i];
            int steps = 0;
            do
            {
                location = directions[steps % directions.Length] == 'L' ? nodes[location].Left : nodes[location].Right;
                steps++;
            } while (!location.EndsWith("Z"));
            cycles[i] = steps;
        }

        long totalSteps = cycles[0];
        for (int i = 1; i < locations.Count; i++)
        {
            totalSteps = LowestCommonMultiple(totalSteps, cycles[i]);
        }
        Console.WriteLine($"Part 2 ({inputFile})- {totalSteps}");

        Console.WriteLine($"Completed part 2 in {end - start}");
    }


}

static class MyExtensions
{
    public static string[] MySplit(this string input, string splitVal)
    {
        return input.Split(splitVal, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}