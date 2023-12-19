
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Utilities;

class Day12
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

    static long ConsumeDot( ImmutableStack<int> required, string inputPattern, Dictionary<(string, ImmutableStack<int>), long> solutionCache)
    {
        return GetSolutions( required, inputPattern[1..], solutionCache);
    }
    static long ConsumeHash( ImmutableStack<int> required, string inputPattern, Dictionary<(string, ImmutableStack<int>), long> solutionCache)
    {
        if (!required.Any())
        {
            return 0; // no more numbers left, this is no good
        }
        // need to have currentCount hashes followed by the end of the string or .
        int currentCount = required.Peek();
        required = required.Pop();

        var numberHashesAvailable = inputPattern.TakeWhile(s => s == '#' || s == '?').Count();

        if (numberHashesAvailable < currentCount)
        {
            return 0; // not enough dead springs 
        }
        else if (inputPattern.Length == currentCount)
        {
            return GetSolutions(required, "",  solutionCache);
        }
        else if (inputPattern[currentCount] == '#')
        {
            return 0; // dead spring follows the range -> not good
        }
        else
        {
            return GetSolutions(required, inputPattern[(currentCount + 1)..],  solutionCache);
        }

    }

    static long ConsumeQuestion(ImmutableStack<int> required, string inputPattern, Dictionary<(string, ImmutableStack<int>), long> solutionCache)
    {

        return GetSolutions( required, "." + inputPattern[1..], solutionCache) + GetSolutions(required, "#" + inputPattern[1..], solutionCache);
    }

    static long DoGetSolution(ImmutableStack<int> required, string inputPattern, Dictionary<(string, ImmutableStack<int>), long> solutionCache)
    {

        if (!inputPattern.Any())
        {
            if (required.Any())
            {
                return 0;
            }

            return 1;

        }
        int thisChar = inputPattern[0];

        switch (thisChar)
        {
            case '.':
                return ConsumeDot(required, inputPattern, solutionCache);
                break;
            case '#':
                return ConsumeHash(required, inputPattern, solutionCache);
                break;
            case '?':
            default:
                return ConsumeQuestion(required, inputPattern, solutionCache);
                break;
        }
    }

    static long GetSolutions(ImmutableStack<int> required, string inputPattern, Dictionary<(string,ImmutableStack<int>), long> solutionCache)
    {
        
        if (solutionCache.ContainsKey((inputPattern, required)))
        {
            return solutionCache[(inputPattern, required)];
        }
        else
        {
            var value = DoGetSolution(required, inputPattern, solutionCache);
            solutionCache[(inputPattern, required)] = value;
            return value;
        }
    }

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
            var solutionCache = new Dictionary<(string, ImmutableStack<int>), long>();
            pattern.Required.Reverse();
            long solutions = GetSolutions(ImmutableStack.CreateRange<int>(pattern.Required), pattern.Pattern, solutionCache);
            solution += solutions;
        }
        
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }
    
    

    static long GetNextIntWithSameBits(long x)
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
                              "?" + pattern.Pattern;
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


        foreach (var pattern in patterns)
        {
            var solutionCache = new Dictionary<(string, ImmutableStack<int>), long>();
            pattern.Required.Reverse();
            long solutions = GetSolutions( ImmutableStack.CreateRange(pattern.Required), pattern.Pattern, solutionCache);
            solution += solutions;
        }
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

