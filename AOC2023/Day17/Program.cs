using System.Text;

class Day17
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

    enum Direction { Left = 0, Right = 1, Up = 2, Down = 3, None }


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


    struct LastDirections
    {
        public LastDirections(Direction a, Direction b, Direction c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Direction A;
        public Direction B;
        public Direction C;

        public bool AllSame()
        {
            return A == B && B == C;
        }
    }

    static bool Valid(int x, int y, Map map)
    {
        if (x > map.Elements[0].Length - 1 || x < 0 || y < 0 || y > map.Elements.Count - 1)
        {
            return false;
        }

        return true;
    }

    static int GetNewValuesPart1(Value value, Value[] newValues)
    {
        int newValueCount = 0;

        switch (value.dir)
        {
            case Direction.Up:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = value.num + 1 };
                }
                newValues[newValueCount++] = new Value() { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                newValues[newValueCount++] = new Value() { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                break;
            case Direction.Down:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value() { x = value.x, y = value.y + 1, dir = Direction.Down, num = value.num + 1 };
                }
                newValues[newValueCount++] = new Value() { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                newValues[newValueCount++] = new Value() { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                break;
            case Direction.Left:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = value.num + 1 };
                }
                newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                newValues[newValueCount++] = new Value() { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
                break;
            case Direction.Right:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value() { x = value.x + 1, y = value.y, dir = Direction.Right, num = value.num + 1 };
                }
                newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                newValues[newValueCount++] = new Value() { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
                break;
        }


        return newValueCount;
    }
    static int GetNewValuesPart2(Value value, Value[] newValues)
    {
        int newValueCount = 0;

        int maxDir = 10;
        switch (value.dir)
        {
            case Direction.Up:
                if (value.num < maxDir)
                {
                    newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = value.num + 1 };
                }

                if (value.num >= 4)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                    newValues[newValueCount++] = new Value()
                    { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                }

                break;
            case Direction.Down:
                if (value.num < maxDir)
                {
                    newValues[newValueCount++] = new Value() { x = value.x, y = value.y + 1, dir = Direction.Down, num = value.num + 1 };
                }

                if (value.num >= 4)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                    newValues[newValueCount++] = new Value()
                    { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                }

                break;
            case Direction.Left:
                if (value.num < maxDir)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = value.num + 1 };
                }

                if (value.num >= 4)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                    newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
                }

                break;
            case Direction.Right:
                if (value.num < maxDir)
                {
                    newValues[newValueCount++] = new Value() { x = value.x + 1, y = value.y, dir = Direction.Right, num = value.num + 1 };
                }

                if (value.num >= 4)
                {
                    newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                    newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
                }

                break;
        }


        return newValueCount;
    }
    

    struct Value
    {
        public int x;
        public int y;
        public Direction dir;
        public int num;
    }

    class NodeValues
    {
        public NodeValues()
        {
            for (int dir = 0; dir < 4; dir++)
            {
                for (int num = 0; num < 11; num++)
                {
                    Values[dir, num] = int.MaxValue;
                }
            }
        }

        public int[,] Values = new int[4, 11];

    }

        static long TraceCrucible(Map map, int part)
        {
            PriorityQueue<Value, int> todo = new PriorityQueue<Value, int>();

            todo.Enqueue(new Value { x = 0, y = 0, dir = Direction.Right, num = 0 }, 0);
            todo.Enqueue(new Value { x = 0, y = 0, dir = Direction.Down, num = 0 }, 0);


            NodeValues[,] values = new NodeValues[map.Elements[0].Length, map.Elements.Count];
            for (int i = 0; i < map.Elements[0].Length; i++)
            {
                for (int j = 0; j < map.Elements.Count; j++)
                {
                    values[i, j] = new NodeValues();
                }
            }

            values[0, 0].Values[(int)Direction.Right, 0] = 0;
            values[0, 0].Values[(int)Direction.Down, 0] = 0;

            int count = 0;
            Value[] newValues = new Value[4];
            while (todo.TryDequeue(out var current, out var currentCost))
            {
                count++;
                if (count % 1000000 == 0)
                {
                    Console.WriteLine($"Processed {count} nodes, {todo.Count} remaining.");
                }

                if (currentCost > values[current.x, current.y].Values[(int)current.dir, current.num])
                {
                    continue;
                }

                int newValueCount = part == 1 ? GetNewValuesPart1(current, newValues) : GetNewValuesPart2(current, newValues);
                for (int i = 0; i < newValueCount; i++)
                {
                    var newValue = newValues[i];
                    if (Valid(newValue.x, newValue.y, map))
                    {
                        var cost = values[current.x, current.y].Values[(int)current.dir, current.num] +
                                   map.Elements[newValue.y][newValue.x];

                        if (values[newValue.x, newValue.y].Values[(int)newValue.dir, newValue.num] > cost)
                        {
                            values[newValue.x, newValue.y].Values[(int)newValue.dir, newValue.num] = cost;
                            todo.Enqueue(newValue, cost);
                        }
                    }
                }
            }

            long retValue = long.MaxValue;

            for (int dir = 0; dir < 4; dir++)
            {
                int num = part == 1 ? 0 : 4;
                for (; num < 11; num++)
                {
                    retValue = Math.Min(retValue,
                        values[map.Elements[0].Length - 1, map.Elements.Count - 1].Values[dir, num]);
                }
            }
            return retValue;
        }
    static bool TraceCrucible(Map map, int x, int y, Dictionary<int, long> visited, LastDirections directions, out long cost)
    {
        cost = long.MaxValue;
        if (x > map.Elements[0].Length - 1 || x < 0 || y < 0 || y > map.Elements.Count - 1)
        {
            return false;
        }
        if (visited.ContainsKey(x + y * 1000))
        {
            var value = visited[x + y * 1000];
            cost = value;
            return value != long.MaxValue;
        }

        if ((x, y) == (map.Elements[0].Length - 1, map.Elements.Count - 1))
        {
            cost = map.Elements[y][x];

            visited[x + y * 1000] = cost;
            return true;
        }

        visited[x + y * 1000] = long.MaxValue;
        bool anySuccess = false;
        var done3 = directions.AllSame();
        if (!(done3 && directions.C == Direction.Up))
        {
            var success = TraceCrucible(map, x, y - 1, visited,
                new LastDirections(directions.B, directions.C, Direction.Up), out var thisCost);
            if (success)
            {
                anySuccess = true;
                cost = Math.Min(cost, thisCost);
            }
        }
        if (!(done3 && directions.C == Direction.Down))
        {
            var success = TraceCrucible(map, x, y + 1, visited,
                new LastDirections(directions.B, directions.C, Direction.Down), out var thisCost);
            if (success)
            {
                anySuccess = true;
                cost = Math.Min(cost, thisCost);
            }
        }
        if (!(done3 && directions.C == Direction.Left))
        {
            var success = TraceCrucible(map, x - 1, y, visited,
                new LastDirections(directions.B, directions.C, Direction.Left), out var thisCost);
            if (success)
            {
                anySuccess = true;
                cost = Math.Min(cost, thisCost);
            }
        }
        if (!(done3 && directions.C == Direction.Right))
        {
            var success = TraceCrucible(map, x + 1, y, visited,
                new LastDirections(directions.B, directions.C, Direction.Right), out var thisCost);
            if (success)
            {
                anySuccess = true;
                cost = Math.Min(cost, thisCost);
            }
        }

        if (anySuccess)
        {
            cost += map.Elements[y][x];
        }

        visited[x + y * 1000] = cost;
        return anySuccess;


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
            map.ElementsChar.Add(line.ToCharArray());
            map.Elements.Add(line.Select(x => x - 48).ToArray());
        }

        solution = TraceCrucible(map, 1);


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
            map.ElementsChar.Add(line.ToCharArray());
            map.Elements.Add(line.Select(x => x - 48).ToArray());
        }

        solution = TraceCrucible(map, 2);


        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
