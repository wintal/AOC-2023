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

    static List<Operation> FindConjunctions(Operation op, int depth)
    {
        if (depth == 0) {
            Debug.Assert(!op.sources.Keys.Any(op => op.operation == Op.FlipFlop));
            return new List<Operation>() { op };
        }
        else
        {
            List<Operation> leaves = new List<Operation>();
            foreach (var childOp in op.sources.Keys)
            {
                leaves.AddRange(FindConjunctions(childOp, depth -1));
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

        //List<Operation> leafConjunctions = FindConjunctions(rxOp, 3);
        List<Operation> leafConjunctions = FindLeafConjunctions(rxOp);
        Dictionary<Operation, int> cycles = new Dictionary<Operation, int>();
        foreach (var operation in leafConjunctions)
        {
            cycles[operation] = 0;
        }


        while (true)
        {
            count++;
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

                if (leafConjunctions.Contains(thisOperator) && thisOperator.state == ButtonState.Low)
                {
                    cycles[thisOperator] = count;
                    if (cycles.Values.All(val => val > 0))
                    {
                        var vals = cycles.Values.ToList();
                        ulong overallCycle = (ulong) vals[0];
                        for (int i = 0; i < vals.Count; i++)
                        {
                            overallCycle = MathUtils.LowestCommonMultiple(overallCycle, (ulong)vals[i]);
                        }

                        solution = overallCycle;
                        break;
                    }
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
