var input = File.ReadAllLines("input.txt");

var blizzards = new List<Blizzard>();

for (var y = 0; y < input.Length; y++) {
  for (var x = 0; x < input[y].Length; x++) {
    var c = input[y][x];
    switch (c) {
      case '^':
        blizzards.Add(new Blizzard {
          Position = new Position{X = x,Y = y},
          Change = new Position {X=0, Y = -1}
        });
        break;
      case '>':
        blizzards.Add(new Blizzard {
          Position = new Position{X = x,Y = y},
          Change = new Position {X=1, Y = 0}
        });
        break;
      case '<':
        blizzards.Add(new Blizzard {
          Position = new Position{X = x,Y = y},
          Change = new Position {X=-1, Y = 0}
        });
        break;
      case 'v':
        blizzards.Add(new Blizzard {
          Position = new Position{X = x,Y = y},
          Change = new Position {X=0, Y = +1}
        });
        break;
    }
  }
}

var yMax = input.Length - 1;
var xMax = input[0].Length - 1;

var iterations = GenerateMap(blizzards);
using var iterator = iterations.GetEnumerator();


var mapCache = new Dictionary<int, List<Blizzard>> { { 0, blizzards } };
for (var i = 1; i < 1000; i++) {
  iterator.MoveNext();
  mapCache.Add(i, iterator.Current);
}

var visited = new HashSet<(int, Position)>();
var queue = new PriorityQueue<(int, Position), int>();
var start = new Position { X = 1, Y = 0 };
var goal = new Position { X = xMax - 1, Y = yMax };
queue.Enqueue((0, start), 0);
var pt1 = FindAWay();

Console.Out.WriteLine(pt1);

(goal, start) = (start, goal);
queue = new PriorityQueue<(int, Position), int>();
queue.Enqueue((pt1, start), 0);
var back = FindAWay();
(goal, start) = (start, goal);
queue = new PriorityQueue<(int, Position), int>();
queue.Enqueue((back, start), 0);
var pt2 = FindAWay();

Console.Out.WriteLine(pt2);

int FindAWay() {
  while (true) {
    var q = queue.Dequeue();
    if (q.Item2 == goal) return q.Item1;
    if (visited.Contains(q)) continue;
    visited.Add(q);
    foreach (var move in GetPossibleMoves(q.Item2, mapCache[q.Item1 + 1]).Where(m => !visited.Contains((q.Item1 + 1, m)))) {
      queue.Enqueue((q.Item1 + 1, move), q.Item1 + 1);
    }
  }
}

IEnumerable<Position> GetPossibleMoves(Position now, IList<Blizzard> map) {
  //Right
  var right = now with { X = now.X + 1 };
  if (!map.Select(x => x.Position).Contains(right) && !IsOutOfBounds(right)) {
    yield return right;
  }
  //Down & Goal
  var down = now with { Y = now.Y + 1 };
  if (down == goal) {
    yield return down;
  }
  if (!map.Select(x => x.Position).Contains(down) && !IsOutOfBounds(down)) {
    yield return down;
  }
  //Up
  var up = now with { Y = now.Y - 1 };
  if (up == goal) {
    yield return up;
  }
  if (!map.Select(x => x.Position).Contains(up) && !IsOutOfBounds(up)) {
    yield return up;
  }
  //Left
  var left = now with { X = now.X - 1 };
  if (!map.Select(x => x.Position).Contains(left) && !IsOutOfBounds(left)) {
    yield return left;
  }
  //Wait
  if (!map.Select(x => x.Position).Contains(now)) {
    yield return now;
  }
}

bool IsOutOfBounds(Position p) {
  return p.X < 1 || p.X >= xMax || p.Y < 1 || p.Y >= yMax;
}

IEnumerable<List<Blizzard>> GenerateMap(List<Blizzard> begin) {
  var generateNewMap = begin;
  var count = 0;
  while (count < 10000) {
    var nm = new List<Blizzard>();
    foreach (var b in generateNewMap) {
      var nb = b with { Position = new Position { X = b.Position.X + b.Change.X, Y = b.Position.Y + b.Change.Y } };
      if (nb.Position.X <= 0) nb = nb with { Position = b.Position with { X = xMax - 1 } };
      if (nb.Position.X >= xMax) nb = nb with { Position = b.Position with { X = 1 } };
      if (nb.Position.Y <= 0) nb = nb with { Position = b.Position with { Y = yMax - 1 } };
      if (nb.Position.Y >= yMax) nb = nb with { Position = b.Position with { Y = 1 } };
      nm.Add(nb); 
    }
    generateNewMap = nm;
    yield return generateNewMap;
    count++;
  }
}

internal record Blizzard {
  public Position Position { get; init; }
  public Position Change { get; init; }
}

internal record Position {
  public int X { get; init; }
  public int Y { get; init; }
}