using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utilities;

class Day21
{
    static int Main()
    {
   //     DoPart1("sample.txt");
        DoPart1("input.txt");

        DoPart2("sample.txt");
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
        public void Print(HashSet<(int, int)> locations)
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

    private class Brick
    {
        public int StartX;
        public int StartY;
        public int StartZ;
        public int EndX;
        public int EndY;
        public int EndZ;

        public int XWidth => EndX - StartX + 1;
        public int YWidth => EndY - StartY + 1;
        public int ZWidth => EndZ - StartZ + 1;

        public int Id;

        public int Zlocation = 0;

        public HashSet<int> Supporters = new HashSet<int>();
        public HashSet<int> Supporting = new HashSet<int>();

    }
    

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Brick> bricks = new List<Brick>();
        Dictionary<int, Brick> brickById = new Dictionary<int, Brick>();
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            var vectors = line.MySplit("~");
            var startPt = vectors[0].MySplit(",");
            var endPt= vectors[1].MySplit(",");
            var brick = new Brick();
            brick.StartX = int.Parse(startPt[0]);
            brick.StartY = int.Parse(startPt[1]);
            brick.StartZ = int.Parse(startPt[2]);
            brick.EndX = int.Parse(endPt[0]);
            brick.EndY = int.Parse(endPt[1]);
            brick.EndZ = int.Parse(endPt[2]);
            maxX = Math.Max(maxX, brick.StartX);
            maxX = Math.Max(maxX, brick.EndX);
            maxY = Math.Max(maxY, brick.StartY);
            maxY = Math.Max(maxY, brick.EndY);
            maxZ = Math.Max(maxZ, brick.StartZ);
            maxZ = Math.Max(maxZ, brick.EndZ);
            brick.Id = i+1;
            bricks.Add(brick);
            brickById[brick.Id] = brick;
            //Debug.Assert(brick.StartX < brick.EndX);
            //Debug.Assert(brick.StartY < brick.EndY);
            //Debug.Assert(brick.StartZ < brick.EndZ);
        }
        bricks.Sort((a,b) => a.StartZ.CompareTo(b.StartZ));


        int[,,] locations = new int[maxX +1, maxY +1, maxZ+30];

        foreach (var brick in bricks)
        {
            for (int z = maxZ; z >= 0; z--)
            {
                bool allClear = true;
                for (int x = brick.StartX; x <= brick.EndX; x++)
                {
                    for (int y = brick.StartY; y <= brick.EndY; y++)
                    {
                        for (int zExtent = z; zExtent < z + brick.ZWidth; zExtent ++)
                        {
                            if (locations[x, y, zExtent] != 0)
                            {
                                allClear = false;
                            }
                        }
                    }
                }

                if (!allClear || z == 0)
                {
                    brick.Zlocation = z+1;
                    for (int x = brick.StartX; x <= brick.EndX; x++)
                    {
                        for (int y = brick.StartY; y <= brick.EndY; y++)
                        {
                            for (int zExtent = brick.Zlocation; zExtent < brick.Zlocation + brick.ZWidth; zExtent++)
                            {
                                locations[x, y, zExtent] = brick.Id;
                            }
                        }
                    }

                    break;
                }

            }
        }
        
        foreach (var brick in bricks)
        {
            bool canBeDestroyed = true;
            for (int x = brick.StartX; x <= brick.EndX; x++)
            {
                for (int y = brick.StartY; y <= brick.EndY; y++)
                {

                    if (locations[x, y, brick.Zlocation + brick.ZWidth] != 0)
                    {
                        brickById[locations[x, y, brick.Zlocation + brick.ZWidth]].Supporters.Add(brick.Id);
                        brick.Supporting.Add(locations[x, y, brick.Zlocation + brick.ZWidth]);
                        canBeDestroyed = false;
                    }
                }
            }
        }

        foreach (var brick in bricks)
        {
            bool canDestroy = true;
            foreach (var supporting in brick.Supporting)
            {
                if (brickById[supporting].Supporters.Count == 1)
                {
                    canDestroy = false;
                }
            }

            if (canDestroy)
            {
                solution++;
            }
        }


        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

