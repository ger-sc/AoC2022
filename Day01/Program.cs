var input = File.ReadAllText("input.txt");
var parsed = input.Split("\r\n\r\n")
  .Select((x, i) => new { i, Cal = x.Split("\r\n").Select(int.Parse).Sum() })
  .ToList();
var max = parsed.Max(x => x.Cal);
Console.Out.WriteLine(max);
var top = parsed.OrderByDescending(x => x.Cal).Take(3).Sum(x => x.Cal);
Console.Out.WriteLine(top);
 