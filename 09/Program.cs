// See https://aka.ms/new-console-template for more information

var exampleInput = @"2333133121414131402";
AssertFor(exampleInput, false, 1928);
//AssertFor(exampleInput, true, 31);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/09/input.txt"), false, false));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/09/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    DiskMap disk = new DiskMap(input[0]);
    disk.Defrag(logging);
    disk.Log();
    Console.WriteLine();
    return disk.GetChecksum();
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


class DiskMap()
{
    private int?[] Disk;
    
    public DiskMap(string input) : this()
    {
        Disk = input.SelectMany((c, i) =>
        {
            var diskAreaLength = int.Parse(c.ToString());
            int? fileId = i % 2 == 0 ? i / 2 : null;
            return Enumerable.Repeat(fileId, diskAreaLength);
        }).ToArray();
        Log();
    }

    public long GetChecksum()
    {
        return Disk.Where(d => d != null).Select((d, i) => (long)(d.Value * i)).Sum();
    }

    public void Defrag(bool logging)
    {
        int readHeadForwardPosition = 0;
        int readHeadReversePosition = Disk.Length - 1;

        while (readHeadForwardPosition < readHeadReversePosition)
        {
            if (Disk[readHeadForwardPosition] == null)
            {
                Disk[readHeadForwardPosition] = Disk[readHeadReversePosition];
                Disk[readHeadReversePosition] = null;
                
                readHeadReversePosition--;
                while (Disk[readHeadReversePosition] == null)
                {
                    readHeadReversePosition--;
                }
            }
            readHeadForwardPosition++;
        }
    }

    public void Log()
    {
        foreach (var entry in Disk)
        {
            Console.Write(entry == null ? '.' : entry.Value.ToString()[0]);
        }
        Console.WriteLine();
    }
   
    
}
