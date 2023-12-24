using Utilities;

class Day19
{
    static int Main()
    {
        DoPart1("sample.txt");
        DoPart1("input.txt");

        DoPart2("sample.txt");
        DoPart2("input.txt");
        return 0;
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
            case "s":
                return Part.S;
            default:
                throw new Exception();
        }
    }

    enum Op
    {
        Lt,
        Gt,
        Accept,
        Reject, 
        Goto
    }

    class Rule
    {
        public Part part;
        public Op op;
        public int number;
        public string target;

        public (XmasValueRange inRange, XmasValueRange outRange) GetOverlap(XmasValueRange value)
        {
            switch (op)
            {
                case Op.Goto:
                    return (value, null);
                case Op.Lt:
                    return value.Split(part, Op.Lt, number);
                case Op.Gt:
                    return value.Split(part, Op.Gt, number);
                case Op.Accept:
                    return (value, null);
                case Op.Reject:
                    return (value, null);
                default:
                    throw new Exception();
            }

            return (null, null);
        }

        public bool Matches(XmasValue value)
        {
            int intValue;
            switch (part)
            {
                case Part.X:
                    intValue = value.X;
                    break;
                case Part.M:
                    intValue = value.M;
                    break;
                case Part.A:
                    intValue = value.A;
                    break;
                case Part.S:
                    intValue = value.S;
                    break;
                default:
                    throw new Exception();
            }
            switch (op)
            {
                case Op.Goto:
                    return true;
                case Op.Lt:
                    return intValue < number;
                case Op.Gt:
                    return intValue > number;
                case Op.Accept:
                    return true;
                case Op.Reject:
                    return true;
                default:
                    throw new Exception();
            }

            return false;
        }

        public override string ToString()
        {
            return $" {part} {op} {number} {target}";
        }
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
        Dictionary<string, RuleSet> instructions = new Dictionary<string, RuleSet>();
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
                var ruleStrings = parts[1].Substring(0, parts[1].Length-1).MySplit(",");
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
                        var ruleStringParts = ruleString.MySplit(":");
                        var ruleParts = ruleStringParts[0].Split('<');
                        rules.rules.Add(new Rule() { op = Op.Lt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]), target = ruleStringParts[1] });
                    }
                    else if (ruleString.Contains(">"))
                    {
                        var ruleStringParts = ruleString.MySplit(":");
                        var ruleParts = ruleStringParts[0].Split('>');
                        rules.rules.Add(new Rule() { op = Op.Gt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]), target = ruleStringParts[1] });
                    }
                    else
                    {
                        rules.rules.Add(new Rule() { op = Op.Goto, target = ruleString});
                    }
                }

                instructions.Add(rules.label, rules);

            }
            else
            {
                var parts = line.Substring(1,line.Length - 2).Split(",");
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

                values.Add(value);
            }
        }

        foreach (var value in values)
        {

            var rules = instructions["in"];
            bool accept = false;
            bool reject = false;
            while (rules != null)
            {

                foreach (var rule in rules.rules)
                {
                    if (rule.Matches(value))
                    {
                        if (rule.op == Op.Accept)
                        {
                            accept = true;
                            break;
                        } else if (rule.op == Op.Reject)
                        {
                            reject = true;
                            break;
                        }
                        if (rule.target == "R")
                        {
                            reject = true;
                            break;
                        } else if (rule.target == "A")
                        {
                            accept = true;
                            break;
                        }
                        rules = instructions[rule.target];
                        break;
                    }
                }

                if (accept || reject)
                {
                    break;
                }
            }

            if (accept)
            {
                solution += value.X + value.M + value.A + value.S;
            }
        }
   

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

    class XmasValueRange
    {
        public Range X;
        public Range M;
        public Range A;
        public Range S;
        public string StartInstruction = "in";

        private Range GetInRange(Range range, Op op, int splitValue)
        {
            switch (op)
            {
                case Op.Gt:
                    return new Range() { FromInc = splitValue + 1, ToInc = range.ToInc };
                case Op.Lt:
                    return new Range() { FromInc = range.FromInc, ToInc = splitValue - 1 };
                default:
                    throw new NotImplementedException();
            }
        }
        private Range GetOutRange(Range range, Op op, int splitValue)
        {
            switch (op)
            {
                case Op.Gt:
                    return new Range() { FromInc = range.FromInc, ToInc = splitValue };
                case Op.Lt:
                    return new Range() { FromInc = splitValue, ToInc = range.ToInc };
                default:
                    throw new NotImplementedException();
            }
        }

        public bool Empty()
        {
            return X.Empty() || M.Empty() || A.Empty() || S.Empty();
        }

        public (XmasValueRange rangeIn, XmasValueRange rangeOut) Split(Part part, Op op, int splitValue)
        {
            switch (part)
            {
                case Part.X:
                {
                    var left = new XmasValueRange() { M = M, A = A, S = S, X = GetInRange(X, op, splitValue) };
                    var right = new XmasValueRange() { M = M, A = A, S = S, X = GetOutRange(X, op, splitValue) };
                    return (left, right);
                }

                    break;
                case Part.M:
                {
                    var left = new XmasValueRange() { X =X, A = A, S = S, M = GetInRange(M, op, splitValue) };
                    var right = new XmasValueRange() { X = X, A = A, S = S, M = GetOutRange(M, op, splitValue) };
                    return (left, right);
                    }

                    break;
                case Part.A:
                {
                    var left = new XmasValueRange() { M = M, X = X, S = S, A = GetInRange(A, op, splitValue) };
                    var right = new XmasValueRange() { M = M, X = X, S = S, A = GetOutRange(A, op, splitValue) };
                    return (left, right);
                    }

                    break;
                case Part.S:
                {
                    var left = new XmasValueRange() { M = M, A = A, X = X, S = GetInRange(S, op, splitValue) };
                    var right = new XmasValueRange() { M = M, A = A, X = X, S = GetOutRange(S, op, splitValue) };
                    return (left, right);
                    }

                    break;
            }

            throw new Exception();
        }

        public override string ToString()
        {
            return $"Ins {StartInstruction} X: {X}\tM: {M}\tA: {A}\tS: {S}";
        }
    }


    static void DoPart2(string inputFile)
    {
        int part = 2;
        DateTime start = DateTime.Now;
        var lines = System.IO.File.ReadAllLines(inputFile);

        long solution = 0;
        Dictionary<string, RuleSet> instructions = new Dictionary<string, RuleSet>();
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
                var ruleStrings = parts[1].Substring(0, parts[1].Length - 1).MySplit(",");
                foreach (var ruleString in ruleStrings)
                {
                    if (ruleString == "A")
                    {
                        rules.rules.Add(new Rule() { op = Op.Accept });
                    }
                    else if (ruleString == "R")
                    {
                        rules.rules.Add(new Rule() { op = Op.Reject });
                    }
                    else if (ruleString.Contains("<"))
                    {
                        var ruleStringParts = ruleString.MySplit(":");
                        var ruleParts = ruleStringParts[0].Split('<');
                        rules.rules.Add(new Rule() { op = Op.Lt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]), target = ruleStringParts[1] });
                    }
                    else if (ruleString.Contains(">"))
                    {
                        var ruleStringParts = ruleString.MySplit(":");
                        var ruleParts = ruleStringParts[0].Split('>');
                        rules.rules.Add(new Rule() { op = Op.Gt, number = int.Parse(ruleParts[1]), part = PartFromString(ruleParts[0]), target = ruleStringParts[1] });
                    }
                    else
                    {
                        rules.rules.Add(new Rule() { op = Op.Goto, target = ruleString });
                    }
                }

                instructions.Add(rules.label, rules);

            }
            else
            {
                var parts = line.Substring(1, line.Length - 2).Split(",");
                XmasValue value = new XmasValue();
                foreach (var partStr in parts)
                {
                    var kvp = partStr.Split('=');
                    switch (kvp[0])
                    {
                        case "x":
                            value.X = int.Parse(kvp[1]);
                            break;
                        case "m":
                            value.M = int.Parse(kvp[1]);
                            break;
                        case "a":
                            value.A = int.Parse(kvp[1]);
                            break;
                        case "s":
                            value.S = int.Parse(kvp[1]);
                            break;
                        default:
                            throw new Exception();
                    }
                }

                values.Add(value);
            }
        }

        XmasValueRange range = new XmasValueRange()
        {
            X = new Range() { FromInc = 1, ToInc = 4000 },
            M = new Range() { FromInc = 1, ToInc = 4000 },
            A = new Range() { FromInc = 1, ToInc = 4000 },
            S = new Range() { FromInc = 1, ToInc = 4000 }
        };

        Stack<XmasValueRange> ranges = new Stack<XmasValueRange>();
        ranges.Push(range);

        List<XmasValueRange> acceptedRanges = new List<XmasValueRange>();
        List<XmasValueRange> rejectedRanges = new List<XmasValueRange>();
        while (ranges.Any()) {

            var thisRange = ranges.Pop();
            var rules = instructions[thisRange.StartInstruction];
            Console.WriteLine($"Processing range {thisRange}");

            foreach (var rule in rules.rules)
            {
                Console.WriteLine($"Rule {rule}");
                var (inRange, outRange) = rule.GetOverlap(thisRange);
                if (inRange != null && !inRange.Empty())
                {
                    if (rule.op == Op.Accept || rule.target == "A")
                    {
                        Console.WriteLine($"Accepting range {inRange}");
                        acceptedRanges.Add(inRange);
                    }
                    else if (rule.op == Op.Reject || rule.target == "R")
                    {
                        Console.WriteLine($"Rejecting range {inRange}");
                        rejectedRanges.Add(inRange);
                    }
                    else
                    {

                        Console.WriteLine($"Further processing on range {inRange} by rule {rule.target}");
                        inRange.StartInstruction = rule.target;
                        ranges.Push(inRange);
                    }
                }

                thisRange = outRange;
                if (thisRange == null || thisRange.Empty())
                {
                    break;
                }
            }
        }

        foreach (var thisRange in acceptedRanges)
        {
            solution += thisRange.X.TotalRange * thisRange.M.TotalRange * thisRange.A.TotalRange * thisRange.S.TotalRange;
        }

        long antiSolution = 0;
        foreach (var thisRange in rejectedRanges)
        {
            antiSolution += thisRange.X.TotalRange * thisRange.M.TotalRange * thisRange.A.TotalRange * thisRange.S.TotalRange;
        }


        DateTime end = DateTime.Now;
        Console.WriteLine($"Completed part {part} in {end - start}");


    }

}
