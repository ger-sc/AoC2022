using System.Collections.Immutable;
using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

const string regex = @"Valve (\w+) has flow rate=(\d+); tunnels? leads? to valves? (.+)";

var tunnels = new List<Tunnel>();

foreach (var line in input) {
  var match = Regex.Match(line, regex);
  tunnels.Add(new Tunnel {
    Name = match.Groups[1].Value, 
    FlowRate = int.Parse(match.Groups[2].Value), 
    Tunnels = match.Groups[3].Value
      .Split(",")
      .Select(t => t.Trim())
      .ToList()
  });
}

var shortestPaths = new Dictionary<Tunnel, Dictionary<Tunnel, int>>();

foreach (var from in tunnels) {
  foreach (var to in tunnels.Where(t => t != from)) {
    var dist = int.MaxValue;
    if (shortestPaths.ContainsKey(from) && shortestPaths[from].ContainsKey(to)) continue;
    FindShortestPath(from, to, tunnels, new List<Tunnel> {from}, ref dist);
    if (shortestPaths.ContainsKey(from)) {
      shortestPaths[from].Add(to, dist);
    } else {
      shortestPaths[from] = new Dictionary<Tunnel, int> { { to, dist } };
    }
    if (shortestPaths.ContainsKey(to)) {
      shortestPaths[to].Add(from, dist);
    } else {
      shortestPaths[to] = new Dictionary<Tunnel, int> { { from, dist } };
    }
  }
}

void FindShortestPath(Tunnel from, Tunnel to, List<Tunnel> map, List<Tunnel> path, ref int dist) {
  if (from.Tunnels.Contains(to.Name)) {
    if (path.Count < dist) {
      dist = path.Count;
    }
  }
  foreach (var tunnel in from.Tunnels.Where(t => !path.Contains(map.Single(x => x.Name == t)))) {
    FindShortestPath(map.Single(x => x.Name == tunnel), to, map, path.Concat(new []{from}).ToList(), ref dist);
  }
}

var start = tunnels.Single(x => x.Name == "AA");
var pt1 = 0;
OpenValves(start, 29, 0, ImmutableHashSet<Tunnel>.Empty.Union(tunnels.Where(x => x.FlowRate == 0)), ref pt1);
Console.Out.WriteLine(pt1);

var pt2 = 0;

OpenValvesWithElephant(start, 25, 0, ImmutableHashSet<Tunnel>.Empty.Union(tunnels.Where(x => x.FlowRate == 0)), ref pt2, false);
Console.Out.WriteLine(pt2);

void OpenValves(Tunnel current, int minutesLeft, int currentFlow, ImmutableHashSet<Tunnel> open, ref int maxFlowRate) {
  if (currentFlow > maxFlowRate) {
    maxFlowRate = currentFlow;
  }
  if (minutesLeft <= 0) return;

  if (!open.Contains(current)) {
    OpenValves(current, minutesLeft - 1, currentFlow + current.FlowRate * minutesLeft, open.Union(new []{current}), ref maxFlowRate);
  } else {
    foreach (var (key, value) in shortestPaths[current].Where(x => !open.Contains(x.Key))) {
      OpenValves(key, minutesLeft - value, currentFlow, open, ref maxFlowRate);
    }
  }
}

void OpenValvesWithElephant(Tunnel current, int minutesLeft, int currentFlow, ImmutableHashSet<Tunnel> open, ref int maxFlowRate, bool elephant) {
  if (currentFlow > maxFlowRate) {
    maxFlowRate = currentFlow;
    Console.Out.WriteLine(maxFlowRate);
  }
  if (minutesLeft <= 0) return;

  if (!open.Contains(current)) {
    OpenValvesWithElephant(current, minutesLeft - 1, currentFlow + current.FlowRate * minutesLeft, open.Union(new []{current}), ref maxFlowRate, elephant);
    if (!elephant) {
      OpenValvesWithElephant(tunnels.Single(x => x.Name == "AA"), 25, currentFlow + current.FlowRate * minutesLeft, open.Union(new []{current}), ref maxFlowRate, true);
    }
  } else {
    foreach (var (key, value) in shortestPaths[current].Where(x => !open.Contains(x.Key))) {
      OpenValvesWithElephant(key, minutesLeft - value, currentFlow, open, ref maxFlowRate, elephant);
    }
  }
}

record Tunnel {
  public string Name { get; init; }
  public int FlowRate { get; init; }
  public IList<string> Tunnels { get; set; }
}