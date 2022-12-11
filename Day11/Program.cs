var monkeys = ReadInput();
for (var i = 0; i < 20; i++) {
  MonkeyBusiness(monkeys, l => l / 3L);
}
var pt1 = monkeys.OrderByDescending(x => x.Inspections)
  .Take(2).Select(x => x.Inspections)
  .Aggregate((m1, m2) => m1 * m2);
Console.Out.WriteLine(pt1);


monkeys = ReadInput();
var modProduct = monkeys.Select(x => x.Test).Aggregate((m1, m2) => m1 * m2);
for (var i = 0; i < 10000; i++) {
  MonkeyBusiness(monkeys, l => l % modProduct);
}
var pt2 = monkeys.OrderByDescending(x => x.Inspections)
  .Take(2).Select(x => x.Inspections)
  .Aggregate((m1, m2) => m1 * m2);
Console.Out.WriteLine(pt2);


void MonkeyBusiness(IList<Monkey> list, Func<long, long> reduceWorryLevel) {
  foreach (var monkey in list) {
    while (monkey.Items.Any()) {
      var item = monkey.Items[0];
      var level = monkey.Operation(item);
      monkey.Inspections += 1;
      level = reduceWorryLevel(level);
      monkey.Items.RemoveAt(0);
      if (level % monkey.Test == 0) {
        list.Single(m => m.Index == monkey.Success).Items.Add(level);
      }
      else {
        list.Single(m => m.Index == monkey.Failure).Items.Add(level);
      }
    }
  }
}

IList<Monkey> ReadInput() {
  var input = File.ReadAllText("input.txt");
  var inputs = input.Split("\r\n\r\n");

  var result = new List<Monkey>();

  foreach (var m in inputs) {
    var lines = m.Split("\r\n");
    var index = int.Parse(lines[0][7..8]);
    var items = lines[1][18..].Split(",").Select(x => long.Parse(x.Trim())).ToList();
    var op = lines[2][23..24];
    var valueString = lines[2][25..];
    var test = int.Parse(lines[3][21..]);
    var success = int.Parse(lines[4][29..]);
    var failure = int.Parse(lines[5][30..]);

    var monkey = new Monkey {
      Index = index,
      Items = items,
      Test = test,
      Success = success,
      Failure = failure,
      Operation = GetOperation(op, valueString)
    };

    Func<long, long> GetOperation(string o, string v) {
      return o switch {
        "*" => (v == "old") ? x => x * x : x => x * int.Parse(v),
        "+" => (v == "old") ? x => x + x : x => x + int.Parse(v),
        _ => throw new ArgumentOutOfRangeException(nameof(o), o, null)
      };
    }

    result.Add(monkey);
  }
  return result;
}

internal class Monkey {
  public int Index { get; init; }
  public List<long> Items { get; init; } = new();
  public Func<long, long> Operation { get; init; }
  public int Test { get; init; }
  public int Success { get; init; }
  public int Failure { get; init; }
  public long Inspections { get; set; }
}