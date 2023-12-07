// See https://aka.ms/new-console-template for more information

using System;
using System.Runtime.CompilerServices;

DoPart1("sample.txt");
DoPart1("input.txt");
DoPart2("sample.txt");
DoPart2("input.txt");

void DoPart1(string inputFile)
{
    DateTime start = DateTime.Now;
    var lines = System.IO.File.ReadAllLines(inputFile);
    long part1Answer = 0;

    int[] times = lines[0].Split(":")[1].MySplit(" ").Select(s => int.Parse(s)).ToArray();
    int[] distances = lines[1].Split(":")[1].MySplit(" ").Select(s => int.Parse(s)).ToArray();

    int[] counts = new int[times.Length];
    for (int race = 0; race < times.Length; race++)
    {
        counts[race] = 0;
        for (int i = 1; i < times[race]; i++)
        {
            var distance = times[race] * i - i * i;
            if (distance > distances[race])
            {
                counts[race]++;
            }
        }
    }

    long solution = counts[0];
    for (int race = 1; race < times.Length; race++)
    {
        solution *= counts[race];
    }
    
    DateTime end = DateTime.Now;
    Console.WriteLine($"Part 1 ({inputFile})- {solution}");

    Console.WriteLine($"Completed part 1 in {end - start}");
}



void DoPart2(string inputFile)
{
    DateTime start = DateTime.Now;
    var lines = System.IO.File.ReadAllLines(inputFile);
    long part1Answer = 0;

    int count = 0;
    int time = 48938595;
    long distance = 296192812361391;

    for (long i = 1; i < time; i++)
    {
        var thisDistance = (long)time * i - i * i;
        if (thisDistance > distance)
        {
            count++;
        }

    }

    long solution = count;


    DateTime end = DateTime.Now;
    Console.WriteLine($"Part 2 ({inputFile})- {solution}");

    Console.WriteLine($"Completed part 1 in {end - start}");
}

static class MyExtensions
{
    public static string[] MySplit(this string input, string splitVal)
    {
        return input.Split(splitVal, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}