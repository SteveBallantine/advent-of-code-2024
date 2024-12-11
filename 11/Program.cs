// See https://aka.ms/new-console-template for more information

var exampleInput = @"125 17";
AssertFor(exampleInput, false, 55312);
//AssertFor(exampleInput, true, 31);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/11/input.txt"), false, false));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/11/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    List<long> stones = input[0].Split(' ').Select(long.Parse).ToList();

    int step = 0;
    while (step < 25)
    {
        for(int p = 0; p < stones.Count; p++)
        {
            var stroneAsString = stones[p].ToString();
            if (stones[p] == 0)
            {
                stones[p] = 1;
            } 
            else if (stroneAsString.Length % 2 == 0)
            {
                var newStone1 = stroneAsString[..(stroneAsString.Length / 2)];
                var newStone2 = stroneAsString[^(stroneAsString.Length / 2)..];
                stones.Insert(p, long.Parse(newStone1));
                p++;
                stones[p] = long.Parse(newStone2);
            }
            else
            {
                stones[p] *= 2024;
            }
        }

        if (logging)
        {
            Console.WriteLine(stones.Count < 100 ? string.Join(' ', stones) : stones.Count);
        }

        step++;
    }
    
    return stones.Count;
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