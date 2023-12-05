// See https://aka.ms/new-console-template for more information

using System;

var lines = System.IO.File.ReadAllLines("input.txt");

int part1Answer = 0;
for (int lineNum = 0; lineNum < lines.Length; lineNum++)
{
    var line = lines[lineNum];
}
Console.WriteLine($"Part 1 answer = {part1Answer}");

int part2Answer = 0;
List<int> pendingCards = new List<int>() { 0 };
for (int lineNum = 0; lineNum < lines.Length; lineNum++)
{

    var line = lines[lineNum];
}
Console.WriteLine($"Part 2 answer = {part2Answer}");