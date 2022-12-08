using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

var input = File.ReadLines("input.txt");

var root = new Info {
  FileSize = 0,
  Name = "/",
  Parent = null
};

var currentNode = root;
foreach(var line in input.Skip(1)) {
  if(line == "$ ls") continue;

  if (line.StartsWith("$ cd ..")) {
    currentNode = currentNode.Parent;
    continue;
  }
  
  if (line.StartsWith("$ cd")) {
    var node = line[5..];
    currentNode = currentNode.Children.Single(x => x.Name == node);
    continue;
  }
  
  if (line.StartsWith("dir")) {
    var dirName = line[4..];
    currentNode.Children.Add(new Info {Name = dirName, Parent = currentNode});
    continue;
  }

  var match = Regex.Match(line, "(\\d+) (.+)$");
  if (match.Success) {
    var fileSize = int.Parse(match.Groups[1].Value);
    var fileName = match.Groups[2].Value;
    currentNode.Children.Add(new Info {Name = fileName, FileSize = fileSize, Parent = currentNode});
  }
}

var dirs = new List<Info>();
GetDirectories(root, ref dirs);
Console.Out.WriteLine(dirs.Where(x => x.DirSize < 100000).Sum(x => x.DirSize));

var unusedSpace = 70000000 - root.DirSize;
var minSize = 30000000 - unusedSpace;
var toDelete = dirs.Where(x => x.DirSize > minSize).OrderBy(x => x.DirSize).First();
Console.Out.WriteLine(toDelete.DirSize);


void GetDirectories(Info info, ref List<Info> directories) {
  if (info.FileSize == 0) {
    directories.Add(info);
  }
  foreach (var child in info.Children) {
    GetDirectories(child, ref directories);
  }
}

record Info {
  public long FileSize { get; set; } = 0;
  public string Name { get; set; } = string.Empty;
  public Info? Parent { get; set; } = null;
  public IList<Info> Children { get; set; } = new Collection<Info>();
  public long DirSize => CalcDirSize();
  private long CalcDirSize() {
    var sum = 0L;
    foreach (var child in Children) {
      if (child.FileSize == 0) {
        sum += child.DirSize;
      }
      else {
        sum += child.FileSize;
      }
    }
    return sum;
  }
}