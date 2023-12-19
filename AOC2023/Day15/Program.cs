using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Utilities;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

class Day15
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

    static int GetHash(string str)
    {
        int hash = 0;
        foreach (var thing in Encoding.ASCII.GetBytes(str))
        {
            hash += thing;
            hash = hash * 17;
            hash = hash % 256;
        }

        return hash;
    }


    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.MySplit(",");
            for (int j = 0; j < parts.Length; j++)
            {
                solution += GetHash(parts[j]);
            }
        }


        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

    class Box
    {
        public List<string> BoxContents= new List<string>();

        public void DoMinus(string pattern)
        {
            BoxContents.RemoveAll( str => str.StartsWith(pattern));
        }
        public void DoEquals(string pattern)
        {
            string searchPattern = pattern.Substring(0, pattern.IndexOf(" "));
            int index = BoxContents.FindIndex(str => str.StartsWith(searchPattern));
            if (index >= 0)
            {
                BoxContents[index] = pattern;
            }
            else
            {
                BoxContents.Add(pattern);
            }

        }
    }
    static void DoPart2(string inputFile)
    {
        Box[] boxes = new Box[256];
        for (int i = 0; i < 256; i++)
        {
            boxes[i] = new Box();
        }
        int partNum = 2;
        DateTime start = DateTime.Now;
        Console.WriteLine($"Part {partNum} ({inputFile})- {1}");
     
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.MySplit(",");
            for (int j = 0; j < parts.Length; j++)
            {
                var part = parts[j];
                var partLocation = parts[j].Split("=").First();
                partLocation = partLocation.Split("-").First();
                int boxIndex = GetHash(partLocation);
                if (part.Last() == '-')
                {
                    boxes[boxIndex].DoMinus(part.Replace("-", ""));
                }
                else if (part.Contains("="))
                {
                    boxes[boxIndex].DoEquals(part.Replace("=", " "));
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        int powerMag = 1;
        foreach (var box in boxes)
        {
            int slot = 1;
            foreach (var lens in box.BoxContents)
            {
                int focalLength = int.Parse(lens.Split(" ").Last());
                solution += powerMag * slot * focalLength;

                slot++;
            }
            
            powerMag++;
        }


        //99875
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {partNum} ({inputFile})- {solution}");
        Console.WriteLine($"Completed part {partNum} in {end - start}");
    }
    
}
