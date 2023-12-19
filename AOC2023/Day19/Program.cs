using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks.Sources;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Utilities;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

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

    enum Direction
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
        None
    }


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
                    newValues[newValueCount++] = new Value()
                        { x = value.x, y = value.y - 1, dir = Direction.Up, num = value.num + 1 };
                }

                newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                newValues[newValueCount++] = new Value()
                    { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                break;
            case Direction.Down:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value()
                        { x = value.x, y = value.y + 1, dir = Direction.Down, num = value.num + 1 };
                }

                newValues[newValueCount++] = new Value()
                    { x = value.x - 1, y = value.y, dir = Direction.Left, num = 1 };
                newValues[newValueCount++] = new Value()
                    { x = value.x + 1, y = value.y, dir = Direction.Right, num = 1 };
                break;
            case Direction.Left:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value()
                        { x = value.x - 1, y = value.y, dir = Direction.Left, num = value.num + 1 };
                }

                newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
                break;
            case Direction.Right:
                if (value.num < 3)
                {
                    newValues[newValueCount++] = new Value()
                        { x = value.x + 1, y = value.y, dir = Direction.Right, num = value.num + 1 };
                }

                newValues[newValueCount++] = new Value() { x = value.x, y = value.y - 1, dir = Direction.Up, num = 1 };
                newValues[newValueCount++] = new Value()
                    { x = value.x, y = value.y + 1, dir = Direction.Down, num = 1 };
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
                    newValues[newValueCount++] = new Value()
                        { x = value.x, y = value.y - 1, dir = Direction.Up, num = value.num + 1 };
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
                    newValues[newValueCount++] = new Value()
                        { x = value.x, y = value.y + 1, dir = Direction.Down, num = value.num + 1 };
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
                    newValues[newValueCount++] = new Value()
                        { x = value.x + 1, y = value.y, dir = Direction.Right, num = value.num + 1 };
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

            int newValueCount =
                part == 1 ? GetNewValuesPart1(current, newValues) : GetNewValuesPart2(current, newValues);
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

    static bool TraceCrucible(Map map, int x, int y, Dictionary<int, long> visited, LastDirections directions,
        out long cost)
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

    class Instruction
    {
        public Direction dir;
        public int distance;
        public string colour;

    }

    static Direction GetDirection(string str)
    {
        switch (str)
        {
            case "U":
                return Direction.Up;
                break;
            case "L":
                return Direction.Left;
                break;
            case "R":
                return Direction.Right;
                break;
            case "D":
                return Direction.Down;
                break;
            default:
                throw new Exception();
                break;
        }
    }

    static Direction GetDirection2(char str)
    {
        switch (str)
        {
            case '3':
                return Direction.Up;
                break;
            case '2':
                return Direction.Left;
                break;
            case '0':
                return Direction.Right;
                break;
            case '1':
                return Direction.Down;
                break;
            default:
                throw new Exception();
                break;
        }
    }

    enum Part
    {
        X,
        M,
        A,
        S
    }
    static Part PartFromString(string s)
    {
        switch (s)
        {
            case "x":
                return Part.X;
            case "m":
                return Part.M;
            case "a":
                return Part.A;
            case "S":
                return Part.S;
            default:
                throw new Exception();
        }
    }

    enum Op
    {
        Lt,
        Gt,
        Eq,
        Accept,
        Reject

    }

    class Rule
    {
        public Part part;
        public Op op;
        public int number;
    }

    class RuleSet
    {
        public String label;
        public List<Rule> rules = new List<Rule>();

    }

    class XmasValue
    {
        public int X;
        public int M;
        public int A;
        public int S;

    }
    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map map = new Map();
        Dictionary<string, Rule> instructions = new Dictionary<string, Rule>();
        List<XmasValue> values = new List<XmasValue>();
        bool inXmas = false;
        for (int i = 0; i < lines.Length; i++)
        {
            
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                inXmas = true;
                continue;
            }
            if (!inXmas)
            {
                var parts = line.MySplit("{");
                var rules = new RuleSet();
                rules.label = parts[0];
                var ruleStrings = parts[1].Substring(0, parts[1].Length-1).MySplit(":");
                foreach (var ruleString in ruleStrings)
                {
                    if (ruleString == "A")
                    {
                        rules.rules.Add(new Rule() { op = Op.Accept });
                    }
                    else if (ruleString == "R")
                    {
                        rules.rules.Add(new Rule(){ op = Op.Reject});
                    }
                    else if (ruleString.Contains("<"))
                    {
                        var ruleParts = ruleString.Split('<');
                        rules.rules.Add(new Rule() { op = Op.Lt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]) });
                    }
                    else if (ruleString.Contains(">"))
                    {
                        var ruleParts = ruleString.Split('<');
                        rules.rules.Add(new Rule() { op = Op.Gt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]) });
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

            }
            else
            {
                var parts = line.Substring(1,line.Length - 1).Split(",");
                XmasValue value = new XmasValue();
                foreach (var partStr in parts)
                { 
                    var kvp = partStr.Split('=');
                    switch (kvp[0])
                    {
                        case "x":
                            value.X = int.Parse(kvp[1]); break;
                        case "m":
                            value.M = int.Parse(kvp[1]); break;
                        case "a":
                            value.A = int.Parse(kvp[1]); break;
                        case "s":
                            value.S = int.Parse(kvp[1]); break;
                        default:
                            throw new Exception();
                    }
                }
            }
        }

   

        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    class LineSegment
    {
        public int FromX;
        public int ToX;
        public int FromY;
        public int ToY;
        public bool horizontal = false;
    }
    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Map map = new Map();
        List<Instruction> instructions = new List<Instruction>();
        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        int minX = 0;
        int maxX = 0;
        int minY = 0;
        int maxY = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var instruction = line.MySplit(" ");
            int distance = Convert.ToInt32(instruction[2].Substring(2, 5), 16);
            instructions.Add(new Instruction() { dir = GetDirection2(instruction[2][7]), distance = distance, colour = instruction[2] });
            //instructions.Add(new Instruction() { dir = GetDirection(instruction[0]), distance = int.Parse(instruction[1]), colour = instruction[2] });
        }

        int x = 0;
        int y = 0;

        visited.Add((x, y));
        List<LineSegment> segments = new List<LineSegment>();
        foreach (var instruction in instructions)
        {
            LineSegment segment = new LineSegment();
            segment.FromX = x;
            segment.FromY = y;

            switch (instruction.dir)
            {
                case Direction.Up:
                    y += instruction.distance;
                    break;
                case Direction.Down:
                    y -= instruction.distance;
                    break;
                case Direction.Left:
                    segment.horizontal = true;
                    x -= instruction.distance;
                    break;
                case Direction.Right:
                    segment.horizontal = true;
                    x += instruction.distance;
                    break;
                default:
                    throw new Exception();
                    break;
            }

            segment.ToX = x;
            segment.ToY = y;
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
            segments.Add(segment);
        }

        Parallel.For(minY, maxY + 1, (y) =>
        {
            List<LineSegment> intersections = segments.Where(segment => (segment.FromY <= y && segment.ToY >= y ||
                                                                         segment.ToY <= y && segment.FromY >= y)
                                                                        && !segment.horizontal).ToList();

            intersections.Sort((a, b) => a.FromX.CompareTo(b.FromX));

            bool previousInSolid = false;
            bool inSolid = false;
            int currentX = minX;
            LineSegment lastSegment = null;
            bool fromAbove = false;
            bool fromBelow = false;
            bool onHorizontal = false;
            bool horizontalStartedInSolid = false;
            foreach (var segment in intersections)
            {
                bool wasInSolid = inSolid;
                if (lastSegment != null && lastSegment.ToX > segment.FromX)
                {
                    throw new Exception();
                }

                var segmentMin = Math.Min(segment.FromY, segment.ToY);
                var segmentMax = Math.Max(segment.FromY, segment.ToY);
                bool thisFromBelow = false;
                bool thisFromAbove = false;
                if (y > segmentMin)
                {
                    thisFromBelow = true;
                }

                if (y < segmentMax)
                {
                    thisFromAbove = true;
                }

                if (!inSolid)
                {
                    inSolid = true;
                    fromBelow = thisFromBelow;
                    fromAbove = thisFromAbove;
                    if (thisFromBelow != thisFromAbove)
                    {
                        onHorizontal = true;
                        horizontalStartedInSolid = false;
                    }
                }
                else
                {


                    if (onHorizontal)
                    {
                        if (fromBelow == thisFromBelow || fromAbove == thisFromAbove)
                        {
                            inSolid = horizontalStartedInSolid;
                            onHorizontal = false;
                        }
                        else
                        {
                            onHorizontal = false;
                            inSolid = !horizontalStartedInSolid;
                        }
                    }
                    else
                    {
                        if (thisFromBelow && thisFromAbove)
                        {
                            fromBelow = false;
                            fromAbove = false;
                            inSolid = !inSolid;
                            onHorizontal = false;
                        }
                        else
                        {
                            horizontalStartedInSolid = true;
                            onHorizontal = true;
                            fromBelow = thisFromBelow;
                            fromAbove = thisFromAbove;
                        }
                    }


                }



                if (wasInSolid && !inSolid)
                {
                    Interlocked.Add(ref solution, segment.FromX - currentX + 1);
                    currentX = segment.FromX;
                }

                if (inSolid && !wasInSolid)
                {
                    currentX = segment.FromX;
                }
            }
        });


        DateTime end = DateTime.Now;
        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
