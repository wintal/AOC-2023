using System.Text;

class Day14
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

        public void Print()
        {
            foreach (var elem in Elements)
            {
                Console.WriteLine(new string(elem));
            }

            Console.WriteLine();
        }

      

        public string GetStringHash()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var element in Elements)
            {
                builder.Append(element);
            }
            return builder.ToString();
        }
    }




    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map currentMap = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            currentMap.Elements.Add(line.ToCharArray());

        }

        for (int y = 1; y < currentMap.Elements.Count; y++)
        {
            for (int x = 0; x < currentMap.Elements[y].Length; x++)
            {
                if (currentMap.Elements[y][x] == 'O')
                {
                    int yCandidate = y - 1;

                    while (yCandidate >= 0 && currentMap.Elements[yCandidate][x] == '.')
                    {
                        currentMap.Elements[yCandidate + 1][x] = '.';
                        currentMap.Elements[yCandidate][x] = 'O';
                        yCandidate--;
                    }
                }
            }
        }

        int weight = currentMap.Elements.Count;
        for (int y = 0; y < currentMap.Elements.Count; y++)
        {
            foreach (var element in currentMap.Elements[y])
            {
                if (element == 'O')
                {
                    solution += weight;
                }
            }

            weight--;
        }

        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

    enum Direction
    {
        North,
        South,
        West,
        East
    }

    private static void RollStones(Map currentMap, Direction dir)
    {

        switch (dir)
        {
            case Direction.North:

                for (int y = 1; y < currentMap.Elements.Count; y++)
                {
                    for (int x = 0; x < currentMap.Elements[y].Length; x++)
                    {
                        if (currentMap.Elements[y][x] == 'O')
                        {
                            int yCandidate = y - 1;

                            while (yCandidate >= 0 && currentMap.Elements[yCandidate][x] == '.')
                            {
                                currentMap.Elements[yCandidate + 1][x] = '.';
                                currentMap.Elements[yCandidate][x] = 'O';
                                yCandidate--;
                            }
                        }
                    }
                }

                break;
            case Direction.South:
                for (int y = currentMap.Elements.Count - 2; y >= 0; y--)
                {
                    for (int x = 0; x < currentMap.Elements[y].Length; x++)
                    {
                        if (currentMap.Elements[y][x] == 'O')
                        {
                            int yCandidate = y + 1;

                            while (yCandidate < currentMap.Elements.Count && currentMap.Elements[yCandidate][x] == '.')
                            {
                                currentMap.Elements[yCandidate - 1][x] = '.';
                                currentMap.Elements[yCandidate][x] = 'O';
                                yCandidate++;
                            }
                        }
                    }
                }

                break;
            case Direction.West:
                for (int x = 1; x < currentMap.Elements[0].Length; x++)
                {
                    for (int y = 0; y < currentMap.Elements.Count; y++)
                    {
                        if (currentMap.Elements[y][x] == 'O')
                        {
                            int xCandidate = x - 1;

                            while (xCandidate >= 0 && currentMap.Elements[y][xCandidate] == '.')
                            {
                                currentMap.Elements[y][xCandidate + 1] = '.';
                                currentMap.Elements[y][xCandidate] = 'O';
                                xCandidate--;
                            }
                        }
                    }
                }

                break;
            case Direction.East:
                for (int x = currentMap.Elements[0].Length - 2; x >= 0; x--)
                {
                    for (int y = 0; y < currentMap.Elements.Count; y++)
                    {
                        if (currentMap.Elements[y][x] == 'O')
                        {
                            int xCandidate = x + 1;

                            while (xCandidate < currentMap.Elements[y].Length &&
                                   currentMap.Elements[y][xCandidate] == '.')
                            {
                                currentMap.Elements[y][xCandidate - 1] = '.';
                                currentMap.Elements[y][xCandidate] = 'O';
                                xCandidate++;
                            }
                        }
                    }
                }

                break;
        }

        //     currentMap.Print();
    }

    private static long CalculateWeight(Map currentMap)
    {
        long solution = 0;
        int weight = currentMap.Elements.Count;
        for (int y = 0; y < currentMap.Elements.Count; y++)
        {
            foreach (var element in currentMap.Elements[y])
            {
                if (element == 'O')
                {
                    solution += weight;
                }
            }

            weight--;
        }

        return solution;
    }

    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {1}");
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map currentMap = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            currentMap.Elements.Add(line.ToCharArray());

        }
        
        List<string> hashes = new List<string>();
        for (int cycle = 0; cycle < 1000000000; cycle++)
        {
            RollStones(currentMap, Direction.North);
            RollStones(currentMap, Direction.West);
            RollStones(currentMap, Direction.South);
            RollStones(currentMap, Direction.East);
           
                var result = CheckForCycle(hashes, currentMap);
                if (result > 0)
                {
                    // found a cycle - need to find the right thing to end on
                    var distanceToEndMod = (1000000000 - cycle -1) % result;

                    for (int i = 0; i <  distanceToEndMod; i++)
                    {
                        RollStones(currentMap, Direction.North);
                        RollStones(currentMap, Direction.West);
                        RollStones(currentMap, Direction.South);
                        RollStones(currentMap, Direction.East);
                    }

                    solution = CalculateWeight(currentMap);
                    break;

                }


        }

        //99875
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");
        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    private static int CheckForCycle(List<string> hashes, Map currentMap)
    {
        var hash = currentMap.GetStringHash();
        if (hashes.Contains(hash))
        {
            return hashes.Count - hashes.IndexOf(hash);
        }

        hashes.Add(hash);
        return -1;
    }
}
