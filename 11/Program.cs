// See https://aka.ms/new-console-template for more information

var exampleInput = @"125 17";
AssertFor(exampleInput, false, 0, 55312);
AssertFor(exampleInput, true, 1, 3);
AssertFor(exampleInput, true, 2, 4);
AssertFor(exampleInput, true, 3, 5);
AssertFor(exampleInput, true, 4, 9);
AssertFor(exampleInput, true, 25, 55312);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/11/input.txt"), false, 25, false));

Console.WriteLine("Part2");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/11/input.txt"), true, 75, false));


long RunFor(string[] input, bool part2, int steps, bool logging)
{
    List<long> stones = input[0].Split(' ').Select(long.Parse).ToList();

    if (part2)
    {
        return Part2(stones, steps, logging);
    }
    return Part1(stones);
}

void AssertFor(string input, bool part2, int steps, long expectedResult)
{
    var lines = input.Split(System.Environment.NewLine);
    var result = RunFor(lines, part2, steps, true);
    if (result != expectedResult)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        throw new Exception($"Result was {result} but expected {expectedResult}");
    }
}

long Part2(List<long> stones, int stepsToRun, bool logging) 
{
    Dictionary<long, StoneEvolution> evolutions = DeterminePossibleEvolutions(stones.ToArray(), logging);

    long result = 0;
    foreach (var stone in stones)
    {
        result += EvolveAndCount(stone, evolutions, stepsToRun);
    }
    return result;
}

long EvolveAndCount(long stone, Dictionary<long, StoneEvolution> evolutions, int stepsRemaining)
{
    if (evolutions[stone].StepsToSplit <= stepsRemaining)
    {
        if (!evolutions[stone].ChildrenAfterXSteps.TryGetValue(stepsRemaining, out var count))
        {
            count += EvolveAndCount(evolutions[stone].Child1, evolutions,
                stepsRemaining - evolutions[stone].StepsToSplit);
            count += EvolveAndCount(evolutions[stone].Child2, evolutions,
                stepsRemaining - evolutions[stone].StepsToSplit);
            evolutions[stone].ChildrenAfterXSteps.Add(stepsRemaining, count);
        }
        return count;
    }
    return 1;
}

int Part1(List<long> stones) 
{
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

        step++;
    }
    
    return stones.Count;
}

Dictionary<long, StoneEvolution> DeterminePossibleEvolutions(long[] stones, bool logging)
{
    Dictionary<long, StoneEvolution> evolutions = new Dictionary<long, StoneEvolution>();
    Queue<long> numbersToProcess = new Queue<long>(stones);
    
    while(numbersToProcess.TryDequeue(out var stone))
    {
        var steps = 0;
        var x = stone;
        while (x == 0 || x.ToString().Length % 2 != 0)
        {
            if (x == 0)
            {
                x = 1; 
            }
            else
            {
                x *= 2024;
            }
            steps++;
        }

        var stoneAsString = x.ToString();
        var child1 = long.Parse(stoneAsString[..(stoneAsString.Length / 2)]);
        var child2 = long.Parse(stoneAsString[^(stoneAsString.Length / 2)..]);
        var childrenAfterXSteps = new Dictionary<int, long> { { steps + 1, 2 } };
        evolutions.Add(stone, new StoneEvolution(steps + 1, child1, child2, childrenAfterXSteps));
        if (!evolutions.ContainsKey(child1) && !numbersToProcess.Contains(child1))
        {
            numbersToProcess.Enqueue(child1);
        }
        if (!evolutions.ContainsKey(child2) && !numbersToProcess.Contains(child2))
        {
            numbersToProcess.Enqueue(child2);
        }

        if (logging && evolutions.Count % 100 == 0)
        {
            Console.WriteLine($"{evolutions.Count} processed - {numbersToProcess.Count} to go");
        }
    }

    return evolutions;
}

record StoneEvolution(int StepsToSplit, long Child1, long Child2, Dictionary<int, long> ChildrenAfterXSteps);
