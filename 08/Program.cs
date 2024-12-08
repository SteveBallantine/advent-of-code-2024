using System.Text;

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

var exampleInput = @"............
........0...
.....0......
.......0....
....0.......
......A.....
............
............
........A...
.........A..
............
............";
AssertFor(exampleInput, false, 14);
//AssertFor(exampleInput, true, 34);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/08/input.txt"), false, false));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/08/input.txt"), true, false));


long RunFor(string[] input, bool part2, bool logging)
{
    Map map = new Map(directions, input.Select(str => str.ToCharArray()).ToArray());
    var antennaFrequencies = input
        .SelectMany(l => l.Select(c => c))
        .Distinct()
        .Where(c => c!= '.')
        .ToArray();

    foreach (var frequency in antennaFrequencies)
    {
        MarkAntinodesFor(frequency, map);
    }
    
    return map.GetMarked().Sum(row => row.Count(marked => marked));
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

void MarkAntinodesFor(char frequency, Map map)
{
    var antennaLocations = map.FindLabel(frequency).ToArray();
    foreach (var antenna in antennaLocations)
    {
        foreach (var otherAntenna in antennaLocations.Where(l => l != antenna))
        {
            var dX = (antenna.X - otherAntenna.X) * -2;
            var dY = (antenna.Y - otherAntenna.Y) * -2;

            var antinode = new Point(antenna.X + dX, antenna.Y + dY);
            if (map.PointIsInBounds(antinode))
            {
                map.Mark(antinode);
            }
        }
    }
}


record LocationDirection(Point Location, Direction Direction);
record Direction(string Label, int DeltaX, int DeltaY, string OppositeDirectionLabel, int Index);

class Map
{
    private readonly char[][] _locations;
    private readonly Dictionary<string, Direction> _directions;
    private readonly bool[][] _marked;
    
    private readonly bool[][] _obstacle;
    
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
        _obstacle = new bool[Height][];
        for(int i = 0; i < Height; i++)
        {
            _marked[i] = new bool[Width];
            _obstacle[i] = new bool[Width];
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
                if (_obstacle[y][x]) { location = 'O'; }
                Console.Write(location);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.WriteLine();
        }
        Console.WriteLine();
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
    
    public void SetObstacleAt(Point p)
    {
        _obstacle[p.Y][p.X] = true;
    }

    public int ObstacleCount => _obstacle.Sum(row => row.Count(o => o));

    public char? GetLabelAt(Point? p)
    {
        if (p == null) return null;
        return _locations[p.Y][p.X];
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

record PointPair(Point A, Point B, Direction DirectionFromAToB);

record Point(int X, int Y);