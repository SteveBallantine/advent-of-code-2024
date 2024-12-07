// See https://aka.ms/new-console-template for more information

using NCalc;

var Operators1 = new char[] { '*', '+' };
var Operators2 = new char[] { '*', '+', '|' };
var SplitChars = new [] { ' ', ':' };

var exampleInput = @"190: 10 19
3267: 81 40 27
83: 17 5
156: 15 6
7290: 6 8 6 15
161011: 16 10 13
192: 17 8 14
21037: 9 7 18 13
292: 11 6 16 20";
AssertFor(exampleInput, false, 3749);
AssertFor(exampleInput, true, 11387);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/07/input.txt"), false, false));

Console.WriteLine("Part2:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/07/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    long result = 0;
    
    var entries = Parse(input);
    foreach (var entry in entries)
    {
        var match = FindMatchingSequence(part2, entry.Expected, entry.Values);
        if (match != string.Empty)
        {
            if (logging)
            {
                Console.WriteLine(match);
            }
            result += entry.Expected;
        }
    }
    
    return result;
}

void AssertFor(string input, bool part2, long expectedResult)
{
    var lines = input.Split(System.Environment.NewLine);
    var result = RunFor(lines, part2, true);
    if (result != expectedResult)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        throw new Exception($"Result was {result} but expected {expectedResult}");
    }
}

string FindMatchingSequence(bool part2, long expectedOutput, long[] numbers)
{
    return Recurse(part2, expectedOutput, numbers, 0, numbers[0].ToString(), numbers[0]);
}

string Recurse(bool part2, long expectedOutput, long[] numbers, int position, string expressionText, long valueSoFar)
{
    foreach (var o in part2 ? Operators2 : Operators1)
    {
        long newValue = 0;
        switch (o)
        {
            case '+':
                newValue = valueSoFar + numbers[position + 1];
                break;
            case '*':
                newValue = valueSoFar * numbers[position + 1];
                break;
            case '|':
                newValue = long.Parse($"{valueSoFar}{numbers[position + 1]}");
                break;
        }
        string newExpressionText = string.Concat(expressionText, $"{o}{numbers[position + 1]}");
        
        if (position < numbers.Length - 2)
        {
            var result = Recurse(part2, expectedOutput, numbers, position + 1, newExpressionText, newValue);
            if (result != string.Empty) { return result; }
        }
        else
        {
            if (expectedOutput == newValue)
            {
                return newExpressionText;
            }
        }
    }
    return string.Empty;
}

IEnumerable<InputLine> Parse(string[] input)
{
    foreach (var line in input)
    {
        var numbers = line
            .Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => long.Parse(s))
            .ToArray();
        yield return new InputLine(numbers[0], numbers[1..]);
    }
}

record InputLine(long Expected, long[] Values);