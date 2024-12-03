// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

var exampleInput = @"xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
AssertFor(exampleInput, false, 161);
var exampleInput2 = @"xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
AssertFor(exampleInput2, true, 48);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/03/input.txt"), false, false));

Console.WriteLine("Part2:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/03/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var result = 0;

    var matches = Regex.Matches(string.Concat(input), "mul\\(\\d+,\\d+\\)|do\\(\\)|don't\\(\\)");
    bool enabled = true;
    
    foreach (var match in matches)
    {
        var instruction = match.ToString();
        if (instruction.StartsWith("mul") && enabled)
        {
            result += Evaluate(match.ToString(), logging);
        }
        else if (instruction.StartsWith("don't") && part2)
        {
            enabled = false;
        }
        else if (instruction.StartsWith("do") && part2)
        {
            enabled = true;
        }
    }
    
    return result;
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

int Evaluate(string input, bool logging)
{
    var values = input.Split(',')
        .Select(IgnoreNonNumeric)
        .Select(y => int.Parse(y))
        .ToArray();

    if (logging)
    {
        Console.WriteLine($"{values[0]} x {values[1]}");
    }
    
    return values[0] * values[1];
}

string IgnoreNonNumeric(string input)
{
    return new string(input.Where(c => char.IsDigit(c)).ToArray());
}