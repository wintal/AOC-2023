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
using System.Xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Utilities;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

class Day16
{
    static int Main()
    {
      //  DoPart1("sample.txt");
        DoPart1("input.txt");

        //DoPart2("sample.txt");
        //DoPart2("input.txt");
        return 0;
    }


    enum Op
    {
        Broadcast,
        Conjunction,
        FlipFlop,
        Accumulator

    }
    class Operation
    {
        public Op operation;
        public string name;
        public List<string> targets = new List<string>();
        public Dictionary<Operation, ButtonState> sources = new Dictionary<Operation, ButtonState>();
        public ButtonState state;
        public long Highs;
        public long Lows;
    }



    enum ButtonState
    {
        High,
        Low
    }

    class PendingOperation
    {
        public string target;
        public ButtonState state;
    }

    class PendingOperationStep
    {
        public List<PendingOperation> operations = new List<PendingOperation>();
    }

    static List<Operation> FindLeafConjunctions(Operation op)
    {
        if (!op.sources.Keys.All(op => op.operation == Op.Conjunction))
        {
            Debug.Assert(!op.sources.Keys.Any(op => op.operation == Op.FlipFlop));
            return new List<Operation>() { op };
        }
        else
        {
            List<Operation> leaves = new List<Operation>();
            foreach (var childOp in op.sources.Keys)
            {
                leaves.AddRange(FindLeafConjunctions(childOp));
            }
            return leaves;
        }

    }

