var input = File.ReadLines("input.txt").ToList();

var trees = new List<Tree>();
for (var y = 0; y < input.Count; y++) {
  for (var x = 0; x < input[y].Length; x++) {
    trees.Add(new Tree {X = x, Y = y, Height = int.Parse(input[y][x].ToString())});
  }
}

var pt1 = 0;
foreach (var tree in trees) {
  var x = tree.X;
  var y = tree.Y;
  var height = tree.Height;
  var visible = trees.Where(t => t.X == x && t.Y < y).All(t => t.Height < height)
                || trees.Where(t => t.X == x && t.Y > y).All(t => t.Height < height)
                || trees.Where(t => t.X < x && t.Y == y).All(t => t.Height < height)
                || trees.Where(t => t.X > x && t.Y == y).All(t => t.Height < height);
  if (visible) pt1++;
}
Console.Out.WriteLine(pt1);

var pt2 = 0L;
foreach (var tree in trees) {
  var x = tree.X;
  var y = tree.Y;
  var height = tree.Height;

  var up = y - trees.Where(t => t.X == x && t.Y < y).OrderByDescending(t => t.Y).FirstOrDefault(t => t.Height >= height)?.Y ?? y;
  var down = trees.Where(t => t.X == x && t.Y > y).OrderBy(t => t.Y).FirstOrDefault(t => t.Height >= height)?.Y - y ?? trees.Max(t => t.Y) - y;
  var left = x - trees.Where(t => t.X < x && t.Y == y).OrderByDescending(t => t.X).FirstOrDefault(t => t.Height >= height)?.X ?? x;
  var right = trees.Where(t => t.X > x && t.Y == y).OrderBy(t => t.X).FirstOrDefault(t => t.Height >= height)?.X - x ?? trees.Max(t => t.X) - x;
  
  var score = up * down * left * right * 1L;
  if (score > pt2) {
    pt2 = score;
  }
}
Console.Out.WriteLine(pt2);

internal record Tree {
  public int Y { get; init; }
  public int X { get; init; }
  public int Height { get; init; }
}