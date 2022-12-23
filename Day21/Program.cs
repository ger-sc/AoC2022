using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");
var dict = ReadInput(input);

while (dict["root"].Number == null)
{
    foreach (var monkey in dict.Values.Where(p => p.Number == null))
    {
        var m1 = dict[monkey.First].Number;
        var m2 = dict[monkey.Second].Number;
        if (m1 != null && m2 != null)
        {
            monkey.Number = monkey.Operation switch
            {
                "+" => m1 + m2,
                "-" => m1 - m2,
                "/" => m1 / m2,
                "*" => m1 * m2,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

Console.Out.WriteLine(dict["root"].Number);

const long root2 = 21718827469549;
var exponent = 12;
var human = 0L;

while (exponent > 0)
{
    var number = 1;
    var toTest = human + (long)(number * Math.Pow(10, exponent));
    var root1 = CalcRoot1(ReadInput(input), toTest);
    while (root1 is > root2 or < 0)
    {
        number++;
        toTest = human + (long)(number * Math.Pow(10, exponent));
        root1 = CalcRoot1(ReadInput(input), toTest);
    }

    human += (long)((number - 1) * Math.Pow(10, exponent));
    exponent--;
}

var h = human;
while(true)
{
    var monkeys = ReadInput(input);
    monkeys["root"] = monkeys["root"] with { Operation = "=" };
    monkeys["humn"].Number = h;
    while (monkeys["root"].Number == null)
    {
        foreach (var monkey in monkeys.Values.Where(p => p.Number == null))
        {
            var m1 = monkeys[monkey.First].Number;
            var m2 = monkeys[monkey.Second].Number;
            if (m1 != null && m2 != null)
            {
                monkey.Number = monkey.Operation switch
                {
                    "+" => m1 + m2,
                    "-" => m1 - m2,
                    "/" => m1 / m2,
                    "*" => m1 * m2,
                    "=" => m1 == m2 ? 1 : 0,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }

    if (monkeys["root"].Number == 1)
    {
        Console.Out.WriteLine(h);
        break;
    }
    h++;
}


long CalcRoot1(Dictionary<string, Problem> monkeys, long humn)
{
    monkeys["root"] = monkeys["root"] with { Operation = "=" };
    monkeys["humn"].Number = humn;
    while (monkeys["root"].Number == null)
    {
        foreach (var monkey in monkeys.Values.Where(p => p.Number == null))
        {
            var m1 = monkeys[monkey.First].Number;
            var m2 = monkeys[monkey.Second].Number;
            if (m1 == null || m2 == null) continue;
            if (monkey == monkeys["root"])
            {
                return m1.Value;
            }
            monkey.Number = monkey.Operation switch
            {
                "+" => m1 + m2,
                "-" => m1 - m2,
                "/" => m1 / m2,
                "*" => m1 * m2,
                "=" => m1 == m2 ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    return 0;
}

Dictionary<string, Problem> ReadInput(IEnumerable<string> lines)
{
    var result = new Dictionary<string, Problem>();
    const string regexPattern = @"([a-z]{4}): ((\d+)|([a-z]{4}) (.{1}) ([a-z]{4}))";
    foreach (var line in lines)
    {
        var match = Regex.Match(line, regexPattern);
        if (!string.IsNullOrWhiteSpace(match.Groups[6].Value))
        {
            result.Add(match.Groups[1].Value, new Problem
            {
                First = match.Groups[4].Value,
                Operation = match.Groups[5].Value,
                Second = match.Groups[6].Value,
            });
        }
        else
        {
            result.Add(match.Groups[1].Value, new Problem { Number = int.Parse(match.Groups[2].Value) });
        }
    }

    return result;
}

internal record Problem
{
    public string First { get; init; } = string.Empty;
    public string Second { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public long? Number { get; set; }
}