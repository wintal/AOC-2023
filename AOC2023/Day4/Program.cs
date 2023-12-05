// See https://aka.ms/new-console-template for more information

using System;

var lines = System.IO.File.ReadAllLines("input.txt");

int part1Answer = 0;
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    var parts = line.Split(":")[1].Trim().Split('|');
    var firstBits = parts[0].Split(" ").Where(str => !string.IsNullOrEmpty(str)).Select(val => int.Parse(val)).ToArray();
    var secondBits = parts[1].Split(" ").Where(str => !string.IsNullOrEmpty(str)).Select(val => int.Parse(val)).ToArray();
    var matches = secondBits.Where(val => firstBits.Contains(val)).Count();
    part1Answer += (int)Math.Pow(2, matches - 1);
}
Console.WriteLine($"Part 1 answer = {part1Answer}");



int part2Answer = 0;
List<int> pendingCards = new List<int>() {0};
for (int i = 0; i < lines.Length; i++)
{
    int multiplier = 1;
    for (int j = 0; j < pendingCards.Count; j++)
    {
        if (pendingCards[j] > 0)
        {
            multiplier++;
            pendingCards[j]--;
        }
    }

    if (multiplier == 0) break;

    part2Answer += multiplier;
    var line = lines[i];
    var parts = line.Split(":")[1].Trim().Split('|');
    var firstBits = parts[0].Split(" ").Where(str => !string.IsNullOrEmpty(str)).Select(val => int.Parse(val)).ToArray();
    var secondBits = parts[1].Split(" ").Where(str => !string.IsNullOrEmpty(str)).Select(val => int.Parse(val)).ToArray();
    var matches = secondBits.Where(val => firstBits.Contains(val)).Count();

   
    if (matches > 0)
    {
        for (int j = 0; j < multiplier; j++)
        {
            pendingCards.Add(matches);
        }
    }
}
 Console.WriteLine($"Part 1 answer = {part2Answer}");
