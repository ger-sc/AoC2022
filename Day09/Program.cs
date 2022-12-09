using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

// Part1
var head = new Position { X = 0, Y = 0 };
var tail = new Position { X = 0, Y = 0 };
var tailPositions = new HashSet<Position>();
foreach (var line in input) {
  var match = Regex.Match(line, "([A-Z]) (\\d+)");
  var direction = match.Groups[1].Value;
  var distance = int.Parse(match.Groups[2].Value);
  for (var i = 0; i < distance; i++) {
    head = MoveHead(head, direction);
    tail = CalcTail(head, tail);
    tailPositions.Add(tail);
  }
}
Console.Out.WriteLine(tailPositions.Count);

//Part2
head = new Position { X = 0, Y = 0 };
tailPositions = new HashSet<Position>();
var tails = new Position[9];
for (var i = 0; i < tails.Length; i++) {
  tails[i] = new Position { X = 0, Y = 0};
}

foreach (var line in input) {
  var match = Regex.Match(line, "([A-Z]) (\\d+)");
  var direction = match.Groups[1].Value;
  var distance = int.Parse(match.Groups[2].Value);
  for (var i = 0; i < distance; i++) {
    head = MoveHead(head, direction);
    for (var ti = 0; ti < tails.Length; ti++) {
      tails[ti] = CalcTail(ti == 0 ? head : tails[ti - 1], tails[ti]);
    }
    tailPositions.Add(tails[8]);
  }
}
Console.Out.WriteLine(tailPositions.Count);

Position MoveHead(Position h, string d) {
  return d switch {
    "U" => h with { Y = h.Y - 1 },
    "D" => h with { Y = h.Y + 1 },
    "L" => h with { X = h.X - 1 },
    "R" => h with { X = h.X + 1 },
    _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
  };
}

Position CalcTail(Position h, Position t) {
  if (IsAdjacent(h, t)) {
    return t;
  }

  var coordinates = (
    from y in Enumerable.Range(h.Y - 1, 3)
    from x in Enumerable.Range(h.X - 1, 3)
    select new Position { X = x, Y = y }
    )
    .ToList();
  
  var calcTail = coordinates
    .Where(x => Distance(h, x) == 1)
    .Select(x => new { x, dist = Distance(t, x) })
    .OrderBy(x => x.dist)
    .First()
    .x;

  if (Distance(calcTail, t) > 2) {
    calcTail = coordinates
      .Select(x => new { x, dist = Distance(t, x) })
      .OrderBy(x => x.dist)
      .First()
      .x;
  }
  
  return calcTail;
}

bool IsAdjacent(Position h, Position t) {
  var coordinates = 
    from y in Enumerable.Range(h.Y - 1, 3)
    from x in Enumerable.Range(h.X - 1, 3)
    select new Position { X = x, Y = y };
  return coordinates.Contains(t);
}

decimal Distance(Position p1, Position p2) {
  return (decimal)Math.Sqrt(Math.Pow(Math.Abs(p1.Y - p2.Y),2) + Math.Pow(Math.Abs(p1.X - p2.X), 2));
}

internal record Position {
  public int X { get; init; }
  public int Y { get; init; }
}