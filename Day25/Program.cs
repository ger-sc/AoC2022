var input = File.ReadLines("input.txt");
var pt1 = input.Select(Calc).Sum();
Console.Out.WriteLine(pt1);
Console.Out.WriteLine(ReverseCalc(pt1));

string ReverseCalc(long decimalNumber) {
  var result = "";

  while (decimalNumber > 0) {
    var number = decimalNumber % 5;
    decimalNumber /= 5;
    
    switch (number) {
      case 0:
        result = "0" + result;
        break;
      case 1:
        result = "1" + result;
        break;
      case 2:
        result = "2" + result;
        break;
      case 3:
        result = "=" + result;
        decimalNumber++;
        break;
      case 4:
        result = "-" + result;
        decimalNumber++;
        break;
    }
  }
  return result;
}

long Calc(string number) {
  var places = number.ToCharArray();
  return (long)places.Reverse().Select((c, i) => {
    var value = Math.Pow(5, i);
    return c switch {
      '0' => 0,
      '1' => value,
      '2' => 2 * value,
      '-' => -value,
      '=' => -2 * value,
      _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };
  }).Sum();
}