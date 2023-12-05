// See https://aka.ms/new-console-template for more information
DateTime start = DateTime.Now;

var lines = System.IO.File.ReadAllLines("input.txt");
long total = 0;
foreach (var line in lines)
{
    total += (line.First(a => Char.IsNumber(a))  - 48) * 10;
    total += line.Last(a => Char.IsNumber(a)) - 48;
}
string[] numbers = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
(int, int) FirstNumberFound(string line)
{
    int returnValue = -1;
    int foundIndex = int.MaxValue;
    for (int i = 0; i < numbers.Length;i++)
    {
        int index = line.IndexOf(numbers[i]);
        if (index >= 0 && index <= foundIndex)
        {
            foundIndex = index;
            returnValue = i + 1;
        }
    }
    return (foundIndex, returnValue);
}

(int, int) LastNumberFound(string line)
{
    int returnValue = -1;
    int foundIndex = -1;
    for (int i = 0; i < numbers.Length; i++)
    {
        int index = line.LastIndexOf(numbers[i]);
        if (index >= 0 && index >= foundIndex)
        {
            foundIndex = index;
            returnValue = i + 1;
        }
    }
    return (foundIndex, returnValue);
}



DateTime end = DateTime.Now;
Console.WriteLine($"result is {total}");
Console.WriteLine($"Completed in {(end - start).TotalMilliseconds} milliseconds");
start = DateTime.Now;
total = 0;
foreach (var line in lines)
{
    char firstNumberChar = line.First(a => Char.IsNumber(a));
    int firstNumberIndex = line.IndexOf(firstNumberChar);
    (int firstWordIndex, int firstWordNumber) = FirstNumberFound(line);
    int firstNumber = firstWordIndex < firstNumberIndex ? firstWordNumber : firstNumberChar - 48;

    char secondNumberChar = line.Last(a => Char.IsNumber(a));
    int secondNumberIndex = line.LastIndexOf(secondNumberChar);
    (int secondWordIndex, int secondWordNumber) = LastNumberFound(line);
    int secondNumber = secondWordIndex > secondNumberIndex ? secondWordNumber : secondNumberChar - 48;
    total += firstNumber * 10;
    total += secondNumber;
}

end = DateTime.Now;
Console.WriteLine($"result is {total}");
Console.WriteLine($"Completed in {(end - start).TotalMilliseconds} milliseconds");
Console.WriteLine("Press any key to close the window.");
Console.ReadKey();