    private  static int GetSupportingCount(Brick brick, Dictionary<int,Brick> brickById, HashSet<int> falling)
    {
        int count = 0;


        HashSet<Brick> toFall = new HashSet<Brick>();
        foreach (var supporting in brick.Supporting)
        {
            var brickSup = brickById[supporting];

            bool canFall = !brickSup.Supporters.Any(a => !falling.Contains(a));
            if (canFall)
            {
                count++;
                toFall.Add(brickSup);
                falling.Add(supporting);
                
            }
            
        }

        foreach (var childBrick in toFall)
        {
            count += GetSupportingCount(childBrick, brickById, falling);
        }

        return count;
    }
    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<Brick> bricks = new List<Brick>();
        Dictionary<int, Brick> brickById = new Dictionary<int, Brick>();
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            var vectors = line.MySplit("~");
            var startPt = vectors[0].MySplit(",");
            var endPt = vectors[1].MySplit(",");
            var brick = new Brick();
            brick.StartX = int.Parse(startPt[0]);
            brick.StartY = int.Parse(startPt[1]);
            brick.StartZ = int.Parse(startPt[2]);
            brick.EndX = int.Parse(endPt[0]);
            brick.EndY = int.Parse(endPt[1]);
            brick.EndZ = int.Parse(endPt[2]);
            maxX = Math.Max(maxX, brick.StartX);
            maxX = Math.Max(maxX, brick.EndX);
            maxY = Math.Max(maxY, brick.StartY);
            maxY = Math.Max(maxY, brick.EndY);
            maxZ = Math.Max(maxZ, brick.StartZ);
            maxZ = Math.Max(maxZ, brick.EndZ);
            brick.Id = i + 1;
            bricks.Add(brick);
            brickById[brick.Id] = brick;
            //Debug.Assert(brick.StartX < brick.EndX);
            //Debug.Assert(brick.StartY < brick.EndY);
            //Debug.Assert(brick.StartZ < brick.EndZ);
        }
        bricks.Sort((a, b) => a.StartZ.CompareTo(b.StartZ));


        int[,,] locations = new int[maxX + 1, maxY + 1, maxZ + 30];

        foreach (var brick in bricks)
        {
            for (int z = maxZ; z >= 0; z--)
            {
                bool allClear = true;
                for (int x = brick.StartX; x <= brick.EndX; x++)
                {
                    for (int y = brick.StartY; y <= brick.EndY; y++)
                    {
                        for (int zExtent = z; zExtent < z + brick.ZWidth; zExtent++)
                        {
                            if (locations[x, y, zExtent] != 0)
                            {
                                allClear = false;
                            }
                        }
                    }
                }

                if (!allClear || z == 0)
                {
                    brick.Zlocation = z + 1;
                    for (int x = brick.StartX; x <= brick.EndX; x++)
                    {
                        for (int y = brick.StartY; y <= brick.EndY; y++)
                        {
                            for (int zExtent = brick.Zlocation; zExtent < brick.Zlocation + brick.ZWidth; zExtent++)
                            {
                                locations[x, y, zExtent] = brick.Id;
                            }
                        }
                    }

                    break;
                }

            }
        }

        foreach (var brick in bricks)
        {
            bool canBeDestroyed = true;
            for (int x = brick.StartX; x <= brick.EndX; x++)
            {
                for (int y = brick.StartY; y <= brick.EndY; y++)
                {

                    if (locations[x, y, brick.Zlocation + brick.ZWidth] != 0)
                    {
                        brickById[locations[x, y, brick.Zlocation + brick.ZWidth]].Supporters.Add(brick.Id);
                        brick.Supporting.Add(locations[x, y, brick.Zlocation + brick.ZWidth]);
                        canBeDestroyed = false;
                    }
                }
            }
        }
        

        foreach (var brick in bricks)
        {
            HashSet<int> falling = new HashSet<int>();
            ;
            falling.Add(brick.Id);
            GetSupportingCount(brick, brickById, falling);
            solution += falling.Count - 1;
        }


        DateTime end = DateTime.Now;
        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
