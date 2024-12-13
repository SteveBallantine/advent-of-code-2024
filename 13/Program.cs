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
//AssertFor(exampleInput, true, 31);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/13/input.txt"), false, false));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/13/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var cheapestWins = Parse(input).Select(FindCheapestWin);
    return cheapestWins.Sum();
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

int FindCheapestWin(Machine m)
{
    var aCost = Math.Sqrt(m.A.X ^ 2 + m.A.Y ^ 2) / m.A.Price;
    var bCost = Math.Sqrt(m.B.X ^ 2 + m.B.Y ^ 2) / m.B.Price;
    
    var cB = aCost >= bCost ? m.B : m.A;
    var eB = aCost >= bCost ? m.A : m.B;

    var maxCheapPressesX = m.Prize.X / cB.X;
    var maxCheapPressesY = m.Prize.Y / cB.Y;
    var maxCheapPresses = Math.Min(maxCheapPressesX, maxCheapPressesY);
    

    int cP = maxCheapPresses;
    while (cP >= 0)
    {
        var remainingDistanceX = m.Prize.X - (cP * cB.X);
        var remainingDistanceY = m.Prize.Y - (cP * cB.Y);

        var xPresses = Math.DivRem(remainingDistanceX, eB.X, out var xRemainder);
        var yPresses = Math.DivRem(remainingDistanceY, eB.Y, out var yRemainder);
        
        if (xRemainder == 0 &&
            yRemainder == 0 &&
            xPresses == yPresses)
        {
            return cP * cB.Price + xPresses * eB.Price;
        }
        cP--;
    }

    return 0;
}

IEnumerable<Machine> Parse(string[] input)
{
    int line = 0;
    while (line < input.Length - 3)
    {
        var v1 = GetValues(input[line]);
        var a = new Button(v1[0], v1[1], 3);
        var v2 = GetValues(input[line + 1]);
        var b = new Button(v2[0], v2[1], 1);
        var v3 = GetValues(input[line + 2]);
        var prize = new Prize(v3[0], v3[1]);
        
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

record Button(int X, int Y, int Price);
record Prize(int X, int Y);

record Machine(Button A, Button B, Prize Prize);