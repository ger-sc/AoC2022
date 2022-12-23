var input = File.ReadAllLines("input.txt");

var elves = new List<Position>();

for (var y = 0; y < input.Length; y++) {
  for (var x = 0; x < input[y].Length; x++) {
    if (input[y][x] == '#') {
      elves.Add(new Position {X = x, Y = y});
    }
  }
}

var step = 0;
while (true) {
  var newPositions = new Dictionary<Position, Position>();
  var startDir = step % 4;
  foreach (var elf in elves) {

    var around = GetSurrounding(elf);
    if (!elves.Intersect(around).Any()) {
      newPositions.Add(elf, elf);
    } else {
      for (var dir = 0; dir <= 4; dir++) {
        if (dir == 4) {
          newPositions.Add(elf, elf);
          break;
        }

        var directionToTest = (Direction)((startDir + dir) % 4);
        var newP = GetPossiblePositions(elf, directionToTest).ToList();
        if (!newP.Intersect(elves).Any()) {
          newPositions.Add(elf, Move(elf, directionToTest));
          break;
        }
      }
    }
  }

  var np = newPositions.Values.ToList();
  
  foreach (var cm in np
             .Select(toMove => newPositions
               .Where(x => x.Value == toMove)
               .ToList())
             .Where(cantMove => cantMove.Count > 1)
             .SelectMany(cantMove => cantMove)) {
    newPositions[cm.Key] = cm.Key;
  }

  var newElves = newPositions.Select(x => x.Value).ToList();
  step++;
  if (!newElves.Except(elves).Any()) {
    Console.Out.WriteLine(step);
    break;
  }
  elves = newElves;

  if (step == 10) {
    var yMax = elves.Max(e => e.Y);
    var yMin = elves.Min(e => e.Y);
    var xMax = elves.Max(e => e.X);
    var xMin = elves.Min(e => e.X);
    var area = (yMax - yMin + 1) * (xMax - xMin + 1);
    Console.Out.WriteLine(area-elves.Count);
  }
}

Position Move(Position p, Direction d) {
  return d switch {
    Direction.North => p with { Y = p.Y - 1 },
    Direction.South => p with { Y = p.Y + 1 },
    Direction.West => p with { X = p.X - 1 },
    Direction.East => p with { X = p.X + 1 },
    _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
  };
}

IEnumerable<Position> GetSurrounding(Position p) {
  yield return new Position { Y = p.Y - 1, X = p.X - 1 };
  yield return p with { Y = p.Y - 1 };
  yield return new Position { Y = p.Y - 1, X = p.X + 1 };
  yield return new Position { Y = p.Y + 1, X = p.X - 1 };
  yield return p with { Y = p.Y + 1 };
  yield return new Position { Y = p.Y + 1, X = p.X + 1 };
  yield return p with { X = p.X - 1 };
  yield return p with { X = p.X + 1 };
}

IEnumerable<Position> GetPossiblePositions(Position p, Direction d) {
  switch (d) {
    case Direction.North:
      yield return new Position { Y = p.Y - 1, X = p.X - 1 };
      yield return p with { Y = p.Y - 1 };
      yield return new Position { Y = p.Y - 1, X = p.X + 1 };
      break;
    case Direction.South:
      yield return new Position { Y = p.Y + 1, X = p.X - 1 };
      yield return p with { Y = p.Y + 1 };
      yield return new Position { Y = p.Y + 1, X = p.X + 1 };
      break;
    case Direction.West:
      yield return new Position { Y = p.Y - 1, X = p.X - 1 };
      yield return p with { X = p.X - 1 };
      yield return new Position { Y = p.Y + 1, X = p.X - 1 };
      break;
    case Direction.East:
      yield return new Position { Y = p.Y - 1, X = p.X + 1 };
      yield return p with { X = p.X + 1 };
      yield return new Position { Y = p.Y + 1, X = p.X + 1 };
      break;
    default:
      throw new ArgumentOutOfRangeException(nameof(d), d, null);
  }
}

internal record Position {
  public int X { get; init; }
  public int Y { get; init; }
}

internal enum Direction {
  North = 0,
  South = 1,
  West = 2,
  East = 3
}