// See https://aka.ms/new-console-template for more information
DateTime start = DateTime.Now;

var lines = System.IO.File.ReadAllLines("input.txt");

int gameSum = 0;
foreach (var line in lines)
{
    var input = line.Split(":");
    var game = input[0];
    int thisGameId = int.Parse(game.Split(" ")[1]);
    var rounds = input[1].Split(";").Select(str => str.Trim());
    bool possible = true;
    foreach (var round in rounds)
    {
        int red = 0;
        int green = 0;
        int blue = 0;
        var items = round.Split(",");
        foreach (var item in items)
        {
            var itemParts = item.Trim().Split(" ");
            var count = Int16.Parse(itemParts[0].Trim());
            var colour = itemParts[1].Trim();
            switch (colour)
            {
                case "red":
                    red += count;
                    break;
                case "green":
                    green += count;
                    break;
                case "blue":
                    blue += count;
                    break;
                default:
                    throw new Exception();
            }

        }
        if (red > 12 || green > 13 || blue > 14)
        {
            possible = false;
        }
    }

    if (possible)
    {

        gameSum += thisGameId;
    }
}

Console.WriteLine($"gameSum = {gameSum}");


int powerSum = 0;
foreach (var line in lines)
{
    var input = line.Split(":");
    var game = input[0];
    int thisGameId = int.Parse(game.Split(" ")[1]);
    var rounds = input[1].Split(";").Select(str => str.Trim());
    bool possible = true;
    int redMax = 0;
    int greenMax = 0;
    int blueMax = 0;
    foreach (var round in rounds)
    {
        var items = round.Split(",");
        foreach (var item in items)
        {
            var itemParts = item.Trim().Split(" ");
            var count = Int16.Parse(itemParts[0].Trim());
            var colour = itemParts[1].Trim();
            switch (colour)
            {
                case "red":
                    redMax = Math.Max(redMax, count);
                    break;
                case "green":
                    greenMax = Math.Max(greenMax, count);
                    break;
                case "blue":
                    blueMax = Math.Max(blueMax, count);
                    break;
                default:
                    throw new Exception();
            }

        }
        
    }

    powerSum += redMax * greenMax * blueMax;
}

Console.WriteLine($"Power sum = {powerSum}");

