Direction[] directions =
{
    new ("N", 0, -1, "S"),
    new ("S", 0, 1, "N"),
    new ("E", 1, 0, "W"),
    new ("W", -1, 0, "E"),
    new ("NW", -1, -1, "SE"),
    new ("NE", 1, -1, "SW"),
    new ("SW", -1, 1, "NE"),
    new ("SE", 1, 1, "NW"),
};
var n = directions.Single(d => d.Label == "N");
var s = directions.Single(d => d.Label == "S");
var e = directions.Single(d => d.Label == "E");
var w = directions.Single(d => d.Label == "W");
var nw = directions.Single(d => d.Label == "NW");
var ne = directions.Single(d => d.Label == "NE");
var sw = directions.Single(d => d.Label == "SW");
var se = directions.Single(d => d.Label == "SE");

var exampleInput = @"MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX";
AssertFor(exampleInput, false, 18);
AssertFor(exampleInput, true, 9);

Console.WriteLine("Part1:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/04/input.txt"), false, false));

Console.WriteLine("Part2:");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/04/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    var result = 0;
    
    Map map = new Map(directions, input.Select(s => s.ToCharArray()).ToArray());

    if (part2)
    {
        var starts = map.FindLabel('A');
        result = starts.Count(s => { return IsXmas2(map, s); });
    }
    else
    {
        var starts = map.FindLabel('X').SelectMany(p => { return directions.Select(d => new LocationVector(p, d)); });

        result = starts.Count(s => { return IsXmas(map, s); });
    }

    if (logging)
    {
        map.LogToConsole();
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

bool IsXmas2(Map map, Point start)
{
    var lNw = map.GetLabelAt(map.GetNextPointInDirection(nw, start)?.B);
    var lNe = map.GetLabelAt(map.GetNextPointInDirection(ne, start)?.B);
    var lSw = map.GetLabelAt(map.GetNextPointInDirection(sw, start)?.B);
    var lSe = map.GetLabelAt(map.GetNextPointInDirection(se, start)?.B);

    var result = lNw != null && lNe != null && lSw != null && lSe != null &&
                 (lNw == 'M' || lNw == 'S') && (lSe == 'M' || lSe == 'S') && lNw != lSe &&
                 (lNe == 'M' || lNe == 'S') && (lSw == 'M' || lSw == 'S') && lNe != lSw;
    if (result)
    {
        map.Mark(start);
    }
    return result;
}


bool IsXmas(Map map, LocationVector start)
{
    var target = "XMAS";
    var count = 0;
    var currentPoint = start.Location;
    while (map.GetLabelAt(currentPoint) == target[count])
    {
        if (count == target.Length - 1)
        {
            break;
        }
        var nextPoint = map.GetNextPointInDirection(start.Direction, currentPoint);
        if (nextPoint == null) break;
        currentPoint = nextPoint.B;
        count++;
    }

    var result = count == target.Length - 1 && map.GetLabelAt(currentPoint) == target[count];
    if (result)
    {
        map.Mark(start.Location);
        var next = map.GetNextPointInDirection(start.Direction, start.Location);
        map.Mark(next.B);
        next = map.GetNextPointInDirection(start.Direction, next.B);
        map.Mark(next.B);
        next = map.GetNextPointInDirection(start.Direction, next.B);
        map.Mark(next.B);
    }
    return result;
}


record LocationVector(Point Location, Direction Direction);
record Direction(string Label, int DeltaX, int DeltaY, string OppositeDirectionLabel);


class Map
{
    private readonly char[][] _locations;
    private readonly Dictionary<string, Direction> _directions;
    private readonly bool[][] _marked;
    
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
        for(int i = 0; i < Height; i++)
        {
            _marked[i] = new bool[Width];
        }
    }
    
    public void LogToConsole()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var location = _locations[y][x];
                if(_marked[y][x]) { Console.ForegroundColor = ConsoleColor.Red; }
                Console.Write(location);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();
        }
        Console.WriteLine();
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
    
    public PointPair? GetNextPointInDirection(Direction d, Point p)
    {
        var nextPoint = new Point(p.X + d.DeltaX, p.Y + d.DeltaY);
        return PointIsInBounds(nextPoint) ? new PointPair(p, nextPoint, d) : null;
    }
    
    private bool PointIsInBounds(Point p)
    {
        return p.X >= 0 && p.X < _locations[0].Length &&
               p.Y >= 0 && p.Y < _locations.Length;
    }
    
    private IEnumerable<PointPair> GetAdjacentPoints(Point p)
    {
        return _directions.Select(d => GetNextPointInDirection(d.Value, p)).Where(x => x is not null);
    }
}

record PointPair(Point A, Point B, Direction DirectionFromAToB);

record Point(int X, int Y);