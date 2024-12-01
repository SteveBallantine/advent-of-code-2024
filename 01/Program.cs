// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;

var exampleInput = @"3   4
4   3
2   5
1   3
3   9
3   3";
AssertFor(exampleInput, false, 11);

Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/01/input.txt"), false, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var l1 = new List<int>();
    var l2 = new List<int>();
    
    var values = input.Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray());
    foreach (var entry in values)
    {
        l1.Add(entry[0]);
        l2.Add(entry[1]);
    }

    var sl1 = l1.Order().ToArray();
    var sl2 = l2.Order().ToArray();

    var result = 0;
    for (int i = 0; i < sl1.Count(); i++)
    {
        result += Math.Max(sl1[i], sl2[i]) - Math.Min(sl1[i], sl2[i]);
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