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

    for (int lineNum = 0; lineNum < lines.Length; lineNum++)
    {

    }

    DateTime end = DateTime.Now;
    Console.WriteLine($"Part 1 - {part1Answer}");

    Console.WriteLine($"Completed part 1 in {end - start}");
}
void DoPart2(string inputFile)
{
    DateTime start = DateTime.Now;
    var lines = System.IO.File.ReadAllLines(inputFile);

    long part2Answer = 0;

    for (int lineNum = 0; lineNum < lines.Length; lineNum++)
    {

    }

    DateTime end = DateTime.Now;
    Console.WriteLine($"Part 2 - {part2Answer}");
    Console.WriteLine($"Completed part 2 in {end - start}");
}