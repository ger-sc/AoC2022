var input = File.ReadLines("input.txt").ToArray();

var terrain = new List<Terrain>();

for (var y = 0; y < input.Length; y++) {
    for (var x = 0; x < input[y].Length; x++) {
        var c = input[y][x];
        var h = c switch {
            'E' => 122,
            'S' => 97,
            _ => c
        };
        terrain.Add(new Terrain {X = x, Y = y, Height = h - 97, OriginalValue = c});
    }
}

var start = terrain.Single(t => t.OriginalValue == 'S');
var end = terrain.Single(t => t.OriginalValue == 'E');

var paths = new List<List<Terrain>>();
var visitLog = new Dictionary<Terrain, int>();

FindPaths(start, terrain, new List<Terrain>(), ref visitLog,end, ref paths);

var minPath = paths.OrderBy(x => x.Count).First();
Console.Out.WriteLine(minPath.Count);

//Part 2

var possibleAs = terrain.Where(p => p.X == 0 && p.OriginalValue == 'a');

paths = new List<List<Terrain>>();
foreach (var a in possibleAs) {
    FindPaths(a, terrain, new List<Terrain>(), ref visitLog,end, ref paths);
}

minPath = paths.OrderBy(x => x.Count).First();
Console.Out.WriteLine(minPath.Count);

void FindPaths(Terrain root, List<Terrain> map, ICollection<Terrain> path, ref Dictionary<Terrain, int> visited, Terrain peak, ref List<List<Terrain>> results) {
    if (visited.ContainsKey(root) && visited[root] <= path.Count) return;
    visited[root] = path.Count;
    var adjacentPaths = FindAdjacent(root, map).ToList();
    if (adjacentPaths.Contains(peak)) {
        results.Add(path.Concat(new []{peak}).ToList());
        return;
    }
    foreach (var possiblePaths in adjacentPaths.Where(p => !path.Contains(p))) {
        FindPaths(possiblePaths, map, path.Concat(new []{possiblePaths}).ToList(), ref visited, peak, ref results);
    }
}

IEnumerable<Terrain> FindAdjacent(Terrain center, List<Terrain> map) {
    var up = map.SingleOrDefault(t => t.X == center.X && t.Y + 1 == center.Y);
    var down = map.SingleOrDefault(t => t.X == center.X && t.Y - 1 == center.Y);
    var left = map.SingleOrDefault(t => t.X + 1 == center.X && t.Y == center.Y);
    var right = map.SingleOrDefault(t => t.X - 1 == center.X && t.Y == center.Y);
    if (up != null && up.Height - 1 <= center.Height) yield return up;
    if (down != null && down.Height - 1 <= center.Height) yield return down;
    if (left != null && left.Height - 1 <= center.Height) yield return left;
    if (right != null && right.Height - 1 <= center.Height) yield return right;
}

record Terrain {
    public int Y { get; init; }
    public int X { get; init; }
    public int Height { get; init; }
    public char OriginalValue { get; init; }
}