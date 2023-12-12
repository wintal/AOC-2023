
using System;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Utilities;

class Day11
{
    static int Main()
    {
        DoPart1("sample.txt");
        DoPart1("input.txt");

        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
    }

    class Question
    {
        public int Start;
        public int End;
    }


    class InputPattern
    {
        public string Pattern;
        public List<int> Required;
        public List<Question> QuestionPart = new List<Question>();
    }
    static bool CheckString(char[] pattern, InputPattern inputPattern)
    {
        var inHash = false;
        int startIndex = 0;
        int patternIndex = 0;

        for (int j = 0; j < pattern.Length; j++)
        {
            if (pattern[j] == '#')
            {
                if (!inHash)
                {
                    inHash = true;
                    startIndex = j;
                }
            }
            else
            {
                if (inHash)
                {
                    inHash = false;
                    if (inputPattern.Required[patternIndex] != (j - startIndex))
                    {
                        return false;
                    }
                    patternIndex++;
                }
            }
        }

        if (inHash)
        {
            inHash = false;
            if (inputPattern.Required[patternIndex] != (pattern.Length - startIndex))
            {
                return false;
            }
            patternIndex++;
        }
        return patternIndex == inputPattern.Required.Count;
    }

    static bool CheckString2( InputPattern inputPattern, long checkPattern)
    {
        var inHash = false;
        int startIndex = 0;
        int patternIndex = 0;
    
        for (int j = 0; j < inputPattern.Pattern.Length; j++)
        {
            char thisChar = inputPattern.Pattern[j];
            if (thisChar == '?')
            {
                thisChar = ((checkPattern & 1) == 0 ) ? '.' : '#';
                checkPattern >>= 1;
            }
            if (thisChar == '#' )
            {
                if (!inHash)
                {
                    inHash = true;
                    startIndex = j;
                }
            }
            else
            {
                if (inHash)
                {
                    inHash = false;
                    if (inputPattern.Required[patternIndex] != (j - startIndex))
                    {
                        return false;
                    }
                    patternIndex++;
                }
            }
        }

        if (inHash)
        {
            inHash = false;
            if (inputPattern.Required[patternIndex] != (inputPattern.Pattern.Length - startIndex))
            {
                return false;
            }
            patternIndex++;
        }
        return patternIndex == inputPattern.Required.Count;
    }

    static int CountBits(int n)
    {
        int count = 0;
        while (n != 0)
        {
            count +=  (n & 1) > 0?1:0;
            n >>= 1;
        }

        return count;
    }

    static List<long> solutions = new List<long>();
    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<InputPattern> patterns = new List<InputPattern>();

        for (int i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].MySplit(" ");
            InputPattern pattern = new InputPattern();
            pattern.Pattern = parts[0];
            pattern.Required = parts[1].MySplit(",").Select(x => int.Parse(x)).ToList();
            var inQuestion = false;
            int startIndex = 0;
            
