// See https://aka.ms/new-console-template for more information

var exampleInput = @"2333133121414131402";
AssertFor(exampleInput, false, 1928);
AssertFor(exampleInput, true, 2858);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/09/input.txt"), false, false));

Console.WriteLine("Part2");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/09/input.txt"), true, true));


long RunFor(string[] input, bool part2, bool logging)
{
    DiskMap disk = new DiskMap(input[0]);
    if (part2)
    {
        disk.DefragPart2();
    }
    else
    {
        disk.Defrag();
    }

    if (logging)
    {
        disk.Log();
    }

    return disk.GetChecksum();
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

record FileData(int Position, int Length, int? FileId)
{
    
}

class DiskMap()
{
    private int?[] Disk;
    private List<FileData> FileData;
    
    public DiskMap(string input) : this()
    {
        Disk = input.SelectMany((c, i) =>
        {
            var diskAreaLength = int.Parse(c.ToString());
            int? fileId = i % 2 == 0 ? i / 2 : null;
            return Enumerable.Repeat(fileId, diskAreaLength);
        }).ToArray();

        List<FileData> data = new List<FileData>();
        int? currentFileId = Disk[0];
        int currentAreaStart = 0;
        
        for (int i = 0; i < Disk.Length; i++)
        {
            if (currentFileId != Disk[i])
            {
                data.Add(new FileData(currentAreaStart, i - currentAreaStart, currentFileId));
                currentAreaStart = i;
            }
            currentFileId = Disk[i];
        }
        data.Add(new FileData(currentAreaStart, Disk.Length - currentAreaStart, currentFileId));
        FileData = data;
    }

    public long GetChecksum()
    {
        return Disk.Select((d, i) => (long)((d ?? 0) * i)).Sum();
    }

    public void Defrag()
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

    public void DefragPart2()
    {
        var fileToMove = FileData.Last();
        
        while (fileToMove.FileId > 0)
        {
            var space = GetDataForFirstSpaceWithSize(fileToMove.Length);
            if (space != null &&
                space.Position < fileToMove.Position)
            {
                for (int i = 0; i < fileToMove.Length; i++)
                {
                    Disk[space.Position + i] = Disk[fileToMove.Position + i];
                    Disk[fileToMove.Position + i] = null;
                }

                FileData.Insert(FileData.IndexOf(space), new FileData(space.Position, fileToMove.Length, fileToMove.FileId));
                if (space.Length - fileToMove.Length > 0)
                {
                    FileData.Insert(FileData.IndexOf(space),
                        new FileData(space.Position + fileToMove.Length, space.Length - fileToMove.Length, null));
                }
                FileData.Remove(space);
            }

            fileToMove = FileData.Single(d => d.FileId == fileToMove.FileId - 1);
        }
    }

    private FileData? GetDataForFirstSpaceWithSize(int size)
    {
        return FileData.Find(d => d.FileId == null && d.Length >= size);
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