    static void DoPart1(string inputFile)
    {
        int part = 1;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        ulong solution = 0;
        Dictionary<string, Operation> operations = new Dictionary<string, Operation>();

        for (int i = 0; i < lines.Length; i++)
        {

            var line = lines[i];
            var parts = line.MySplit(" -> ");
            if (parts[0].StartsWith("%"))
            {
                Operation op = new Operation();
                op.name = parts[0].Substring(1);
                op.operation = Op.FlipFlop;
                op.targets = parts[1].MySplit(",").ToList();
                op.state = ButtonState.Low;
                operations[op.name] = op;
            }
            else if (parts[0].StartsWith("&"))
            {
                Operation op = new Operation();
                op.name = parts[0].Substring(1);
                op.operation = Op.Conjunction;
                op.targets = parts[1].MySplit(",").ToList();
                op.state = ButtonState.Low;
                operations[op.name] = op;
            }
            else
            {
                Operation op = new Operation();
                op.name = parts[0];
                op.operation = Op.Broadcast;
                op.targets = parts[1].MySplit(",").ToList();
                op.state = ButtonState.Low;
                operations[op.name] = op;
            }

        }


        List<string> toAdd = new List<string>();
        foreach (var operation in operations.Values)
        {
            foreach (var targetName in operation.targets)
            {
                if (!operations.ContainsKey(targetName))
                {
                    toAdd.Add(targetName);
                }
                else
                {
                    var target = operations[targetName];
                    if (target.operation == Op.Conjunction)
                    {
                        target.sources[operation] = ButtonState.Low;
                    }
                }
            }
        }

        foreach (var target in toAdd)
        {

            operations[target] = new Operation() { operation = Op.Accumulator, name = target };
        }

        foreach (var operation in operations.Values)
        {
            foreach (var targetName in operation.targets)
            {
                if (!operations.ContainsKey(targetName))
                {
                    toAdd.Add(targetName);
                }
                else
                {
                    var target = operations[targetName];
                   
                        target.sources[operation] = ButtonState.Low;
                }
            }
        }

        Queue<PendingOperation> pendingOperations = new Queue<PendingOperation>();
        var firstStep = new PendingOperationStep();

        long low = 0;
        long high = 0;
        var rxOp = operations["rx"];
        int count = 0;

        List<Operation> leafConjunctions = FindLeafConjunctions(rxOp);
        List<List<bool>> sequences = new List<List<bool>>();
        List<bool> seenLow = new List<bool>();
        List<bool> seenhigh = new List<bool>();
        List<int> leafConjunctionsCycle = new List<int>();
        foreach (var con in leafConjunctions)
        {
            foreach (var source in con.sources)
            {
                sequences.Add(new List<bool>());
                seenLow.Add(false);
                seenhigh.Add(false);
            }

            leafConjunctionsCycle.Add(int.MaxValue);
        }

        while (rxOp.Lows != 1)
        {
            count++;
            
                StringBuilder builder = new StringBuilder();
                int flipFlopCount = 0;
                for (int i = 0; i < leafConjunctions.Count; i++)
                {
                    var leaf = leafConjunctions[i];
                    foreach (var source in leaf.sources)
                    {
                        sequences[flipFlopCount].Add(source.Key.state == ButtonState.Low);
                        if (source.Key.state == ButtonState.High)
                        {
                            seenhigh[flipFlopCount] = true;
                        }
                        else if (seenhigh[flipFlopCount] && source.Key.state == ButtonState.Low)
                        {
                            seenLow[flipFlopCount] = true;
                        }
                        flipFlopCount++;
                        builder.Append(source.Key.state == ButtonState.Low ? "L" : "H");
                    }

                    if (leaf.state == ButtonState.Low)
                    {
                        leafConjunctionsCycle.Add(count - 1);
                    }
                  //  builder.Append(" ");

                }
            //    Console.WriteLine(builder.ToString());

                if (leafConjunctionsCycle.All(count => count != int.MaxValue))
                {
                    ulong cycle = (ulong)leafConjunctionsCycle[0];
                    for (int i = 1; i < leafConjunctionsCycle.Count; i++)
                    {
                        cycle = MathUtils.LowestCommonMultiple(cycle, (ulong)leafConjunctionsCycle[i]);
                    }

                    solution = cycle;

                }

                if (seenLow.All(op => op == true) && seenhigh.All(op => op == true))
                {
                    List<int> sequenceLengths = new List<int>();
                    
                    foreach (var sequence in sequences)
                    {
                        bool seenHigh = false;
                        for (int i = 1; i < sequence.Count; i++)
                        {
                            if (!sequence[i])
                            {
                                seenHigh = true;
                            }
                            else if (seenHigh && sequence[i] == true)
                            {
                                sequenceLengths.Add(i);
                                break;
                            }
                        }
                    }

                    ulong cycle = (ulong)sequenceLengths[0];
                    for (int i = 1; i < sequenceLengths.Count; i++)
                    {
                        cycle = MathUtils.LowestCommonMultiple(cycle, (ulong)sequenceLengths[i]);
                    }

                    solution = cycle;
                  //  break;
                }


                rxOp.Lows = 0;
            pendingOperations.Enqueue(new PendingOperation() { state = ButtonState.Low, target = "broadcaster" });
            while (pendingOperations.Any())
            {
                var op = pendingOperations.Dequeue();
                var thisOperator = operations[op.target];
                if (op.state == ButtonState.Low)
                {
                    low++;
                }
                else
                {
                    high++;
                }

                switch (thisOperator.operation)
                {
                    case Op.FlipFlop:
                    {
                        if (op.state == ButtonState.Low)
                        {
                            var newState = thisOperator.state == ButtonState.High
                                ? ButtonState.Low
                                : ButtonState.High;
                            thisOperator.state = newState;
                            foreach (var target in thisOperator.targets)
                            {
                                pendingOperations.Enqueue(new PendingOperation()
                                    { target = target, state = newState });
                            }
                        }
                    }
                        break;
                    case Op.Broadcast:
                    {
                        foreach (var target in thisOperator.targets)
                        {
                            pendingOperations.Enqueue(new PendingOperation()
                                { target = target, state = thisOperator.state });
                        }
                    }
                        break;
                    case Op.Conjunction:
                    {
                        bool allHigh = true;
                        foreach (var source in thisOperator.sources.Keys)
                        {
                            if (source.state == ButtonState.Low)
                            {
                                allHigh = false;
                            }
                        }

                        var newState = allHigh ? ButtonState.Low : ButtonState.High;
                        thisOperator.state = newState;
                        foreach (var target in thisOperator.targets)
                        {
                            pendingOperations.Enqueue(new PendingOperation() { target = target, state = newState });
                        }
                    }
                        break;
                    case Op.Accumulator:
                    {
                        if (op.state == ButtonState.Low)
                        {
                            thisOperator.Lows += 1;
                        }
                        else
                        {
                            thisOperator.Highs += 1;

                        }

                        thisOperator.state = op.state;
                        ;
                    }
                        break;

                }


            }
        }

        solution = (ulong)count;
        DateTime end = DateTime.Now;
        Console.WriteLine($"Part {part} ({inputFile})- {solution}");

        Console.WriteLine($"Completed part {part} in {end - start}");
    }

    struct Range
    {
        public Range()
        {
            FromInc = 1;
            ToInc = 4000;
        }
        public int FromInc = 1;
        public int ToInc = 4000;
        public long TotalRange => ToInc - FromInc + 1;
        public bool Empty() => FromInc > ToInc;


        public override string ToString()
        {
            return $"{FromInc}->{ToInc}";
        }
    }
    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;



        DateTime end = DateTime.Now;
        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
