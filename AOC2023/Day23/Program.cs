using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Transactions;
using Microsoft.VisualBasic.CompilerServices;
using Utilities;

class Day21
{
    static int Main()
    {
    //    DoPart1("sample.txt");
   //     DoPart1("input.txt");

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
        public void Print(HashSet<int> locations)
        {
            int y = 0;
            foreach (var elem in Elements)
            {
                var line = new string(elem).ToCharArray();
                foreach (var location in locations.Where(a => a/10000 == y))
                {
                    if ((location % 10000) >= 0 && (location % 10000) < Elements[0].Length)
                    {
                        line[location % 10000] = 'O';
                    }
                }
                Console.WriteLine(new string(line));
                y++;
            }

            Console.WriteLine();
        }

        public char GetAt(int x, int y)
        {
            while (x < 0)
            {
                x += Elements[0].Length;
            }
            x = x % Elements[0].Length;
            while (y < 0)
            {
                y += Elements.Count;
            }
            y = y % Elements.Count;
            return Elements[y][x];
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

    class PositionScore
    {
        public int x;
        public int y;
        public int stepsSoFar = 0;
        public HashSet<int> visited = new HashSet<int>();
    }

    static int GetNewLocations(PositionScore currentLocation, Map map, PositionScore[] outputs)
    {
        int count = 0;
        var currentNode = map.Elements[currentLocation.y][currentLocation.x];
        switch (currentNode)
        {
            case '#':
                throw new Exception();
            case '>':
            {

                int newX = currentLocation.x + 1;
                int newY = currentLocation.y;
                if (!currentLocation.visited.Contains(newX + newY * 10000))
                {
                    outputs[count++] = new PositionScore()
                    {
                        x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1
                    };

                }
            }
                break;
            case '<':
            {

                int newX = currentLocation.x -1;
                int newY = currentLocation.y;
                if (!currentLocation.visited.Contains(newX + newY * 10000))
                {
                    outputs[count++] = new PositionScore()
                    {
                        x = newX,
                        y = newY,
                        stepsSoFar = currentLocation.stepsSoFar + 1
                    };
                    }
            }
                break;
            case '^':
            {

                int newX = currentLocation.x;
                int newY = currentLocation.y - 1;
                if (!currentLocation.visited.Contains(newX + newY * 10000))
                {
                    outputs[count++] = new PositionScore()
                    {
                        x = newX,
                        y = newY,
                        stepsSoFar = currentLocation.stepsSoFar + 1
                    };
                }
            }
                break;
            case 'v':
            {

                int newX = currentLocation.x;
                int newY = currentLocation.y + 1;
                if (!currentLocation.visited.Contains(newX + newY * 10000))
                {
                    outputs[count++] = new PositionScore()
                    {
                        x = newX,
                        y = newY,
                        stepsSoFar = currentLocation.stepsSoFar + 1
                    };
                    }
            }

                break;
            default:
                if (currentLocation.x > 0)
                {
                    int newX = currentLocation.x - 1;
                    int newY = currentLocation.y;
                    if (map.Elements[newY][newX] != '#' &&  !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                            { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.x < map.Elements[0].Length - 1)
                {
                    int newX = currentLocation.x + 1;
                    int newY = currentLocation.y;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                            { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.y > 0)
                {
                    int newX = currentLocation.x;
                    int newY = currentLocation.y - 1;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                            { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.y < map.Elements.Count - 1)
                {
                    int newX = currentLocation.x;
                    int newY = currentLocation.y + 1;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                        { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                break;
        }

        for (int i = 0; i < count; i ++)
        {
            outputs[i].visited = new HashSet<int>(currentLocation.visited);
            outputs[i].visited.Add(outputs[i].x + 10000 * outputs[i].y);

        }
        return count;
    }

  

    static long TracePath(Map map, int part)
    {
        PriorityQueue<PositionScore, int> todo = new PriorityQueue<PositionScore, int>();

        todo.Enqueue(new PositionScore { x = 1, y = 0, stepsSoFar = 0},int.MaxValue );

        int[,] values = new int[map.Elements[0].Length, map.Elements.Count];
        for (int i = 0; i < map.Elements[0].Length; i++)
        {
            for (int j = 0; j < map.Elements.Count; j++)
            {
                values[i, j] = 0;
            }
        }
        
       

        int count = 0;
        PositionScore[] newValues = new PositionScore[4];
        while (todo.TryDequeue(out var current, out var currentCost))
        {
            count++;
            if (count % 1000000 == 0)
            {
                Console.WriteLine($"Processed {count} nodes, {todo.Count} remaining.");
            }

            if (currentCost < values[current.x, current.y])
            {
                continue;
            }

            int newValueCount = GetNewLocations(current, map, newValues);
            for (int i = 0; i < newValueCount; i++)
            {
                var newValue = newValues[i];


                if (values[newValue.x, newValue.y] < newValue.stepsSoFar)
                {
                    values[newValue.x, newValue.y] = newValue.stepsSoFar;
                    todo.Enqueue(newValue, int.MaxValue - newValue.stepsSoFar);
                }
            }
        }

        int endX = map.Elements[0].Length - 2;
        int endY = map.Elements.Count - 1;

        return values[endX, endY];
    }

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        var map = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.Elements.Add(line.ToCharArray());
        }

        int startX = 1;
        int startY = 0;

        int endX = map.Elements[0].Length - 2;
        int endY = map.Elements.Count -1;
        solution = TracePath(map, 1);
        // dijkstra, maximise cost


        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }


    static int GetNewLocations2(PositionScore currentLocation, Map map, PositionScore[] outputs)
    {
        int count = 0;
        var currentNode = map.Elements[currentLocation.y][currentLocation.x];
        switch (currentNode)
        {
            case '#':
                throw new Exception();
            default:
                if (currentLocation.x > 0)
                {
                    int newX = currentLocation.x - 1;
                    int newY = currentLocation.y;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                        { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.x < map.Elements[0].Length - 1)
                {
                    int newX = currentLocation.x + 1;
                    int newY = currentLocation.y;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                        { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.y > 0)
                {
                    int newX = currentLocation.x;
                    int newY = currentLocation.y - 1;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                        { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                if (currentLocation.y < map.Elements.Count - 1)
                {
                    int newX = currentLocation.x;
                    int newY = currentLocation.y + 1;
                    if (map.Elements[newY][newX] != '#' && !currentLocation.visited.Contains(newX + newY * 10000))
                    {
                        outputs[count++] = new PositionScore()
                        { x = newX, y = newY, stepsSoFar = currentLocation.stepsSoFar + 1 };
                    }
                }

                break;
        }

        for (int i = 0; i < count; i++)
        {
            outputs[i].visited = new HashSet<int>(currentLocation.visited);
            outputs[i].visited.Add(outputs[i].x + 10000 * outputs[i].y);

        }
        return count;
    }

    class NodeValues
    {
        public HashSet<int> valuesForNode = new HashSet<int>();
    }

    static long TracePath2(Map map, int part)
    {
        PriorityQueue<PositionScore, int> todo = new PriorityQueue<PositionScore, int>();

        todo.Enqueue(new PositionScore { x = 1, y = 0, stepsSoFar = 0 }, int.MaxValue);

        NodeValues[,] values = new NodeValues[map.Elements[0].Length, map.Elements.Count];
        for (int i = 0; i < map.Elements[0].Length; i++)
        {
            for (int j = 0; j < map.Elements.Count; j++)
            {
                values[i, j] = new NodeValues();
            }
        }



        int count = 0;
        PositionScore[] newValues = new PositionScore[4];
        while (todo.TryDequeue(out var current, out var currentCost))
        {
            count++;
            if (count % 10000 == 0)
            {

                Console.WriteLine($"Processed {count} nodes, {todo.Count} remaining.");

                map.Print(current.visited);
            }

        
            int newValueCount = GetNewLocations2(current, map, newValues);
            for (int i = 0; i < newValueCount; i++)
            {
                var newValue = newValues[i];
                bool skip = false;
                //foreach (var alreadyVisited in values[newValue.x, newValue.y].valuesForNode)
                //{
                //    if (newValue.visited.All(a => !alreadyVisited.Contains(a)))
                //    {
                //        // skip adding this, as a longer path got here 
                //        skip = true;
                //    }
                //}

                //if (!skip)
                //{
                if (newValue.visited.Count > values[newValue.x, newValue.y].valuesForNode.Count)
                {
                    values[newValue.x, newValue.y].valuesForNode = newValue.visited;
                }

                todo.Enqueue(newValue, int.MaxValue - newValue.stepsSoFar);
               // }
            }
        }

        int endX = map.Elements[0].Length - 2;
        int endY = map.Elements.Count - 1;

        return values[endX, endY].valuesForNode.Count;
    }

    class PathNode
    {
        public (int x, int y) Start;
        public (int x, int y) End;
        public int Length;
        public HashSet<PathNode> StartConnections = new HashSet<PathNode>();
        public HashSet<PathNode> EndConnections = new HashSet<PathNode>();

        public List<(int x, int y)> Locations = new List<(int x, int y)>();

        public override string ToString()
        {
            return $"{Start.x}, {Start.y}";
        }
    }




    static (PathNode, List<PathNode>) BuildGraph(Map map)
    {
        List<PathNode> allNodes = new List<PathNode>();

        int startX = 1;
        int startY = 0;

        PathNode root = new PathNode()
        {
            Start = (startX, startY),
        };

        allNodes.Add(root);
        Stack<(PathNode, (int x, int y))> nodesToProcess = new Stack<(PathNode, (int x, int y))>();
        nodesToProcess.Push((root, (1, 1)));

        int endX = map.Elements[0].Length - 2;
        int endY = map.Elements.Count - 1;

        PositionScore[] newValues = new PositionScore[4];

        HashSet<(int, int)> visited = new HashSet<(int,int)>();
        visited.Add((startX, startY));

        while (nodesToProcess.Any())
        {
            var (toProcess, next) = nodesToProcess.Pop();

            bool firstStep = true;

            toProcess.Length++;
            int currentX = next.x;
            int currentY = next.y;
            (int x, int y) last = toProcess.Start;
            while (true)
            {
                toProcess.Locations.Add((currentX, currentY));
                int count = GetNewLocations2(
                    new PositionScore()
                        { x = currentX, y = currentY, visited = new HashSet<int>() { last.x + 10000 * last.y } }, map,
                    newValues);
                toProcess.Length++;
                if (count == 0)
                {
                    // dead end
                    toProcess.End = (currentX, currentY);
                    break;
                }
                else if (count == 1)
                {
                    // straight path, continue
                    last.x = currentX;
                    last.y = currentY;
                    currentX = newValues[0].x;
                    currentY = newValues[0].y;
                }
                else if (count > 1)
                {
                    toProcess.End = (currentX, currentY);
                    if (visited.Contains((currentX, currentY)))
                    {
                        // add the current Node to any existing one
                    } 
                    else
                    {

                        visited.Add((currentX, currentY));
                        // found an intersection, split
                        for (int i = 0; i < count; i++)
                        {
                            var newNode = newValues[i];
                            PathNode newBranch = new PathNode() { Start = (currentX, currentY) };
                            toProcess.EndConnections.Add(newBranch);
                            newBranch.StartConnections.Add(toProcess);
                            nodesToProcess.Push((newBranch, (newNode.x, newNode.y)));

                            allNodes.Add(newBranch);
                        }
                    }

                    break;
                }
            }
        }

        return (root, allNodes);
    }
    static int GetMaxPath((int x, int y) location, PathNode node, HashSet<(int x, int y)> visited, int soFar)
    {
        HashSet<PathNode> connections = null;
        (int x, int y) nextLocation = (0, 0);
        if (location == node.Start)
        {
            connections = node.EndConnections;
            nextLocation = node.End;
        }
        else if (location == node.End)
        {
            connections = node.StartConnections;
            nextLocation = node.Start;
        }

        if (connections.Count == 0)
        {
            return node.Length - 1;
        }
        else
        {
            int max = 0;
            if (!visited.Contains(nextLocation))
            {
                foreach (var childNode in connections)
                {

                    visited.Add(nextLocation);
                    max = Math.Max(max,
                        GetMaxPath(nextLocation, childNode, visited, soFar + node.Length - 1));

                    visited.Remove(nextLocation);
                }

                return max + node.Length - 1;
            }

            return 0;
        }
    }

    static int GetMaxPathNew(Dictionary<(int x, int y), HashSet<((int x, int y) pos, int value)>> graph, (int, int) start,
        (int, int) end, Dictionary<(int, int), int> visited)
    {
        if (start == end)
        {
            return visited.Values.Sum();
        }

        int maxSeen = 0;
        foreach (var path in graph[start])
        {
            if (!visited.ContainsKey(path.pos))
            {
                visited[path.pos] = path.value;
                int thisPath = GetMaxPathNew(graph, path.pos, end, visited);
                if (thisPath > maxSeen)
                {
                    maxSeen = thisPath;
                }
                visited.Remove(path.pos);
            }
        }

        return maxSeen;

    }


    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        var map = new Map();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            map.Elements.Add(line.ToCharArray());
        }

        // build a graph
        (PathNode graph, List<PathNode> allNodes) = BuildGraph(map);
        List<PathNode> firstNodes = new List<PathNode>();
        foreach (var node in allNodes)
        {
            if (!firstNodes.Any(edge =>
                    edge.Start == node.Start && edge.End == node.End ||
                    edge.End == node.Start && edge.Start == node.End))
            {
                node.EndConnections.Clear();
                node.StartConnections.Clear();
                firstNodes.Add(node);
            }
        }

        foreach (var node in firstNodes)
        {
            foreach (var checkNode in firstNodes)
            {
                if (checkNode == node) continue;
                if (node.Start == checkNode.Start || node.Start == checkNode.End)
                {
                    node.StartConnections.Add(checkNode);
                }

                if (node.End == checkNode.End || node.End == checkNode.Start)
                {
                    node.EndConnections.Add(checkNode);
                }
            }
        }


        Dictionary<(int x, int y), HashSet<((int x, int y), int)>> otherGraph =
            new Dictionary<(int x, int y), HashSet<((int x, int y), int)>>();
        foreach (var node in allNodes)
        {
            if (!otherGraph.ContainsKey(node.Start))
            {
                otherGraph[node.Start] = new HashSet<((int x, int y), int)>();
            }
            if (!otherGraph.ContainsKey(node.End))
            {
                otherGraph[node.End] = new HashSet<((int x, int y), int)>();
            }

            otherGraph[node.Start].Add((node.End, node.Length - 1));
            otherGraph[node.End].Add((node.Start, node.Length - 1));
        }

        int originalPath = GetMaxPath(graph.Start, graph,  new HashSet<(int x, int y)>(), 0);

        int endX = map.Elements[0].Length - 2;
        int endY = map.Elements.Count - 1;
        int path = GetMaxPathNew(otherGraph, graph.Start, (endX, endY), new Dictionary<(int, int), int>());

        DateTime end = DateTime.Now;
        Console.WriteLine($"Completed part {part} in {end - start}");
        Console.WriteLine($"Max path is {path}");


    }

}
