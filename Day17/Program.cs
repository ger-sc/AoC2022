var input = File.ReadAllText("input.txt").ToCharArray();
var shapes = new List<Shape> {
  new() {
    Rocks = new List<Position> {
      new() { X = 0, Y = 0 },
      new() { X = 1, Y = 0 },
      new() { X = 2, Y = 0 },
      new() { X = 3, Y = 0 }
    }
  }, new() {
    Rocks = new List<Position> {
      new() { X = 1, Y = 0 },
      new() { X = 0, Y = 1 },
      new() { X = 1, Y = 1 },
      new() { X = 2, Y = 1 },
      new() { X = 1, Y = 2 }
    }
  }, new() {
    Rocks = new List<Position> {
      new() { X = 0, Y = 0 },
      new() { X = 1, Y = 0 },
      new() { X = 2, Y = 0 },
      new() { X = 2, Y = 1 },
      new() { X = 2, Y = 2 }
    }
  }, new() {
    Rocks = new List<Position> {
      new() { X = 0, Y = 0 },
      new() { X = 0, Y = 1 },
      new() { X = 0, Y = 2 },
      new() { X = 0, Y = 3 }
    }
  }, new() {
    Rocks = new List<Position> {
      new() { X = 0, Y = 0 },
      new() { X = 1, Y = 0 },
      new() { X = 0, Y = 1 },
      new() { X = 1, Y = 1 }
    }
  }
};

var jetIndex = 0;
var blockCounter = 0L;
var blockIndex = 0;
var currentMaxHeight = -1L;
var map = new HashSet<Position>();
var cache = new Dictionary<Cache, (long, long)>();
const long limit = 1000000000000L;
var loopFound = false;

while (blockCounter < limit) {
  
  if (!loopFound) {
    var newCacheItem = new Cache {
      ShapeIndex = blockIndex,
      GustIndex = jetIndex,
      Board0 = currentMaxHeight - map.Where(m => m.X == 0).MaxBy(m => m.Y)?.Y,
      Board1 = currentMaxHeight - map.Where(m => m.X == 1).MaxBy(m => m.Y)?.Y,
      Board2 = currentMaxHeight - map.Where(m => m.X == 2).MaxBy(m => m.Y)?.Y,
      Board3 = currentMaxHeight - map.Where(m => m.X == 3).MaxBy(m => m.Y)?.Y,
      Board4 = currentMaxHeight - map.Where(m => m.X == 4).MaxBy(m => m.Y)?.Y,
      Board5 = currentMaxHeight - map.Where(m => m.X == 5).MaxBy(m => m.Y)?.Y,
      Board6 = currentMaxHeight - map.Where(m => m.X == 6).MaxBy(m => m.Y)?.Y
    };

    if (cache.ContainsKey(newCacheItem)) {
      loopFound = true;
      var (loopStart, oldMaxHeight) = cache[newCacheItem];
      
      var diff = blockCounter - loopStart;
      var maxHeightDiff = currentMaxHeight - oldMaxHeight;
      var times = (limit - blockCounter) / diff;
      currentMaxHeight += times * maxHeightDiff;
      blockCounter += times * diff;
      map.Add(new Position { X = 0, Y = currentMaxHeight - newCacheItem.Board0!.Value });
      map.Add(new Position { X = 1, Y = currentMaxHeight - newCacheItem.Board1!.Value });
      map.Add(new Position { X = 2, Y = currentMaxHeight - newCacheItem.Board2!.Value });
      map.Add(new Position { X = 3, Y = currentMaxHeight - newCacheItem.Board3!.Value });
      map.Add(new Position { X = 4, Y = currentMaxHeight - newCacheItem.Board4!.Value });
      map.Add(new Position { X = 5, Y = currentMaxHeight - newCacheItem.Board5!.Value });
      map.Add(new Position { X = 6, Y = currentMaxHeight - newCacheItem.Board6!.Value });
    }
    else {
      cache.Add(newCacheItem, (blockCounter, currentMaxHeight));
    }
  }
  
  var block = shapes[blockIndex];
  var position = new Position { X = 2, Y = currentMaxHeight + 4 };
  var settled = false;

  if (blockCounter == 2022) {
    Console.Out.WriteLine(currentMaxHeight + 1);
  }
  
  while (!settled) {
    var jet = input[jetIndex];
    jetIndex = (jetIndex + 1) % input.Length;
    Position newPosition;
    IList<Position> newRocks;
    switch (jet) {
      case '>':
        newPosition = position with { X = position.X + 1 };
        newRocks = GetRocksPositions(block, newPosition).ToList();
        if (newRocks.Max(x => x.X) > 6 || newRocks.Intersect(map).Any()) {
          newPosition = position;
        }

        position = newPosition;
        break;
      case '<':
        newPosition = position with { X = position.X - 1 };
        newRocks = GetRocksPositions(block, newPosition).ToList();
        if (newRocks.Min(x => x.X) < 0 || newRocks.Intersect(map).Any()) {
          newPosition = position;
        }

        position = newPosition;
        break;
    }
    newPosition = position with { Y = position.Y - 1 };
    if (GetRocksPositions(block, newPosition).Intersect(map).Any() || newPosition.Y < 0) {
      settled = true;
      foreach (var p in GetRocksPositions(block, position)) {
        map.Add(p);
      }
      blockCounter++;
      blockIndex = (blockIndex+1)%5;
      currentMaxHeight = map.Max(x => x.Y);
    }
    else {
      position = newPosition;
    }
  }
}

Console.Out.WriteLine(currentMaxHeight + 1);

IEnumerable<Position> GetRocksPositions(Shape shape, Position pos) {
  return shape.Rocks.Select(r => new Position { X = pos.X + r.X, Y = pos.Y + r.Y });
}

internal record Shape {
  public List<Position> Rocks { get; set; } = null!;
}

internal record Position {
  public int X { get; set; }
  public long Y { get; set; }
}

internal record Cache {
  public int ShapeIndex { get; init; }
  public int GustIndex { get; init; }
  public long? Board0 { get; init; }
  public long? Board1 { get; init; }
  public long? Board2 { get; init; }
  public long? Board3 { get; init; }
  public long? Board4 { get; init; }
  public long? Board5 { get; init; }
  public long? Board6 { get; init; }
}