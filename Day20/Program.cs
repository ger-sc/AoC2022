var list = new LinkedList<ListItem>(
  File.ReadAllLines("input.txt")
    .Select((s, index) => new ListItem {
      Value = long.Parse(s), 
      OriginalIndex = index
    })
  .ToList());

var mixedList = Mix(list);

var zero = mixedList.Single(l => l.Value == 0);
var zeroIndex = IndexOf(mixedList, zero);
var pt1 = mixedList.ElementAt((zeroIndex + 1000) % mixedList.Count).Value +
          mixedList.ElementAt((zeroIndex + 2000) % mixedList.Count).Value +
          mixedList.ElementAt((zeroIndex + 3000) % mixedList.Count).Value;

Console.Out.WriteLine(pt1);

const long key = 811589153;

mixedList = new LinkedList<ListItem>(
  File.ReadAllLines("input.txt")
    .Select((s, index) => new ListItem {
      Value = long.Parse(s) * key, 
      OriginalIndex = index
    })
  .ToList());

for (var c = 0; c < 10; c++) {
  mixedList = Mix(mixedList);
}

zero = mixedList.Single(l => l.Value == 0);
zeroIndex = IndexOf(mixedList, zero);
var pt2 = mixedList.ElementAt((zeroIndex + 1000) % mixedList.Count).Value +
          mixedList.ElementAt((zeroIndex + 2000) % mixedList.Count).Value +
          mixedList.ElementAt((zeroIndex + 3000) % mixedList.Count).Value;

Console.Out.WriteLine(pt2);

LinkedList<ListItem> Mix(LinkedList<ListItem> items) {
  for (var i = 0; i < items.Count; i++) {
    var i1 = i;
    var value = items.Single(k => k.OriginalIndex == i1);
    var node = items.Find(value)!;
    for (var m = 0; m < Math.Abs(value.Value % (items.Count - 1)); m++) {
      if (value.Value > 0) {
        var newNode = node.Next ?? node.List?.First!;
        items.Remove(node);
        items.AddAfter(newNode, node);
      }
      else {
        var newNode = node.Previous ?? node.List?.Last!;
        items.Remove(node);
        items.AddBefore(newNode, node);
      }
    }
  }
  return items;
}

int IndexOf(IReadOnlyCollection<ListItem> linkedList, ListItem item) {
  for (var i = 0; i < linkedList.Count; i++) {
    if (linkedList.ElementAt(i) == item) {
      return i;
    }
  }
  return -1;
}

internal record ListItem {
  public long Value { get; init; }
  public int OriginalIndex { get; init; }
}


