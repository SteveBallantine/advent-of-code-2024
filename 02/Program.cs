// See https://aka.ms/new-console-template for more information

using System.ComponentModel.Design;

var exampleInput = @"7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9";
AssertFor(exampleInput, false, 2);
AssertFor(exampleInput, true, 4);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/02/input.txt"), false, false));

Console.WriteLine("Part2:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/02/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var result = input.Select(i => i.Split(' ').Select(int.Parse).ToArray())
        .Count(i => IsSafe(i, part2, logging));
    
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

bool IsSafe(int[] report, bool part2, bool logging)
{
    var result = IsReportSafe(report, logging);

    if (!result && part2)
    {
        var removingMakesSafe = false;
        var i = 0;
        while(!removingMakesSafe && i < report.Length)
        {
            var list = report.ToList();
            list.RemoveAt(i);
            removingMakesSafe = removingMakesSafe || IsReportSafe(list.ToArray(), logging);
            i++;
        }

        result = removingMakesSafe;
    }

    if (logging)
    {
        Console.WriteLine(string.Join(',', report));
        Console.WriteLine(result ? "SAFE" : "NOT SAFE");
    }

    return result;
}


bool IsReportSafe(int[] report, bool logging)
{
    bool result = true;
    bool increasing = report[0] < report[1];
    int i = 0;
    
    while (result && i < report.Length - 1)
    {
        result = Compare(report[i], report[i + 1], increasing);
        i++;
    }
    
    return result;
}

bool Compare(int i1, int i2, bool increasing)
{
    var diff = i1 - i2;
    var result = increasing ? diff < 0 : diff > 0;
    result = result && diff >= -3 && diff <= 3 && diff != 0;
    return result;
}
