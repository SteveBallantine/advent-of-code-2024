var splitChars = new [] { '+', ',', '=' };

var exampleInput = @"Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279";
AssertFor(exampleInput, false, 480);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/13/input.txt"), false, false));

Console.WriteLine("Part2");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/13/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var machines = Parse(input, part2).ToArray();
    return machines.Sum(GetCost);
}

void AssertFor(string input, bool part2, long expectedResult)
{
    var lines = input.Split(System.Environment.NewLine);
    var result = RunFor(lines, part2, false);
    if (result != expectedResult)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        throw new Exception($"Result was {result} but expected {expectedResult}");
    }
}

long GetCost(Machine m)
{
    var aCost = Math.Sqrt(m.A.X ^ 2 + m.A.Y ^ 2) / m.A.Price;
    var bCost = Math.Sqrt(m.B.X ^ 2 + m.B.Y ^ 2) / m.B.Price;
    
    var cB = aCost >= bCost ? m.B : m.A;
    var eB = aCost >= bCost ? m.A : m.B;
    
    var cBOriginLine = new Line(new Point(cB.X, cB.Y), new Point(0, 0));
    var eBPrizeLine = new Line(new Point(m.Prize.X - eB.X, m.Prize.Y - eB.Y), new Point(m.Prize.X, m.Prize.Y));

    var i = GetIntersection(cBOriginLine, eBPrizeLine);

    if (i.X > 0 && i.Y > 0 &&
        i.X % cB.X == 0 &&
        (m.Prize.X - i.X) % eB.X == 0)
    {
        var cBPresses = i.X / cB.X;
        var eBPresses = (m.Prize.X - i.X) / eB.X;
        return (cBPresses * cB.Price + eBPresses * eB.Price);
    }

    return 0;
}

Point GetIntersection(Line a, Line b)
{        
    double c1 = a.P2.Y - a.P1.Y;
    double d1 = a.P1.X - a.P2.X;
    double e1 = c1 * a.P1.X + d1 * a.P1.Y;

    double c2 = b.P2.Y - b.P1.Y;
    double d2 = b.P1.X - b.P2.X;
    double e2 = c2 * b.P1.X + d2 * b.P1.Y;

    double delta = c1 * d2 - c2 * d1;
    
    if (delta == 0) return new Point(-1, -1);
    var x = (d2 * e1 - d1 * e2) / delta; 
    var y = (c1 * e2 - c2 * e1) / delta;
    return new Point((long)Math.Round(x), (long)Math.Round(y));
}


IEnumerable<Machine> Parse(string[] input, bool part2)
{
    int line = 0;
    while (line <= input.Length - 3)
    {
        var v1 = GetValues(input[line]);
        var a = new Button(v1[0], v1[1], 3);
        var v2 = GetValues(input[line + 1]);
        var b = new Button(v2[0], v2[1], 1);
        var v3 = GetValues(input[line + 2]);
        
        var prize = part2 ? new Prize(v3[0] + 10000000000000, v3[1] + 10000000000000) : new Prize(v3[0], v3[1]);
        
        yield return new Machine(a, b, prize);
        line += 4;
    }
}

int[] GetValues(string input)
{
    return input.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => 
        {
            if (int.TryParse(x, out var v))
            {
                return v;
            }
            return 0;
        })
        .Where(x => x > 0)
        .ToArray();
}

record Point(long X, long Y);

record Button(int X, int Y, int Price);
record Prize(long X, long Y);

record Line(Point P1, Point P2);

record Machine(Button A, Button B, Prize Prize);