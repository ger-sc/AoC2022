using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var mapInput = input[..^2];
var directions = input.Last();

var map = new Dictionary<Position, (char, int)>();
var sideMap = new Dictionary<(int, int), int>();
var index = 1;

for (var y = 1; y <= mapInput.Length; y++) {
  for (var x = 1; x <= mapInput[y - 1].Length; x++) {
    var tile = mapInput[y - 1][x - 1];
    if (tile != ' ') {
      var position = new Position { Row = y, Col = x };
      var sideKey = ((x - 1) / 50, (y - 1) / 50);
      if (!sideMap.ContainsKey(sideKey)) {
        sideMap[sideKey] = index++;
      }

      map[position] = (tile, sideMap[sideKey]);
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

currentDirection = Direction.East;
top = map.Keys.Min(k => k.Row);
currentPosition = new Position { Row = top, Col = map.Where(m => m.Key.Row == top).Min(m => m.Key.Col) };

foreach (var instruction in instructions) {
  if (int.TryParse(instruction, out var steps)) {
    for (var m = 0; m < steps; m++) {
      (currentPosition, currentDirection) = Move2(map, currentPosition, currentDirection);
      map[currentPosition] = (currentDirection switch {
        Direction.East => '>',
        Direction.South => 'V',
        Direction.West => '<',
        Direction.North => 'A',
        _ => throw new ArgumentOutOfRangeException()
      }, map[currentPosition].Item2);
    }
  }
  else {
    currentDirection = Turn(currentDirection, instruction);
  }
}

Console.Out.WriteLine(1000 * currentPosition.Row + 4 * currentPosition.Col + currentDirection);

Position Move(IReadOnlyDictionary<Position, (char, int)> m, Position c, Direction d) {
  Position? next;
  switch (d) {
    case Direction.East:
      next = c with { Col = c.Col + 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Col = m.Where(x => x.Key.Row == c.Row).Min(x => x.Key.Col) };
      }

      return m[next].Item1 == '#' ? c : next;
    case Direction.South:
      next = c with { Row = c.Row + 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Row = m.Where(x => x.Key.Col == c.Col).Min(x => x.Key.Row) };
      }

      return m[next].Item1 == '#' ? c : next;
    case Direction.West:
      next = c with { Col = c.Col - 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Col = m.Where(x => x.Key.Row == c.Row).Max(x => x.Key.Col) };
      }

      return m[next].Item1 == '#' ? c : next;
    case Direction.North:
      next = c with { Row = c.Row - 1 };
      if (!m.ContainsKey(next)) {
        next = c with { Row = m.Where(x => x.Key.Col == c.Col).Max(x => x.Key.Row) };
      }

      return m[next].Item1 == '#' ? c : next;
    default:
      throw new ArgumentOutOfRangeException(nameof(d), d, null);
  }
}

(Position, Direction) Move2(IReadOnlyDictionary<Position, (char, int)> m, Position c, Direction d) {
  Position? next;
  var newDir = d;
  switch (d) {
    case Direction.East:
      next = c with { Col = c.Col + 1 };
      if (!m.ContainsKey(next)) {
        (next, newDir) = GetNextPositionOnDifferentSide(c, d, m[c].Item2);
      }

      return m[next].Item1 == '#' ? (c, d) : (next, newDir);
    case Direction.South:
      next = c with { Row = c.Row + 1 };
      if (!m.ContainsKey(next)) {
        (next, newDir) = GetNextPositionOnDifferentSide(c, d, m[c].Item2);
      }

      return m[next].Item1 == '#' ? (c, d) : (next, newDir);
    case Direction.West:
      next = c with { Col = c.Col - 1 };
      if (!m.ContainsKey(next)) {
        (next, newDir) = GetNextPositionOnDifferentSide(c, d, m[c].Item2);
      }

      return m[next].Item1 == '#' ? (c, d) : (next, newDir);
    case Direction.North:
      next = c with { Row = c.Row - 1 };
      if (!m.ContainsKey(next)) {
        (next, newDir) = GetNextPositionOnDifferentSide(c, d, m[c].Item2);
      }

      return m[next].Item1 == '#' ? (c, d) : (next, newDir);
    default:
      throw new ArgumentOutOfRangeException(nameof(d), d, null);
  }
}

(Position, Direction) GetNextPositionOnDifferentSide(Position p, Direction d, int side) {
  return side switch {
    1 => d switch {
      Direction.East => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.South => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.West => (new Position { Col = 1, Row = 151 - p.Row }, Direction.East),
      Direction.North => (new Position { Col = 1, Row = p.Col + 100 }, Direction.East),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    2 => d switch {
      Direction.East => (new Position { Col = 100, Row = 151 - p.Row }, Direction.West),
      Direction.South => (new Position { Col = 100, Row = p.Col - 50 }, Direction.West),
      Direction.West => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.North => (new Position { Col = p.Col - 100, Row = 200 }, Direction.North),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    3 => d switch {
      Direction.East => (new Position { Col = p.Row + 50, Row = 50 }, Direction.North),
      Direction.South => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.West => (new Position { Row = 101, Col = p.Row - 50 }, Direction.South),
      Direction.North => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    4 => d switch {
      Direction.East => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.South => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.West => (new Position { Col = 51, Row = 151 - p.Row }, Direction.East),
      Direction.North => (new Position { Row = p.Col + 50, Col = 51 }, Direction.East),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    5 => d switch {
      Direction.East => (new Position { Col = 150, Row = 151 - p.Row }, Direction.West),
      Direction.South => (new Position { Col = 50, Row = p.Col + 100 }, Direction.West),
      Direction.West => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      Direction.North => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    6 => d switch {
      Direction.East => (new Position { Col = p.Row - 100, Row = 150 }, Direction.North),
      Direction.South => (new Position { Col = p.Col + 100, Row = 1 }, Direction.South),
      Direction.West => (new Position { Col = p.Row - 100, Row = 1 }, Direction.South),
      Direction.North => throw new ArgumentOutOfRangeException(nameof(d), d, null),
      _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    },
    _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
  };
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