using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt")
  .Split("\r\n\r\n")
  .Select(x => x.Split("\r\n"))
  .Select((x, i) => (i+1, ParseSignal(new SignalReader(x[0])), ParseSignal(new SignalReader(x[1]))));

var pt1 = 0;
foreach (var (i, l, r) in input) {
  var isOrdered = l.CompareTo(r);
  if (isOrdered == -1) {
    pt1 += i;
  }
}

Console.Out.WriteLine(pt1);

var input2 = File.ReadAllLines("input.txt")
  .Where(x => !string.IsNullOrWhiteSpace(x))
  .Select(x => ParseSignal(new SignalReader(x)))
  .ToList();

var newSignal2 = new Signal {Items = new List<object> { new Signal {Items = new List<object> {2}}}};
input2.Add(newSignal2);
var newSignal6 = new Signal {Items = new List<object> { new Signal {Items = new List<object> {6}}}};
input2.Add(newSignal6);

input2.Sort();

Console.Out.WriteLine((input2.IndexOf(newSignal2) + 1) * (input2.IndexOf(newSignal6) + 1));

Signal ParseSignal(SignalReader signal) {
  var result = new Signal();
  string next;
  while ((next = signal.Read()) != string.Empty) {
    if (next == "[") {
      result.Items.Add(ParseSignal(signal));
    } else if (next == "]") {
      return result;
    } else if (IsNumeric(next)) {
      result.Items.Add(ReadNumber(next, signal));
    }
  }

  return result;
}

int ReadNumber(string first, SignalReader signal) {
  var number = first;
  while (IsNumeric(signal.Peak())) {
    number += signal.Read();
  }
  return int.Parse(number);
}

bool IsNumeric(string s) {
  return Regex.IsMatch(s, "\\d");
}

internal class Signal : IComparable<Signal> {
  public IList<object> Items { get; init; } = new List<object>();
  public int CompareTo(Signal? other) {
    if (other == null) throw new ArgumentException("Could not compare");
    var isOrdered = IsOrdered(this, other);
    if (!isOrdered.HasValue) throw new ArgumentException("Could not compare");
    return isOrdered.Value ? -1 : 1;
  }
  
  private static bool? IsOrdered(Signal left, Signal right) {
    for (var i = 0; i < Math.Max(left.Items.Count, right.Items.Count); i++) {
      var leftItem = left.Items.ElementAtOrDefault(i);
      var rightItem = right.Items.ElementAtOrDefault(i);
      if (leftItem == null && rightItem != null) return true;
      if (leftItem != null && rightItem == null) return false;

      if (leftItem is int li && rightItem is int ri) {
        if (li > ri) return false;
        if (li < ri) return true;
      } else if (leftItem is int && rightItem is Signal rightSignal) {
        var ordered = IsOrdered(new Signal { Items = new List<object> { leftItem } }, rightSignal);
        if (ordered.HasValue) return ordered.Value;
      } else if (rightItem is int && leftItem is Signal leftSignal) {
        var ordered = IsOrdered(leftSignal, new Signal { Items = new List<object> { rightItem } });
        if (ordered.HasValue) return ordered.Value;
      } else {
        var ordered = IsOrdered((Signal)leftItem!, (Signal)rightItem!);
        if (ordered.HasValue) return ordered.Value;
      }
    }
    return null;
  }
}

internal class SignalReader {
  private readonly string _signal;
  private int _position;

  public SignalReader(string signal) {
    _signal = signal;
    _position = 0;
  }

  public string Read() {
    if (_position >= _signal.Length) return string.Empty;
    var s = _signal[_position];
    _position++;
    return s.ToString();
  }

  public string Peak() {
    return _position >= _signal.Length ? string.Empty : _signal[_position].ToString();
  }
}