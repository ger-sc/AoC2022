using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

const string regex = @"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)";

var map = new Dictionary<Position, Position>();

foreach (var line in input) {
  var match = Regex.Match(line, regex);
  var sensor = new Position {X = int.Parse(match.Groups[1].Value), Y = int.Parse(match.Groups[2].Value)};
  var beacon = new Position {X = int.Parse(match.Groups[3].Value), Y = int.Parse(match.Groups[4].Value)};
  map.Add(sensor, beacon);
}

const int rowToSearch = 2000000;
var result = new HashSet<int>();

foreach (var (sensor, beacon) in map) {
  var distance = Distance(sensor, beacon);
  var yDistance = Math.Abs(rowToSearch - sensor.Y);
  if (yDistance <= distance) {
    var rest = distance - yDistance;
    var xCoordinates = Enumerable.Range(sensor.X - rest, 2 * rest);
    foreach (var x in xCoordinates) {
      result.Add(x);
    }    
  }
}
Console.Out.WriteLine(result.Distinct().Count());

var reach = map.ToDictionary(
  m => m.Key, 
  m => Distance(m.Key, m.Value));

foreach (var (sensor, dist) in reach) {
  var edges = GetEdge(sensor, dist);
  foreach (var edge in edges) {
    FindNotCovered(edge);
  }
}

IEnumerable<Position> GetEdge(Position sensor, int distance) {
  for (var i = 0; i <= distance; i++) {
    yield return new Position { Y = sensor.Y - i, X = sensor.X - distance - 1 + i };
    yield return new Position { Y = sensor.Y - i, X = sensor.X + distance + 1 - i };
    yield return new Position { Y = sensor.Y + i, X = sensor.X + distance + 1 - i };
    yield return new Position { Y = sensor.Y + i, X = sensor.X - distance - 1 + i };
  }
}

const int min = 0;
const int max = 4000000;

void FindNotCovered(Position point) {
  if (point.X is < min or > max || point.Y is < min or > max) return;
  if (map.ContainsKey(point) || map.ContainsValue(point)) return;
  
  var found = reach.Where(kv => Distance(kv.Key, point) <= kv.Value);
  if (!found.Any()) {
    Console.Out.WriteLine(point.X * 4000000L + point.Y);
  }
}

int Distance(Position a, Position b) {
  return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}

internal record Position {
  public int X { get; init; }
  public int Y { get; init; }
}