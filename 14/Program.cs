var splitChars = new [] { '=', ',', ' ' };
    
var singleRobot1 = @"p=2,4 v=2,-3"; 
AssertFinalPosition(singleRobot1, new Room(11, 7), 1, false, new Point(4, 1));
AssertFinalPosition(singleRobot1, new Room(11, 7), 2, false, new Point(6, 5));
AssertFinalPosition(singleRobot1, new Room(11, 7), 3, false, new Point(8, 2));
AssertFinalPosition(singleRobot1, new Room(11, 7), 4, false, new Point(10, 6));
AssertFinalPosition(singleRobot1, new Room(11, 7), 5, false, new Point(1, 3));
var singleRobot2 = @"p=9,5 v=-3,-3"; 
AssertFinalPosition(singleRobot2, new Room(11, 7), 1, false, new Point(6, 2));
AssertFinalPosition(singleRobot2, new Room(11, 7), 2, false, new Point(3, 6));
AssertFinalPosition(singleRobot2, new Room(11, 7), 3, false, new Point(0, 3));
AssertFinalPosition(singleRobot2, new Room(11, 7), 4, false, new Point(8, 0));
var exampleInput = @"p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3";
AssertFor(exampleInput, new Room(11, 7), false, 12);
//AssertFor(exampleInput, true, 31);

Console.WriteLine("Part1");
Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/14/input.txt"), new Room(101, 103), false, false));

//Console.WriteLine("Part2");
//Console.WriteLine(RunFor(File.ReadAllLines(@"/Users/steveballantine/RiderProjects/advent-of-code-2024/14/input.txt"), true, false));


long RunFor(string[] input, Room room, bool part2, bool logging)
{
    var secondsToRun = 100;

    var robots = Parse(input);
    var finalRobotPositionsByQuadrant = robots
        .Select(r => (r, PostionAfterTime(r, secondsToRun)))
        .Select(p => (p.r, TranslateToRoomSpace(p.Item2, room)))
        .GroupBy(p => GetQuadrant(p.Item2, room));
    
    if (logging)
    {
        foreach (var entry in finalRobotPositionsByQuadrant)
        {
            Console.WriteLine($"Quad {entry.Key}");
            foreach (var (robot, position) in entry)
            {
                Console.WriteLine($"{position.X}, {position.Y} - {robot.Position.X},{robot.Position.Y} - {robot.Velocity.X},{robot.Velocity.Y}");
            }
        }
    }

    long result = 1;
    foreach (var quadrant in finalRobotPositionsByQuadrant.Where(g => g.Key != 0))
    {
        result *= quadrant.Count();
    }
    
    return result;
}

void AssertFor(string input, Room room, bool part2, long expectedResult)
{
    var lines = input.Split(System.Environment.NewLine);
    var result = RunFor(lines, room, part2, true);
    if (result != expectedResult)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        throw new Exception($"Result was {result} but expected {expectedResult}");
    }
}

void AssertFinalPosition(string input, Room room, int secondsToRun, bool part2, Point expected)
{    
    var lines = input.Split(System.Environment.NewLine);
    var robots = Parse(lines);
    var finalRobotPosition = robots
        .Select(r => PostionAfterTime(r, secondsToRun))
        .Select(p => TranslateToRoomSpace(p, room))
        .Single();
    if (finalRobotPosition != expected)
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
        throw new Exception($"Result was {finalRobotPosition.X},{finalRobotPosition.Y} after {secondsToRun} but expected {expected.X},{expected.Y}");
    }
}

int GetQuadrant(Point p, Room r)
{
    if (p.X < r.SplitX)
    {
        if (p.Y < r.SplitY)
        {
            return 1;
        }
        if (p.Y > r.SplitY)
        {
            return 2;
        }
    } 
    if (p.X > r.SplitX)
    {
        if (p.Y < r.SplitY)
        {
            return 3;
        }
        if (p.Y > r.SplitY)
        {
            return 4;
        }
    }
    return 0;
}

Point TranslateToRoomSpace(Point p, Room r)
{
    var x = p.X % r.Width;
    var y = p.Y % r.Height;
    return new Point(p.X >= 0 || x == 0 ? x : r.Width + x, 
        p.Y >= 0 || y == 0 ? y : r.Height + y);
}

Point PostionAfterTime(Robot r, int seconds)
{
    return new Point(r.Position.X + r.Velocity.X * seconds, 
        r.Position.Y + r.Velocity.Y * seconds);
}

Robot[] Parse(string[] input)
{
    return input.Select(GetValues)
        .Select(v => new Robot(new Point(v[0], v[1]), new Point(v[2], v[3])))
        .ToArray();
}

int[] GetValues(string line)
{
    return line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries)
        .Select(x =>
        {
            if (int.TryParse(x, out var v))
            {
                return v;
            }

            return int.MaxValue;
        })
        .Where(x => x != int.MaxValue)
        .ToArray();
}

class Room
{
    public int Width;
    public int Height;

    public int SplitX;
    public int SplitY;
    
    public Room(int width, int height)
    {
        Width = width;
        Height = height;
        SplitX = (int)Math.Floor((double)Width / 2);
        SplitY = (int)Math.Floor((double)Height / 2);
    }
}


record Robot(Point Position, Point Velocity);

record Point(long X, long Y);