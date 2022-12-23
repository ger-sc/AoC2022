using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var mapInput = input[..^2];
var directions = input.Last();

var map = new Dictionary<Position, char>();

for (var y = 1; y <= mapInput.Length; y++) {
  for (var x = 1; x <= mapInput[y-1].Length; x++) {
    var tile = mapInput[y - 1][x - 1];
    if (tile != ' ') {
      map[new Position { Row = y, Col = x }] = tile;
    }
  }
}

const string regexPattern = @"(\d+)|(R|L)";
var match = Regex.Matches(directions, regexPattern);
var instructions = match.Select(m => m.Value).ToList();
var currentDirection = Direction.East;
var top = map.Keys.Min(k => k.Row);
var currentPosition = new Position { Row = top, Col = map.Where(m => m.Key.Row == top).Min(m => m.Key.Col) };

foreach (var instruction in instructions) {
  if (int.TryParse(instruction, out var steps)) {
    for (var m = 0; m < steps; m++) {
      currentPosition = Move(map, currentPosition, currentDirection);
    }
  }
  else {
    currentDirection = Turn(currentDirection, instruction);
  }
}

Console.Out.WriteLine(1000 * currentPosition.Row + 4 * currentPosition.Col + currentDirection);

Position Move(IReadOnlyDictionary<Position, char> m, Position c, Direction d) {
  Position? next;
  switch (d) {
    case Direction.East:
      next = c with { Col = c.Col + 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Col = m.Where(x => x.Key.Row == c.Row).Min(x => x.Key.Col) };
      }
      return m[next] == '#' ? c : next;
    case Direction.South:
      next = c with { Row = c.Row + 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Row = m.Where(x => x.Key.Col == c.Col).Min(x => x.Key.Row) };
      }
      return m[next] == '#' ? c : next;
    case Direction.West:
      next = c with { Col = c.Col - 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Col = m.Where(x => x.Key.Row == c.Row).Max(x => x.Key.Col) };
      }
      return m[next] == '#' ? c : next;
    case Direction.North:
      next = c with { Row = c.Row - 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Row = m.Where(x => x.Key.Col == c.Col).Max(x => x.Key.Row) };
      }
      return m[next] == '#' ? c : next;
    default:
      throw new ArgumentOutOfRangeException(nameof(d), d, null);
  }
}

Direction Turn(Direction now, string turn) {
  return now switch {
    Direction.East => turn == "R" ? Direction.South : Direction.North,
    Direction.South => turn == "R" ? Direction.West : Direction.East,
    Direction.West => turn == "R" ? Direction.North : Direction.South,
    Direction.North => turn == "R" ? Direction.East : Direction.West,
    _ => throw new ArgumentOutOfRangeException(nameof(now), now, null)
  };
}

internal record Position {
  public int Row { get; init; }
  public int Col { get; init; }
}

internal enum Direction {
  East = 0,
  South = 1,
  West = 2,
  North = 3
}