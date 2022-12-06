var input = File.ReadAllLines("input.txt");
var stacks = new Stack<string>[10];
var stacks2 = new Stack<string>[10];
for (var i = 0; i < stacks.Length; i++) {
  stacks[i] = new Stack<string>();
  stacks2[i] = new Stack<string>();
}
foreach (var line in input[0..8].Reverse()) {
  for (var i = 0; i < 9; i++) {
    var start = i * 4 + 1;
    var crate = line.Substring(start, 1);
    if (!string.IsNullOrWhiteSpace(crate)) {
      stacks[i+1].Push(crate);
      stacks2[i+1].Push(crate);
    }
  }
}
var moves = input[10..]
  .Select(x => x.Split(" "))
  .Select(x => (int.Parse(x[1]), int.Parse(x[3]), int.Parse(x[5])))
  .ToList();
foreach (var (amount, from, to) in moves) {
  for (var i = 0; i < amount; i++) {
    var crate = stacks[from].Pop();
    stacks[to].Push(crate);
  }
}
var pt1 = string.Join("", stacks[1..10].Select(x => x.Peek()));
Console.Out.WriteLine(pt1);
foreach (var (amount, from, to) in moves) {
  var buffer = new List<string>();
  for (var i = 0; i < amount; i++) {
    var crate = stacks2[from].Pop();
    buffer.Add(crate);
  }
  buffer.Reverse();
  foreach (var crate in buffer) {
    stacks2[to].Push(crate);
  }
}
var pt2 = string.Join("", stacks2[1..10].Select(x => x.Peek()));
Console.Out.WriteLine(pt2);