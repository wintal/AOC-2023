
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

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



    static void DoPart1(string inputFile)
    {
       
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        for (int i = 0; i < lines.Length; i++)
        {
          
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part 1 ({inputFile})- {solution}");

        Console.WriteLine($"Completed part 1 in {end - start}");
    }



    static void DoPart2(string inputFile)
    {
     

        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
        }

        DateTime end = DateTime.Now;
        Console.WriteLine($"Part 2 ({inputFile})- {solution}");

        Console.WriteLine($"Completed part 2 in {end - start}");
    }


}