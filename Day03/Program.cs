var input = File.ReadAllLines("input.txt")
  .Select(l => l.ToArray())
  .ToList();

var sum = input
  .Select(x => (x.Take(x.Length/2), x.Skip(x.Length/2)))
  .Select(x => x.Item1.Intersect(x.Item2).Single())
  .Select(c => char.IsLower(c) ? Convert.ToInt32(c) - 96 : Convert.ToInt32(c) - 38)
  .Sum();
Console.Out.WriteLine(sum);

var group = input
  .Chunk(3)
  .Select(x => x[0].Intersect(x[1]).Intersect(x[2]).Single())
  .Select(c => char.IsLower(c) ? Convert.ToInt32(c) - 96 : Convert.ToInt32(c) - 38)
  .Sum();
Console.Out.WriteLine(group);  