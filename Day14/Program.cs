//Part 1
var map = ReadInput();
var bottom = map.Keys.Max(p => p.Y);
var sand = new Position { X = 500, Y = 0 };
while (sand.Y < bottom) {
    sand = MoveSand();
}
Console.Out.WriteLine(map.Count(kv => kv.Value == 'O'));

//Part2
map = ReadInput();
var start = new Position { X = 500, Y = 0 };
sand = start;
var newBottom = map.Keys.Max(p => p.Y) + 2;
var newBottomPositions = Enumerable.Range(0, 10000)
    .Select(r => new Position { X = r, Y = newBottom });
foreach (var b in newBottomPositions) {
    map.TryAdd(b, '#');
}

while (!map.ContainsKey(start)) {
    sand = MoveSand();
}
Console.Out.WriteLine(map.Count(kv => kv.Value == 'O'));

Position MoveSand() {
    var down = sand with { Y = sand.Y + 1 };
    if (!map.ContainsKey(down)) {
        sand = down;
    }
    else {
        var downLeft = new Position { X = sand.X - 1, Y = sand.Y + 1 };
        if (!map.ContainsKey(downLeft)) {
            sand = downLeft;
        }
        else {
            var downRight = new Position { X = sand.X + 1, Y = sand.Y + 1 };
            if (!map.ContainsKey(downRight)) {
                sand = downRight;
            }
            else {
                map[sand] = 'O';
                sand = new Position { X = 500, Y = 0 };
            }
        }
    }
    return sand;
}

Dictionary<Position, char> ReadInput() {
    var input = File.ReadAllLines("input.txt");
    var dict = new Dictionary<Position, char>();
    foreach (var line in input) {
        var coords = line.Split(" -> ");
        var rocks = coords.Zip(coords.Skip(1)).Select(tuple => {
            var s = tuple.First.Split(",").Select(int.Parse).ToArray();
            var e = tuple.Second.Split(",").Select(int.Parse).ToArray();
            return new { start = new Position { X = s[0], Y = s[1] }, end = new Position { X = e[0], Y = e[1] } };
        }).SelectMany(arg => {
            if (arg.start.X == arg.end.X) {
                var minY = Math.Min(arg.start.Y, arg.end.Y);
                return Enumerable.Range(minY, Math.Abs(arg.end.Y - arg.start.Y) + 1)
                    .Select(y => arg.start with { Y = y });
            }

            var minX = Math.Min(arg.start.X, arg.end.X);
            return Enumerable.Range(minX, Math.Abs(arg.end.X - arg.start.X) + 1)
                .Select(x => arg.start with { X = x });
        });
        foreach (var rock in rocks) {
            dict.TryAdd(rock, '#');
        }
    }
    return dict;
}

record Position {
    public int X { get; init; }
    public int Y { get; init; }
}