var input = File.ReadAllText("input.txt");

Console.Out.WriteLine(Find(input, 4));
Console.Out.WriteLine(Find(input, 14));

int Find(string signal, int length) {
  for (var i = 0; i < signal.Length - length; i++) {
    var sub = signal.Substring(i, length);
    if (sub.ToArray().Distinct().Count() == length) {
      return i + length;
    }
  }
  return -1;
}