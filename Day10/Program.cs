var input = File.ReadAllLines("input.txt");

var iteration = 1;
var xValue = 1;

var register = new Dictionary<int, int>();

foreach (var line in input) {
  if (line == "noop") {
    register[iteration++] = xValue;
  }
  else {
    var value = int.Parse(line[5..]);
    register[iteration++] = xValue;
    register[iteration++] = xValue;
    xValue += value;
  }
}

var entries = new List<int> {
  20, 60, 100, 140, 180, 220
};

var pt1 = entries.Select(x => register[x] * x).Sum();
Console.Out.WriteLine(pt1);

var crt = "";
for (var i = 0; i < 240; i++) {
  var lineNumber = i / 40;
  var sprite = Enumerable.Range(register[i+1] - 1, 3);
  crt += sprite.Contains(i - 40 * lineNumber) ? "#" : " ";
}

var lines = crt.Chunk(40);
foreach (var l in lines) {
  Console.Out.WriteLine(new string(l));
}