using System.Text;

class Day16
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

        public void Clear()
        {
            foreach (var elem in Elements)
            {
                for(int x = 0; x < elem.Length; x++)
                {
                    elem[x] = ' ';
                }
            }
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

    enum Direction { Left, Right, Up, Down }


    static (int x, int y) MoveDirection(int x, int y, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                x--;
                break;
            case Direction.Right:
                x++;
                break;
            case Direction.Up:
                y--;
                break;
            case Direction.Down:
                y++;
                break;

        }

        return (x, y);
    }
    static void TraceBeam(Map map, Map visited, int x, int y, Direction direction, HashSet<(int,int,Direction)> done)
    {
        while (true)
        {
            if (done.Contains((x, y, direction))) return;
            done.Add((x, y, direction));
            if (y < 0 || y >= map.Elements.Count || x < 0 || x >= map.Elements[0].Length)
            {
                return;
            }

            visited.Elements[y][x] = '#';
         //   map.Print();
       //     visited.Print();
            switch (map.Elements[y][x])
            {
                case '.':
                    (x, y) = MoveDirection(x, y, direction);
                    break;
                case '/':
                    switch (direction)
                    {
                        case Direction.Left:
                            direction = Direction.Down;
                            break;
                        case Direction.Right:
                            direction = Direction.Up;
                            break;
                        case Direction.Up:
                            direction = Direction.Right;
                            break;
                        case Direction.Down:
                            direction = Direction.Left;
                            break;

                    }
                    (x, y) = MoveDirection(x, y, direction);
                    break;
                case '\\':
                    switch (direction)
                    {
                        case Direction.Left:
                            direction = Direction.Up;
                            break;
                        case Direction.Right:
                            direction = Direction.Down;
                            break;
                        case Direction.Up:
                            direction = Direction.Left;
                            break;
                        case Direction.Down:
                            direction = Direction.Right;
                            break;
                    }
                    (x, y) = MoveDirection(x, y, direction);
                    break;
                case '|':
                    switch (direction)
                    {
                        case Direction.Left:
                        case Direction.Right:
                            // first
                            var (firstx, firsty) = MoveDirection(x, y, Direction.Up);
                            TraceBeam(map, visited, firstx, firsty, Direction.Up, done);
                            // second
                            var (secondX, secondY) = MoveDirection(x, y, Direction.Down);
                            TraceBeam(map, visited, secondX, secondY, Direction.Down, done);
                            return;
                            return;
                            break;
                        default:
                            break;
                    }

                    (x, y) = MoveDirection(x, y, direction);
                    break;
                case '-':
                    switch (direction)
                    {
                        case Direction.Up:
                        case Direction.Down:
                            // first
                            var (firstx, firsty) = MoveDirection(x, y, Direction.Left);
                            TraceBeam(map, visited, firstx, firsty, Direction.Left, done);
                            // second
                            var (secondX, secondY) = MoveDirection(x, y, Direction.Right);
                            TraceBeam(map, visited, secondX, secondY, Direction.Right, done);
                            return;
                            break;
                        default:
                            break;
                    }

                    (x, y) = MoveDirection(x, y, direction);
                    break;
                default:
                    throw new Exception();

            }
        }

    }

    static long GetScore(Map visited)
    {
        long solution = 0;
        foreach (var line in visited.Elements)
        {
            foreach (var character in line)
            {
                if (character == '#')
                {
                    solution++;
                }
            }
        }

        return solution;
    }

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map map = new Map();
        Map visited = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.Elements.Add(line.ToCharArray());
            visited.Elements.Add(line.ToCharArray());
        }

        TraceBeam(map, visited, 0, 0, Direction.Right, new HashSet<(int, int, Direction)>());
        solution = GetScore(visited);
     
        
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

 
    static void DoPart2(string inputFile)
    {
        int part = 2;
        Console.WriteLine($"Part {part} ({inputFile})- {1}");

        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map map = new Map();
        Map visited = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.Elements.Add(line.ToCharArray());
            visited.Elements.Add(line.ToCharArray());
        }
        
        for (int y = 0; y < lines.Length; y++)
        {
            TraceBeam(map, visited, 0, y, Direction.Right, new HashSet<(int, int, Direction)>());
            var thisSolution = GetScore(visited);
            solution = Math.Max(solution, thisSolution);
            visited.Clear();

            TraceBeam(map, visited, lines[0].Length-1 , y, Direction.Left, new HashSet<(int, int, Direction)>());
            thisSolution = GetScore(visited);
            solution = Math.Max(solution, thisSolution);
            visited.Clear();
        }
        for (int x = 0; x < lines[0].Length; x++)
        {
            TraceBeam(map, visited, x, 0, Direction.Down, new HashSet<(int, int, Direction)>());
            var thisSolution = GetScore(visited);
            solution = Math.Max(solution, thisSolution);
            visited.Clear();

            TraceBeam(map, visited, x, lines.Length -1, Direction.Up, new HashSet<(int, int, Direction)>());
            thisSolution = GetScore(visited);
            solution = Math.Max(solution, thisSolution);
            visited.Clear();
        }

        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
        
    }

}
