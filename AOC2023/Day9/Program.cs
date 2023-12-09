
using Utilities;

class Day8
{
    static int Main()
    { 
        DoPart1("sample.txt");
        DoPart1("input.txt");
        
        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }


    class Node
    {
        public string From;
        public String Right;
        public String Left;
    }

    private static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private static long LowestCommonMultiple(long a, long b)
    {
        return a * b / GreatestCommonDivisor(a, b);
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
            var values = line.MySplit(" ").Select(a => int.Parse(a)).ToList();
            List<List<int>> allDiffs = new List<List<int>>();
            allDiffs.Add(values);
            bool done = false;
            var currentDiffs = values;
            while (!done)
            {
                var diffs = new List<int>();
                for (int j = 1; j < currentDiffs.Count;j++) {
                    diffs.Add(currentDiffs[j] - currentDiffs[j-1]);
                }

                currentDiffs = diffs;
                allDiffs.Add(diffs);
                if (diffs.All(a=> a==0))
                {
                    done = true;
                }
            }

            int currentIncrement = 0;
            for (int j = allDiffs.Count - 1; j >= 0; j--)
            {
                currentIncrement = allDiffs[j].Last() + currentIncrement;
            }

            solution += currentIncrement;
        }
        DateTime end = DateTime.Now;
        
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var values = line.MySplit(" ").Select(a => int.Parse(a)).ToList();
            List<List<int>> allDiffs = new List<List<int>>();
            allDiffs.Add(values);
            bool done = false;
            var currentDiffs = values;
            while (!done)
            {
                var diffs = new List<int>();
                for (int j = 1; j < currentDiffs.Count; j++)
                {
                    diffs.Add(currentDiffs[j] - currentDiffs[j - 1]);
                }

                currentDiffs = diffs;
                allDiffs.Add(diffs);
                if (diffs.All(a => a == 0))
                {
                    done = true;
                }
            }

            int currentIncrement = 0;
            for (int j = allDiffs.Count - 1; j >= 0; j--)
            {
                currentIncrement = allDiffs[j].First() - currentIncrement;
            }

            solution += currentIncrement;
        }
        DateTime end = DateTime.Now;

        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

