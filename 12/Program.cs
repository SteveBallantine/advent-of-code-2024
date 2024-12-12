Direction[] directions =
{
    new ("N", 0, -1, "S", 0),
    new ("E", 1, 0, "W", 1),
    new ("S", 0, 1, "N", 2),
    new ("W", -1, 0, "E", 3),
};
var n = directions.Single(d => d.Label == "N");
var s = directions.Single(d => d.Label == "S");
var e = directions.Single(d => d.Label == "E");
var w = directions.Single(d => d.Label == "W");

var exampleInput = @"AAAA
BBCD
BBCC
EEEC";
AssertFor(exampleInput, false, 140);
var exampleInput2 = @"OOOOO
OXOXO
OOOOO
OXOXO
OOOOO";
AssertFor(exampleInput2, false, 772);
var exampleInput3 = @"RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE";
AssertFor(exampleInput3, false, 1930);

//AssertFor(exampleInput, true, 31);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/12/input.txt"), false, true));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/12/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    Map map = new Map(directions, input.Select(str => str.ToCharArray()).ToArray());

    var maxGroupId = IdentifyGroups(map);
    var boundryCounts = CountGroupBoundries(map, maxGroupId);

    var result = 0;
    
    for (int g = 0; g <= maxGroupId; g++)
    {
        if (logging)
        {
            Console.WriteLine($"Group {map.GetLabelForGroup(g)} - size = {map.GetGroupSize(g)} - perimeter = {boundryCounts[g]}");
        }
        result += map.GetGroupSize(g) * boundryCounts[g];
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
    Console.WriteLine();
}

int IdentifyGroups(Map map)
{
    int nextGroupId = 0;
    for (int x = 0; x < map.Width; x++)
    {
        for (int y = 0; y < map.Height; y++)
        {
            var p = new Point(x, y);
            if (map.GetGroupAt(p) == null)
            {
                map.FloodFillGroupAt(p, nextGroupId);
                nextGroupId++;
            }
        }
    }

    return nextGroupId - 1;
}

Dictionary<int, int> CountGroupBoundries(Map map, int maxGroupId)
{
    Dictionary<int, int> boundryCountByGroup = Enumerable.Range(0, maxGroupId + 1).ToDictionary(i => i, i => 0);
    
    for (int x = -1; x < map.Width; x++)
    {
        for (int y = 0; y < map.Height; y++)
        {
            var g1 = map.GetGroupAt(new Point(x + 1, y));
            var g2 = map.GetGroupAt(new Point(x, y));

            if (g1 != g2)
            {
                if (g1.HasValue) { boundryCountByGroup[g1.Value]++; }
                if (g2.HasValue) { boundryCountByGroup[g2.Value]++; }
            }
        }
    }
    
    for (int x = 0; x < map.Width; x++)
    {
        for (int y = -1; y < map.Height; y++)
        {
            var g1 = map.GetGroupAt(new Point(x, y + 1));
            var g2 = map.GetGroupAt(new Point(x, y));

            if (g1 != g2)
            {
                if (g1.HasValue) { boundryCountByGroup[g1.Value]++; }
                if (g2.HasValue) { boundryCountByGroup[g2.Value]++; }
            }
        }
    }

    return boundryCountByGroup;
}


record LocationDirection(Point Location, Direction Direction);
record Direction(string Label, int DeltaX, int DeltaY, string OppositeDirectionLabel, int Index);

class Map
{
    private readonly char[][] _locations;
    private readonly Dictionary<string, Direction> _directions;
    private readonly bool[][] _marked;
    
    private readonly int?[][] _group;
    
    public int Width => _locations[0].Length;
    public int Height => _locations.Length;
    
    public Map(Direction[] directions, char[][] locations)
    {
        _directions = directions.ToDictionary(d => d.Label, d => d);
        _locations = locations;
        _marked = new bool[Height][];
        var width = locations[0].Length;
        if (locations.Any(x => x.Length != width))
        {
            throw new Exception("All lines do not have same width");
        }
        
        _marked = new bool[Height][];
        _group = new int?[Height][];
        for(int i = 0; i < Height; i++)
        {
            _marked[i] = new bool[Width];
            _group[i] = new int?[Width];
        }
    }
    
    public void LogToConsole()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var location = _locations[y][x];
                if (_marked[y][x]) { Console.ForegroundColor = ConsoleColor.Red; }
                Console.Write(location);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public void FloodFillGroupAt(Point p, int groupId)
    {
        HashSet<Point> seen = new HashSet<Point>();
        Queue<Point> toVisit = new Queue<Point>();

        var groupLabel = GetLabelAt(p);
        toVisit.Enqueue(p);
        seen.Add(p);
        
        while(toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();

            if (GetLabelAt(current) == groupLabel)
            {
                _group[current.Y][current.X] = groupId;

                foreach (var direction in _directions.Values)
                {
                    var next = GetNextPointInDirection(direction, current);
                    if (next != null &&
                        !seen.Contains(next))
                    {
                        seen.Add(next);
                        toVisit.Enqueue(next);
                    }
                }
            }
        }
    }
    
    public LocationDirection? Step(LocationDirection start)
    {
        var (location, direction) = start;
    
        var next = GetNextPointInDirection(direction, location);
        if (next == null)
        {
            return null;
        }

        while (GetLabelAt(next) == '#')
        {
            direction = GetNextDirection(direction);
            next = GetNextPointInDirection(direction, location);
            if (next == null)
            {
                return null;
            }
        }

        return new LocationDirection(next, direction);
    }

    public bool[][] GetMarked()
    {
        return _marked;
    }
    
    public void Mark(Point p)
    {
        _marked[p.Y][p.X] = true;
    }
    
    public char? GetLabelAt(Point? p)
    {
        if (p == null) return null;
        return _locations[p.Y][p.X];
    }
    
    public int? GetGroupAt(Point? p)
    {
        if (p == null) return null;
        if (!PointIsInBounds(p)) return null;
        return _group[p.Y][p.X];
    }

    public char GetLabelForGroup(int groupId)
    {
        for (var y = 0; y < _locations.Length; y++)
        {
            for (var x = 0; x < _locations[0].Length; x++)
            {
                if (_group[y][x] == groupId)
                {
                    return _locations[y][x];
                }
            }
        }

        return '-';
    }

    public int GetGroupSize(int groupId)
    {
        return _group.Sum(r => r.Count(g => g == groupId));
    }

    public void SetLabelAt(Point p, char label)
    {
        _locations[p.Y][p.X] = label;
    }
    
    public bool IsMarked(Point p)
    {
        return _marked[p.Y][p.X];
    }
    
    public IEnumerable<Point> FindLabel(char label)
    {
        for (var y = 0; y < _locations.Length; y++)
        {
            for (var x = 0; x < _locations[0].Length; x++)
            {
                if (_locations[y][x] == label)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
    
    public Point? GetNextPointInDirection(Direction d, Point p)
    {
        var nextPoint = new Point(p.X + d.DeltaX, p.Y + d.DeltaY);
        return PointIsInBounds(nextPoint) ? nextPoint : null;
    }
    
    public bool PointIsInBounds(Point p)
    {
        return p.X >= 0 && p.X < _locations[0].Length &&
               p.Y >= 0 && p.Y < _locations.Length;
    }
    
    public Direction? GetNextDirection (Direction d)
    {
        if (d == _directions["N"]) return _directions["E"];
        if (d == _directions["E"]) return _directions["S"];
        if (d == _directions["S"]) return _directions["W"];
        if (d == _directions["W"]) return _directions["N"];
        return null;
    }
}

record Point(int X, int Y);