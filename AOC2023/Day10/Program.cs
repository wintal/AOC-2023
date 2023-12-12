
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32.SafeHandles;
using Utilities;

class Day9
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

    enum PipeType
    {
        Vertical,
        Horizontal,
        NorthEastElbow,
        NorthWestElbow,
        SouthEastElbow,
        SouthWestElbow,
        NoPipe,
        StartingPosition,
        Visited
    };

    private static Dictionary<char, PipeType> m_ipeLookup = new Dictionary<char, PipeType>()
    {
        { '|', PipeType.Vertical },
        { '-', PipeType.Horizontal },
        { 'L', PipeType.NorthEastElbow },
        { 'J', PipeType.NorthWestElbow },
        { '7', PipeType.SouthWestElbow },
        { 'F', PipeType.SouthEastElbow },
        { '.', PipeType.NoPipe },
        { 'S', PipeType.StartingPosition },
    };

    enum Direction
    {
        North = 1, 
        South = 2,
        East = 4,
        West = 8,
    }

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        List<List<PipeType>> map = new List<List<PipeType>>();
        int startY = 0;
        int startX = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var values = line.Select(a => m_ipeLookup[a]).ToList();
            if (line.Contains("S"))
            {
                startY = i;
                startX = line.IndexOf("S");
            }
            map.Add(values);
        }

        
        var above = startY > 0? map[startY - 1][startX] : PipeType.NoPipe;
        var below = map[startY + 1][startX];
        var left = map[startY][startX - 1];
        var right = map[startY][startX + 1];

        var aboveJoins = above == PipeType.Vertical || above == PipeType.SouthEastElbow || above == PipeType.SouthWestElbow;
        var belowJoins = below == PipeType.Vertical || below == PipeType.NorthEastElbow || below == PipeType.NorthWestElbow;
        var leftJoins = left == PipeType.Horizontal || left == PipeType.SouthEastElbow || left == PipeType.NorthEastElbow;
        var rightJoin = right == PipeType.Horizontal || right == PipeType.NorthEastElbow || right == PipeType.NorthWestElbow;
        var direction = Direction.North;
        if (aboveJoins)
        {
            direction = Direction.North;

        } else if (belowJoins)
        {
            direction = Direction.South;

        } else if (leftJoins)
        {
            direction = Direction.West;
        }
        else
        {
            throw new Exception();
        }
        
        DateTime end = DateTime.Now;
        int currentX = startX;
        int currentY = startY;
        var currentNode = PipeType.StartingPosition;
        int count = 0;
        do
        {
            switch (direction)
            {
                case Direction.North:
                    currentY--;
                    
                    break;
                case Direction.South:
                    currentY++;
                    break;
                case Direction.West:
                    currentX--;
                    break;
                case Direction.East:
                    currentX++;
                    break;
                default:
                    break;
            }
            currentNode = map[currentY][currentX];
            switch (currentNode)
            {
                case PipeType.NorthEastElbow:
                    if (direction == Direction.South)
                    {
                        direction = Direction.East;
                    }
                    else
                    {
                        direction = Direction.North;
                    }
                    break;
                case PipeType.NorthWestElbow:
                    if (direction == Direction.South)
                    {
                        direction = Direction.West;
                    }
                    else
                    {
                        direction = Direction.North;
                    }
                    break;
                case PipeType.SouthEastElbow:
                    if (direction == Direction.North)
                    {
                        direction = Direction.East;
                    }
                    else
                    {
                        direction = Direction.South;
                    }
                    break;
                case PipeType.SouthWestElbow:
                    if (direction == Direction.North)
                    {
                        direction = Direction.West;
                    }
                    else
                    {
                        direction = Direction.South;
                    }
                    break;
                    break;
                case PipeType.Horizontal:
                    break;
                case PipeType.Vertical:
                    break;
                    default:
                        break;

            }

            count++;
        } while (currentNode != PipeType.StartingPosition);

        solution = count / 2 + count % 1;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;

        List<List<PipeType>> map = new List<List<PipeType>>();
        int startY = 0;
        int startX = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var values = line.Select(a => m_ipeLookup[a]).ToList();
            if (line.Contains("S"))
            {
                startY = i;
                startX = line.IndexOf("S");
            }
            map.Add(values);
        }

        var above = startY > 0 ? map[startY - 1][startX] : PipeType.NoPipe;
        var below = map[startY + 1][startX];
        var left = map[startY][startX - 1];
        var right = map[startY][startX + 1];

        var aboveJoins = above == PipeType.Vertical || above == PipeType.SouthEastElbow || above == PipeType.SouthWestElbow;
        var belowJoins = below == PipeType.Vertical || below == PipeType.NorthEastElbow || below == PipeType.NorthWestElbow;
        var leftJoins = left == PipeType.Horizontal || left == PipeType.SouthEastElbow || left == PipeType.NorthEastElbow;
        var rightJoin = right == PipeType.Horizontal || right == PipeType.NorthEastElbow || right == PipeType.NorthWestElbow;
        
        var direction = Direction.North;
        PipeType startPosition = PipeType.NorthEastElbow;
        if (aboveJoins)
        {
            direction = Direction.North;
            if (belowJoins)
            {
                startPosition = PipeType.Vertical;
            } else if (leftJoins)
            {
                startPosition = PipeType.NorthWestElbow;
            }
            else if (rightJoin)
            {
                startPosition = PipeType.NorthWestElbow;
            }
        }
        else if (belowJoins)
        {
            direction = Direction.South;
            
            if (leftJoins)
            {
                startPosition = PipeType.SouthWestElbow;
            }
            else if (rightJoin)
            {
                startPosition = PipeType.SouthEastElbow;
            }
        }
        else if (leftJoins)
        {
            direction = Direction.West; 
            if (rightJoin)
            {
                startPosition = PipeType.Horizontal;
            }
        }
        else
        {
            throw new Exception();
        }

        HashSet<(int,int)> visitedNodes = new HashSet<(int,int)> ();
        DateTime end = DateTime.Now;
        int currentX = startX;
        int currentY = startY;
        var currentNode = PipeType.StartingPosition;
        visitedNodes.Add((currentX, currentY));
        int count = 0;
        do
        {
            switch (direction)
            {
                case Direction.North:
                    currentY--;

                    break;
                case Direction.South:
                    currentY++;
                    break;
                case Direction.West:
                    currentX--;
                    break;
                case Direction.East:
                    currentX++;
                    break;
                default:
                    break;
            }
            visitedNodes.Add((currentX, currentY));
            currentNode = map[currentY][currentX];
            switch (currentNode)
            {
                case PipeType.NorthEastElbow:
                    if (direction == Direction.South)
                    {
                        direction = Direction.East;
                    }
                    else
                    {
                        direction = Direction.North;
                    }
                    break;
                case PipeType.NorthWestElbow:
                    if (direction == Direction.South)
                    {
                        direction = Direction.West;
                    }
                    else
                    {
                        direction = Direction.North;
                    }
                    break;
                case PipeType.SouthEastElbow:
                    if (direction == Direction.North)
                    {
                        direction = Direction.East;
                    }
                    else
                    {
                        direction = Direction.South;
                    }
                    break;
                case PipeType.SouthWestElbow:
                    if (direction == Direction.North)
                    {
                        direction = Direction.West;
                    }
                    else
                    {
                        direction = Direction.South;
                    }
                    break;
                    break;
                case PipeType.Horizontal:
                    break;
                case PipeType.Vertical:
                    break;
                default:
                    break;

            }

            count++;
        } while (currentNode != PipeType.StartingPosition);


        solution = 0;


        for (int y = 0; y < map.Count; y++)
        {
            int pipesVisited = 0;

            PipeType lastIntercept = PipeType.NoPipe;
            var mapLine = map[y];
            for (int x = 0; x < mapLine.Count; x++)
            {
                var thisNode = mapLine[x];
                if (thisNode == PipeType.StartingPosition )
                {
                    thisNode = startPosition;
                }
                if (visitedNodes.Contains((x,y)))
                {
                    if (thisNode != PipeType.Horizontal)
                    {
                        switch (lastIntercept)
                        {
                            case PipeType.Vertical:
                                pipesVisited++;
                                break;
                            case PipeType.NorthEastElbow:
                                switch (thisNode)
                                {
                                    case PipeType.SouthWestElbow:
                                        // no increment
                                        break;
                                    default:
                                        pipesVisited++;
                                        break;
                                }
                                
                                break;
                            case PipeType.SouthEastElbow:
                                switch (thisNode)
                                {
                                    case PipeType.NorthWestElbow:
                                        // no increment
                                        break;
                                    default:
                                        pipesVisited++;
                                        break;
                                }

                                break;



                            default:
                                pipesVisited++;
                                break;
                        }
                        lastIntercept = thisNode;
                    }

                    ;

                }
                else if (!visitedNodes.Contains((x, y)))
                { 
                    if (pipesVisited % 2 == 1)
                    {
                        solution++;
                    }
                }
            }
        }
        
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


}

