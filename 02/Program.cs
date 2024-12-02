// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;

var exampleInput = @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9";
AssertFor(exampleInput, false, 2);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/02/input.txt"), false, false));

//Console.WriteLine("Part2:");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/02/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var result = input.Select(i => i.Split(' ').Select(int.Parse).ToArray())
        .Count(i => IsSafe(i));
    
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

bool IsSafe(int[] report)
{
    bool result = true;
    bool increasing = true;
    int i = 0;
    
    while (result && i < report.Length - 1)
    {
        var diff = report[i] - report[i + 1];
        if (i == 0)
        {
            increasing = diff > 0;
        }
        else
        {
            result = result && increasing ? diff > 0 : diff < 0;
        }
        result = result && diff >= -3 && diff <= 3 && diff != 0;
        i++;
    }
    Console.WriteLine(string.Join(',', report));
    Console.WriteLine(result ? "SAFE" : "NOT SAFE");
    
    return result;
}