var input = File.ReadAllLines("input.txt")
  .Select(x => x.Split(","))
  .Select(x => {
    var f = x[0].Split("-").Select(int.Parse).ToArray();
    var s = x[1].Split("-").Select(int.Parse).ToArray();
    return (f[0]..f[1], s[0]..s[1]);
  })
  .ToList();

var pt1 = input.Count(tuple => Contains(tuple.Item1, tuple.Item2));
Console.Out.WriteLine(pt1);
var pt2 = input.Count(tuple => Overlap(tuple.Item1, tuple.Item2));
Console.Out.WriteLine(pt2);

bool Contains(Range r1, Range r2) {
  return (r1.Start.Value >= r2.Start.Value && r1.End.Value <= r2.End.Value)
         || (r2.Start.Value >= r1.Start.Value && r2.End.Value <= r1.End.Value);
}

bool Overlap(Range r1, Range r2) {
  return r1.Start.Value <= r2.End.Value && r2.Start.Value <= r1.End.Value;
}
