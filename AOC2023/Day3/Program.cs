// See https://aka.ms/new-console-template for more information

using System;

var lines = System.IO.File.ReadAllLines("input.txt");
bool IsSymbol(char input)
{
  
    return (input == '#' || input == '$' || input == '%' || input == '&' 
            || input == '*' || input == '+' || input == '-' || input == '/'
            || input == '=' 
            || input == '@');
}
int part1Answer = 0;
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    var previousLine = i > 0 ? lines[i - 1] : null;
    var nextLine = i < (lines.Length -1) ? lines[i + 1] : null;
    bool inNumber = false;
    int currentNumber = 0;
    bool foundSymbol = false;
    for (int character = 0; character < line.Length; character++)
    {
        if (Char.IsDigit(line[character]))
        {
            if (inNumber)
            {
                currentNumber = currentNumber * 10 + line[character] - 48;
            }
            else
            {
                inNumber = true;
                currentNumber = line[character] - 48;
            }
            for (int x = Math.Max(character - 1, 0); x <= Math.Min(character + 1, line.Length - 1); x++)
            {
                for (int y = Math.Max(i - 1, 0); y <= Math.Min(i + 1, lines.Length -1 ); y++)
                {
                    var symbolChar = lines[y][x];
                    if (!Char.IsDigit(symbolChar) && symbolChar != '.')
                    {
                        
                        foundSymbol = true;
                    }
                }
            }

        }
        else
        {
            if (inNumber && foundSymbol)
            {
                part1Answer += currentNumber;
            }
            inNumber = false;
            currentNumber = 0;
            foundSymbol = false;
        }
    }
    if (inNumber && foundSymbol)
    {
        part1Answer += currentNumber;
    }
    inNumber = false;
    currentNumber = 0;
    foundSymbol = false;
}

Console.WriteLine($"Answer to part 1 is {part1Answer}");

int part2Answer = 0;

Dictionary<int, List<int>> gears = new Dictionary<int, List<int>>();
for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    var previousLine = i > 0 ? lines[i - 1] : null;
    var nextLine = i < (lines.Length - 1) ? lines[i + 1] : null;
    bool inNumber = false;
    int currentNumber = 0;
    bool foundGear = false;
    HashSet<int> gearsLocal = new HashSet<int>();
    for (int character = 0; character < line.Length; character++)
    {
        if (Char.IsDigit(line[character]))
        {
            if (inNumber)
            {
                currentNumber = currentNumber * 10 + line[character] - 48;
            }
            else
            {
                inNumber = true;
                currentNumber = line[character] - 48;
            }
            for (int x = Math.Max(character - 1, 0); x <= Math.Min(character + 1, line.Length - 1); x++)
            {
                for (int y = Math.Max(i - 1, 0); y <= Math.Min(i + 1, lines.Length - 1); y++)
                {
                    var symbolChar = lines[y][x];
                    if (symbolChar == '*')
                    {
                        int index = x + 1000 * y;
                        gearsLocal.Add(index);
                    }
                }
            }
        }
        else
        {
            if (inNumber && gearsLocal.Any())
            {
                foreach (var index in gearsLocal)
                {
                    if (!gears.ContainsKey(index))
                    {
                        gears[index] = new List<int>();
                    }

                    gears[index].Add(currentNumber);
                }
            }
            inNumber = false;
            currentNumber = 0;
            gearsLocal.Clear();
        }
    }
    if (inNumber && gearsLocal.Any())
    {
        foreach (var index in gearsLocal)
        {
            if (!gears.ContainsKey(index))
            {
                gears[index] = new List<int>();
            }

            gears[index].Add(currentNumber);
        }
    }
    inNumber = false;
    currentNumber = 0;
}

foreach (var kvp in gears)
{
    if (kvp.Value.Count == 2)
    {
        part2Answer += kvp.Value[0] * kvp.Value[1];
    }
}

Console.WriteLine($"Answer to part 2 is {part2Answer}");
