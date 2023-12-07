
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

    long solution = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        long[] data = line.Split(":")[1].MySplit(" ").Select(s => long.Parse(s)).ToArray();
    }

    DateTime end = DateTime.Now;
    Console.WriteLine($"Part 1 ({inputFile})- {solution}");

    Console.WriteLine($"Completed part 1 in {end - start}");
}



void DoPart2(string inputFile)
{
    DateTime start = DateTime.Now;
    var lines = System.IO.File.ReadAllLines(inputFile);

    long solution = 0;


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