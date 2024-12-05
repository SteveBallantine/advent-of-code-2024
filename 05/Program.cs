// See https://aka.ms/new-console-template for more information

var exampleInput = @"47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47";
AssertFor(exampleInput, false, 143);
AssertFor(exampleInput, true, 123);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/05/input.txt"), false, false));

Console.WriteLine("Part2:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/05/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var pagesThatMustBeAfterKey = new Dictionary<int, List<int>>();
    var pageLists = new List<int[]>();
    
    Parse(input, pagesThatMustBeAfterKey, pageLists);

    if (!part2)
    {
        return pageLists.Where(l => IsValid(l, pagesThatMustBeAfterKey))
            .Select(l => l[l.Length / 2])
            .Sum();
    }
    
    var invalidLists = pageLists.Where(l => !IsValid(l, pagesThatMustBeAfterKey));
    
    return invalidLists.Select(l => FixList(l, pagesThatMustBeAfterKey))
        .Select(l => l[l.Length / 2])
        .Sum();
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

int[] FixList(int[] pageList, Dictionary<int, List<int>> pagesThatMustBeAfterKey)
{
    var newList = new List<int>();
    var previousPages = new List<int>();

    var relevantRules = pageList.ToDictionary(pageNo => pageNo,
        pageNo => pagesThatMustBeAfterKey.TryGetValue(pageNo, out var pagesThatMustBeAfter) ? 
            pagesThatMustBeAfter.Where(pageList.Contains).ToArray() : []);

    foreach (var pageNo in pageList)
    {
        if (relevantRules.TryGetValue(pageNo, out var pagesThatMustBeAfter))
        {
            if (previousPages.Any(p => pagesThatMustBeAfter.Contains(p)))
            {
                int indexToInsertAt = 0;
                while (!pagesThatMustBeAfter.Contains(newList[indexToInsertAt]))
                {
                    indexToInsertAt++;
                }
                newList.Insert(indexToInsertAt, pageNo);
            }
            else
            {
                newList.Add(pageNo);
            }
        }
        
        Console.WriteLine(string.Join(",", newList));
        previousPages.Add(pageNo);
    }

    return newList.ToArray();
}

bool IsValid(int[] pageList, Dictionary<int, List<int>> pagesThatMustBeAfterKey)
{
    var previousPages = new List<int>();
    
    foreach (var pageNo in pageList)
    {
        if (pagesThatMustBeAfterKey.TryGetValue(pageNo, out var pagesThatMustBeAfter))
        {
            if (previousPages.Any(p => pagesThatMustBeAfter.Contains(p)))
            {
                break;
            }
        }
        
        previousPages.Add(pageNo);
    }

    return previousPages.Count == pageList.Length;
}

void Parse(string[] strings, Dictionary<int, List<int>> pagesThatMustBeAfterKey, List<int[]> pageLists)
{
    int lineNo = 0;
    bool rules = true;
    while (lineNo < strings.Length)
    {
        var line = strings[lineNo];
        if (rules)
        {
            if (line.Contains('|'))
            {
                var pageNos = line.Split('|').Select(int.Parse).ToArray();
                if (!pagesThatMustBeAfterKey.TryGetValue(pageNos[0], out var pagesThatMustBeAfter))
                {
                    pagesThatMustBeAfter = new List<int>();
                    pagesThatMustBeAfterKey.Add(pageNos[0], pagesThatMustBeAfter);
                }
                pagesThatMustBeAfter.Add(pageNos[1]);
            }
            else 
            {
                rules = false;
            }
        }
        else
        {
            pageLists.Add(line.Split(',').Select(int.Parse).ToArray());
        }
        
        lineNo++;
    }
}


