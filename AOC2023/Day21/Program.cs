using System.ComponentModel.DataAnnotations;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utilities;

class Day21
{
    static int Main()
    {
      //  DoPart1("sample.txt");
     //   DoPart1("input.txt");

   //     DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }

    class Map
    {
        public List<int[]> Elements = new List<int[]>();
        public List<char[]> ElementsChar = new List<char[]>();

        public void Print()
        {
            foreach (var elem in ElementsChar)
            {
                Console.WriteLine(new string(elem));
            }

            Console.WriteLine();
        }
        public void Print(HashSet<(int,int)> locations)
        {
            int y = 0;
            foreach (var elem in ElementsChar)
            {
                var line = new string(elem).ToCharArray();
                foreach (var location in locations.Where(a => a.Item2 == y))
                {
                    if (location.Item1 >= 0 && location.Item1 < ElementsChar[0].Length)
                    {
                        line[location.Item1] = 'O';
                    }
                }
                Console.WriteLine(new string(line));
                y++;
            }

            Console.WriteLine();
        }

        public char GetAt(int x, int y)
        {
            while (x < 0)
            {
                x += ElementsChar[0].Length;
            }
            x = x % ElementsChar[0].Length;
            while (y < 0)
            {
                y += ElementsChar.Count;
            }
            y = y % ElementsChar.Count;
            return ElementsChar[y][x];
        }
        public string GetStringHash()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var element in ElementsChar)
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
 
        Map map = new Map();
        int starty = 0;
        int startx = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.ElementsChar.Add(line.ToCharArray());
            if (line.Contains("S"))
            {
                starty = i;
                startx = line.IndexOf('S');
            }
        }

        map.Print();

        HashSet<(int x, int y)> locations = new HashSet<(int x, int y)>();
        locations.Add((startx, starty));

        for (int i = 0; i < 64; i++)
        {
            HashSet<(int x, int y)> newLocations = new HashSet<(int x, int y)>();
            foreach (var location in locations)
            {
                if (location.x - 1 >= 0)
                {
                    if (map.ElementsChar[location.y][location.x -1] != '#')
                    {
                        newLocations.Add((location.x - 1, location.y));
                    }
                }

                if (location.x + 1 < map.ElementsChar[0].Length)
                {
                    if (map.ElementsChar[location.y][location.x + 1] != '#')
                    {
                        newLocations.Add((location.x + 1, location.y));
                    }
                }

                if (location.y - 1 >= 0)
                {
                    if (map.ElementsChar[location.y -1 ][location.x] != '#')
                    {
                        newLocations.Add((location.x, location.y -1));
                    }
                }

                if (location.y + 1 < map.ElementsChar.Count)
                {
                    if (map.ElementsChar[location.y+ 1][location.x] != '#')
                    {
                        newLocations.Add((location.x, location.y+ 1));
                    }
                }
            }

            locations = newLocations;
            Console.WriteLine();
            map.Print(locations);
        }


        DateTime end = DateTime.Now;
        solution = locations.Count();
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        Map map = new Map();
        int starty = 0;
        int startx = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.ElementsChar.Add(line.ToCharArray());
            if (line.Contains("S"))
            {
                starty = i;
                startx = line.IndexOf('S');
            }
        }

        map.Print();

        // map is 131 characters wide. The S has a clear run to the edge, so he'll get to the new spot in 131 steps 

        //HashSet<(int x, int y)> locations = new HashSet<(int x, int y)>();
        //int[,] counts = new int[map.ElementsChar[0].Length, map.ElementsChar.Count];

        //locations.Add((startx, starty));
        ////This is used to dump stuff to excel for inspection
        //for (int i = 0; i < 1000; i++)
        //    {
        //        HashSet<(int x, int y)> newLocations = new HashSet<(int x, int y)>();
        //        foreach (var location in locations)
        //        {
        //            // if (location.x - 1 >= 0)
        //            {
        //                if (map.GetAt(location.x - 1, location.y) != '#')
        //                {
        //                    newLocations.Add((location.x - 1, location.y));
        //                }
        //            }

        //            //   if (location.x + 1 < map.ElementsChar[0].Length)
        //            {
        //                if (map.GetAt(location.x + 1, location.y) != '#')
        //                {
        //                    newLocations.Add((location.x + 1, location.y));
        //                }
        //            }

        //            //  if (location.y - 1 >= 0)
        //            {
        //                if (map.GetAt(location.x, location.y - 1) != '#')
        //                {
        //                    newLocations.Add((location.x, location.y - 1));
        //                }
        //            }

        //            // if (location.y + 1 < map.ElementsChar.Count)
        //            {
        //                if (map.GetAt(location.x, location.y + 1) != '#')
        //                {
        //                    newLocations.Add((location.x, location.y + 1));
        //                }
        //            }
        //        }

        //        locations = newLocations;
        //        Console.WriteLine($"{locations.Count}");

        //        //   map.Print(locations);
        //    }

        // From excel:
        // n    |  positions
        // 65   |  3778
        // 196  | 33695
        // 327  | 93438
        // 458  |183007
        // first delta is 29917
        // each other delta goes up by 29826

        int desiredStepCount = 26501365;
        int currentStep = 65;
        long currentValue = 3778;
        long increment = 29917;
        long incrementStep = 29826;
        while (currentStep < desiredStepCount)
        {
            currentStep += 131;
            currentValue = currentValue + increment;
            increment += incrementStep;
            Console.WriteLine($"{currentStep} : {currentValue}");
        }

        DateTime end = DateTime.Now;
        solution = currentValue;
        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
