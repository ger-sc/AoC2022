var input = File.ReadAllLines("input.txt")
  .Select(x => x.Split(" "))
  .ToList();

var scorePlayer = 0;
foreach (var g in input) {
  var opponent = GetShape(g[0]);
  var player = GetShape(g[1]);
  var pl = CalcScore(opponent, player);
  scorePlayer += pl;
}

Console.Out.WriteLine(scorePlayer);

scorePlayer = 0;
foreach (var g in input) {
  var opponent = GetShape(g[0]);
  var player = GetRightShape(opponent, g[1]);
  var pl = CalcScore(opponent, player);
  scorePlayer += pl;
}

Console.Out.WriteLine(scorePlayer);

Shape GetRightShape(Shape opponent, string shape) {
  return shape switch {
    "X" => opponent switch {
      Shape.Rock => Shape.Scissors,
      Shape.Paper => Shape.Rock,
      Shape.Scissors => Shape.Paper,
      _ => throw new ArgumentOutOfRangeException(nameof(opponent), opponent, null)
    },
    "Y" => opponent switch {
      Shape.Rock => Shape.Rock,
      Shape.Paper => Shape.Paper,
      Shape.Scissors => Shape.Scissors,
      _ => throw new ArgumentOutOfRangeException(nameof(opponent), opponent, null)
    },
    "Z" => opponent switch {
      Shape.Rock => Shape.Paper,
      Shape.Paper => Shape.Scissors,
      Shape.Scissors => Shape.Rock,
      _ => throw new ArgumentOutOfRangeException(nameof(opponent), opponent, null)
    },
    _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
  };
}

int CalcScore(Shape opp, Shape pl) {
  return opp switch {
    Shape.Rock =>
      pl switch {
        Shape.Rock => (int)Shape.Rock + 3,
        Shape.Paper => (int)Shape.Paper + 6,
        Shape.Scissors => (int)Shape.Scissors,
        _ => throw new ArgumentOutOfRangeException(nameof(pl), pl, null)
      },
    Shape.Paper =>
      pl switch {
        Shape.Rock => (int)Shape.Rock,
        Shape.Paper => (int)Shape.Paper + 3,
        Shape.Scissors => (int)Shape.Scissors + 6,
        _ => throw new ArgumentOutOfRangeException(nameof(pl), pl, null)
      },
    Shape.Scissors =>
      pl switch {
        Shape.Rock => (int)Shape.Rock + 6,
        Shape.Paper => (int)Shape.Paper,
        Shape.Scissors => (int)Shape.Scissors + 3,
        _ => throw new ArgumentOutOfRangeException(nameof(pl), pl, null)
      },
    _ => throw new ArgumentOutOfRangeException(nameof(opp), opp, null)
  };
}

Shape GetShape(string shape) {
  return shape switch {
    "A" => Shape.Rock,
    "X" => Shape.Rock,
    "B" => Shape.Paper,
    "Y" => Shape.Paper,
    "C" => Shape.Scissors,
    "Z" => Shape.Scissors,
    _ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null)
  };
}

internal enum Shape {
  Rock = 1,
  Paper = 2,
  Scissors = 3
}