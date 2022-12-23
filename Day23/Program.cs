var input = File.ReadAllLines("input.txt");

var elves = new List<(int,int)>();

for (var y = 0; y < input.Length; y++) {
  for (var x = 0; x < input[y].Length; x++) {
    if (input[y][x] == '#') {
      elves.Add((x, y));
    }
  }
}

var step = 0;
while (true) {
  var newPositions = new Dictionary<(int, int), (int, int)>();

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

        var directionToTest = (Direction)((step % 4 + dir) % 4);
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
    var yMax = elves.Max(e => e.Item2);
    var yMin = elves.Min(e => e.Item2);
    var xMax = elves.Max(e => e.Item1);
    var xMin = elves.Min(e => e.Item1);
    var area = (yMax - yMin + 1) * (xMax - xMin + 1);
    Console.Out.WriteLine(area-elves.Count);
  }
}

(int,int) Move((int,int) p, Direction d) {
  return d switch {
    Direction.North => (p.Item1, p.Item2 - 1),
    Direction.South => (p.Item1, p.Item2 + 1),
    Direction.West => (p.Item1 - 1, p.Item2),
    Direction.East => (p.Item1 + 1, p.Item2),
    _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
  };
}

IEnumerable<(int, int)> GetSurrounding((int, int) p) {
  yield return (p.Item1 - 1, p.Item2 - 1);
  yield return (p.Item1, p.Item2 - 1);
  yield return (p.Item1 + 1, p.Item2 - 1);
  yield return (p.Item1 - 1, p.Item2 + 1);
  yield return (p.Item1, p.Item2 + 1);
  yield return (p.Item1 + 1, p.Item2 + 1);
  yield return (p.Item1 - 1, p.Item2);
  yield return (p.Item1 + 1, p.Item2);
}

IEnumerable<(int, int)> GetPossiblePositions((int, int) p, Direction d) {
  switch (d) {
    case Direction.North:
      yield return (p.Item1 - 1, p.Item2 - 1); 
      yield return (p.Item1, p.Item2 - 1); 
      yield return (p.Item1 + 1, p.Item2 - 1); 
      break;
    case Direction.South:
      yield return (p.Item1 - 1, p.Item2 + 1); 
      yield return (p.Item1, p.Item2 + 1); 
      yield return (p.Item1 + 1, p.Item2 + 1); 
      break;
    case Direction.West:
      yield return (p.Item1 - 1, p.Item2 - 1); 
      yield return (p.Item1 - 1, p.Item2);
      yield return (p.Item1 - 1, p.Item2 + 1);
      break;
    case Direction.East:
      yield return (p.Item1 + 1, p.Item2 - 1); 
      yield return (p.Item1 + 1, p.Item2);
      yield return (p.Item1 + 1, p.Item2 + 1); 
      break;
    default:
      throw new ArgumentOutOfRangeException(nameof(d), d, null);
  }
}

internal enum Direction {
  North = 0,
  South = 1,
  West = 2,
  East = 3
}