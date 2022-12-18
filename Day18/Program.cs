var input = File.ReadAllLines("input.txt");

var dropletPositions = input
  .Select(c => c.Split(','))
  .Select(c => new CubePosition { X = int.Parse(c[0]), Y = int.Parse(c[1]), Z = int.Parse(c[2]) })
  .ToList();

Console.Out.WriteLine(dropletPositions.SelectMany(GetNeighbours).Count(neighbor => !dropletPositions.Contains(neighbor)));

var minX = dropletPositions.Min(d => d.X);
var maxX = dropletPositions.Max(d => d.X);
var minY = dropletPositions.Min(d => d.Y);
var maxY = dropletPositions.Max(d => d.Y);
var minZ = dropletPositions.Min(d => d.Z);
var maxZ = dropletPositions.Max(d => d.Z);

var hull = 
  (from x in Enumerable.Range(minX - 1, maxX - minX + 3)
  from y in Enumerable.Range(minY - 1, maxY - minY + 3)
  from z in Enumerable.Range(minZ - 1, maxZ - minZ + 3)
  select new CubePosition { X = x, Y = y, Z = z }).ToList();

var visited = new HashSet<CubePosition>();
var todo = new Queue<CubePosition>();
visited.Add(hull.First());
todo.Enqueue(hull.First());

while (todo.TryDequeue(out var cube)) {
  foreach (var next in GetNeighbours(cube)
             .Where(c => !dropletPositions.Contains(c) && !visited.Contains(c) && hull.Contains(c))) {
    todo.Enqueue(next);
    visited.Add(next);
  }
}

Console.Out.WriteLine(dropletPositions.Select(GetNeighbours).Select(neighbors => visited.Count(neighbors.Contains)).Sum());

IEnumerable<CubePosition> GetNeighbours(CubePosition pos) {
  yield return pos with { X = pos.X + 1 };
  yield return pos with { X = pos.X - 1 };
  yield return pos with { Y = pos.Y + 1 };
  yield return pos with { Y = pos.Y - 1 };
  yield return pos with { Z = pos.Z + 1 };
  yield return pos with { Z = pos.Z - 1 };
}

internal record CubePosition {
  public int X { get; init; }
  public int Y { get; init; }
  public int Z { get; init; }
}  