            for (int j = 0; j < pattern.Pattern.Length; j++)
            {
                if (pattern.Pattern[j] == '?')
                {
                    if (!inQuestion)
                    {
                        inQuestion = true;
                        startIndex = j;
                    }
                }
                else
                {
                    if (inQuestion)
                    {
                        inQuestion = false;
                        pattern.QuestionPart.Add(new Question() { Start = startIndex, End = j -1 });
                    }
                }
            }
            if (inQuestion)
            {
                pattern.QuestionPart.Add(new Question() { Start = startIndex, End = pattern.Pattern.Length - 1 });
            }
            patterns.Add(pattern);

        }


        foreach (var pattern in patterns)
        {
            solution = 0;
            int requiredHashes = pattern.Required.Sum();
            int availableHashes = pattern.QuestionPart.Select(qp => qp.End - qp.Start + 1).Sum();
            int includedHashes = pattern.Pattern.Select(qp => qp == '#'? 1:0).Sum();
            int toAdd = requiredHashes - includedHashes;

            bool done = false;
            char[] pt = pattern.Pattern.ToCharArray();
            int checkPattern = 0;
            while (!done)
            {
                if (CountBits(checkPattern) != toAdd)
                {
                    checkPattern++;
                    if (checkPattern >= 1 << availableHashes)
                    {
                        break;
                    }
                    continue;
                }


                int thisPattern = checkPattern;
                foreach (var qp in pattern.QuestionPart)
                {
                    for (int index = qp.Start; index <= qp.End; index++)
                    {
                        if ((thisPattern & 1) == 0)
                        {
                            pt[index] = '.';
                        }
                        else
                        {
                            pt[index] = '#';
                        }

                        thisPattern = thisPattern >> 1;
                    }
                }

                if (CheckString2(pattern, checkPattern))
                {
                    solution++;
                }

                checkPattern++;
                if (checkPattern >= 1 << availableHashes)
                {
                    break;
                }
            }

            solutions.Add(solution);
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solutions.Sum()}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

   static long snoob(long x)
    {
        long smallest, ripple, ones;
        // x = xxx0 1111 0000
        smallest = x & -x;           //     0000 0001 0000
        ripple = x + smallest;       //     xxx1 0000 0000
        ones = x ^ ripple;           //     0001 1111 0000
        ones = (ones >> 2) / smallest; //     0000 0000 0111
        return ripple | ones;        //     xxx1 0000 0111
    }


   static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        List<InputPattern> patterns = new List<InputPattern>();

        for (int i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].MySplit(" ");
            InputPattern pattern = new InputPattern();
            pattern.Pattern = parts[0];
            pattern.Required = parts[1].MySplit(",").Select(x => int.Parse(x)).ToList();

            pattern.Required.AddRange(parts[1].MySplit(",").Select(x => int.Parse(x)));
            pattern.Required.AddRange(parts[1].MySplit(",").Select(x => int.Parse(x)));
            pattern.Required.AddRange(parts[1].MySplit(",").Select(x => int.Parse(x)));
            pattern.Required.AddRange(parts[1].MySplit(",").Select(x => int.Parse(x)));
            pattern.Pattern = pattern.Pattern + "?" + pattern.Pattern + "?" + pattern.Pattern + "?" + pattern.Pattern +
                              "?" + pattern.Pattern + "?";
                              var inQuestion = false;
            int startIndex = 0;

            for (int j = 0; j < pattern.Pattern.Length; j++)
            {
                if (pattern.Pattern[j] == '?')
                {
                    if (!inQuestion)
                    {
                        inQuestion = true;
                        startIndex = j;
                    }
                }
                else
                {
                    if (inQuestion)
                    {
                        inQuestion = false;
                        pattern.QuestionPart.Add(new Question() { Start = startIndex, End = j - 1 });
                    }
                }
            }
            if (inQuestion)
            {
                pattern.QuestionPart.Add(new Question() { Start = startIndex, End = pattern.Pattern.Length - 1 });
            }
            patterns.Add(pattern);

        }


        Parallel.ForEach(patterns, pattern =>
        {
            int requiredHashes = pattern.Required.Sum();
            long availableHashes = pattern.QuestionPart.Select(qp => qp.End - qp.Start + 1).Sum();
            long includedHashes = pattern.Pattern.Select(qp => qp == '#' ? 1 : 0).Sum();
            long toAdd = requiredHashes - includedHashes;

            Console.WriteLine($"We have a search space of {toAdd} bits over {availableHashes} bits");
            bool done = false;
            char[] pt = pattern.Pattern.ToCharArray();
            long checkPattern = 0;
            for (int i = 0; i < toAdd; i++)
            {
                checkPattern <<= 1;
                checkPattern += 1;
            }

            while (!done)
            {




                if (CheckString2(pattern, checkPattern))
                {
                    System.Threading.Interlocked.Increment(ref solution);
                }

                checkPattern = snoob(checkPattern);
                if (checkPattern >= 1L << (int)availableHashes)
                {
                    break;
                }
            }

            Console.WriteLine($"Completed one");

        });
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

