using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

const string regex =
    @"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.";

var blueprints = input.Select(line => {
    var match = Regex.Match(line, regex);
    return new Blueprint {
        Index = int.Parse(match.Groups[1].Value),
        OreRobot = new Costs {Ore = int.Parse(match.Groups[2].Value)},
        ClayRobot = new Costs {Ore = int.Parse(match.Groups[3].Value)},
        ObsidianRobot = new Costs {Ore = int.Parse(match.Groups[4].Value), Clay = int.Parse(match.Groups[5].Value)},
        GeodeRobot = new Costs {Ore = int.Parse(match.Groups[6].Value), Obsidian = int.Parse(match.Groups[7].Value)}
    };
}).ToList();

var pt1 = 0;

Parallel.ForEach(blueprints, bp => {
    var result = 0;
    var inventory = new Inventory { OreRobot = 1 };
    Mine(inventory, 24, null, bp, ref result);
    pt1 += result * bp.Index;
});

Console.Out.WriteLine(pt1);

var pt2 = 1;

Parallel.ForEach(blueprints.Take(3), bp => {
    var result = 0;
    var inventory = new Inventory { OreRobot = 1 };
    Mine(inventory, 32, null, bp, ref result);
    pt2 *= result;
});

Console.Out.WriteLine(pt2);

void Mine(Inventory inventory, int minute, Robot? toBuild, Blueprint blueprint, ref int result) {
    while (true) {
        if (minute <= 0) {
            if (inventory.Geode > result) {
                result = inventory.Geode;
            }
        }
        else {
            var theoreticalGeodes = inventory.Geode;
            var bots = inventory.GeodeRobot;
            for (var togo = minute; togo > 0; togo--) {
                theoreticalGeodes += bots;
                bots++;
            }
            if (theoreticalGeodes < result) {
                minute = 0;
                continue;
            }
            switch (toBuild) {
                case Robot.Geode:
                    if (CanBuildGeodeRobot(blueprint, inventory)) {
                        inventory = inventory with {
                            Ore = inventory.Ore - blueprint.GeodeRobot.Ore + inventory.OreRobot,
                            Clay = inventory.Clay - blueprint.GeodeRobot.Clay + inventory.ClayRobot,
                            Obsidian = inventory.Obsidian - blueprint.GeodeRobot.Obsidian + inventory.ObsidianRobot,
                            Geode = inventory.Geode + inventory.GeodeRobot,
                            GeodeRobot = inventory.GeodeRobot + 1
                        };
                        foreach (var r in CanBuild(inventory)) {
                            Mine(inventory, minute - 1, r, blueprint, ref result);
                        }
                    }
                    else {
                        inventory = Collect(inventory);
                        minute -= 1;
                        continue;
                    }

                    break;
                case Robot.Ore:
                    if (CanBuildOreRobot(blueprint, inventory)) {
                        inventory = inventory with {
                            Ore = inventory.Ore - blueprint.OreRobot.Ore + inventory.OreRobot,
                            Clay = inventory.Clay - blueprint.OreRobot.Clay + inventory.ClayRobot,
                            Obsidian = inventory.Obsidian - blueprint.OreRobot.Obsidian + inventory.ObsidianRobot,
                            Geode = inventory.Geode + inventory.GeodeRobot,
                            OreRobot = inventory.OreRobot + 1
                        };
                        foreach (var r in CanBuild(inventory)) {
                            Mine(inventory, minute - 1, r, blueprint, ref result);
                        }
                    }
                    else {
                        inventory = Collect(inventory);
                        minute -= 1;
                        continue;
                    }

                    break;
                case Robot.Clay:
                    if (CanBuildClayRobot(blueprint, inventory)) {
                        inventory = inventory with {
                            Ore = inventory.Ore - blueprint.ClayRobot.Ore + inventory.OreRobot,
                            Clay = inventory.Clay - blueprint.ClayRobot.Clay + inventory.ClayRobot,
                            Obsidian = inventory.Obsidian - blueprint.ClayRobot.Obsidian + inventory.ObsidianRobot,
                            Geode = inventory.Geode + inventory.GeodeRobot,
                            ClayRobot = inventory.ClayRobot + 1
                        };
                        foreach (var r in CanBuild(inventory)) {
                            Mine(inventory, minute - 1, r, blueprint, ref result);
                        }
                    }
                    else {
                        inventory = Collect(inventory);
                        minute -= 1;
                        continue;
                    }

                    break;
                case Robot.Obsidian:
                    if (CanBuildObsidianRobot(blueprint, inventory)) {
                        inventory = inventory with {
                            Ore = inventory.Ore - blueprint.ObsidianRobot.Ore + inventory.OreRobot,
                            Clay = inventory.Clay - blueprint.ObsidianRobot.Clay + inventory.ClayRobot,
                            Obsidian = inventory.Obsidian - blueprint.ObsidianRobot.Obsidian + inventory.ObsidianRobot,
                            Geode = inventory.Geode + inventory.GeodeRobot,
                            ObsidianRobot = inventory.ObsidianRobot + 1
                        };
                        foreach (var r in CanBuild(inventory)) {
                            Mine(inventory, minute - 1, r, blueprint, ref result);
                        }
                    }
                    else {
                        inventory = Collect(inventory);
                        minute -= 1;
                        continue;
                    }

                    break;
                case null:
                    inventory = Collect(inventory);
                    foreach (var r in CanBuild(inventory)) {
                        Mine(inventory, minute - 1, r, blueprint, ref result);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(toBuild), toBuild, null);
            }
        }

        break;
    }
}

Inventory Collect(Inventory inventory) {
    return inventory with {
        Ore = inventory.Ore + inventory.OreRobot,
        Clay = inventory.Clay + inventory.ClayRobot,
        Obsidian = inventory.Obsidian + inventory.ObsidianRobot,
        Geode = inventory.Geode + inventory.GeodeRobot
    };
}

IEnumerable<Robot> CanBuild(Inventory inventory) {
    if (inventory.ObsidianRobot > 0) yield return Robot.Geode;
    if (inventory.ClayRobot > 0) yield return Robot.Obsidian;
    if (inventory.OreRobot > 0) yield return Robot.Ore;
    if (inventory.OreRobot > 0) yield return Robot.Clay;
}

bool CanBuildOreRobot(Blueprint bp, Inventory i) {
    return i.Ore >= bp.OreRobot.Ore;
}

bool CanBuildClayRobot(Blueprint bp, Inventory i) {
    return i.Ore >= bp.ClayRobot.Ore;
}

bool CanBuildObsidianRobot(Blueprint bp, Inventory i) {
    return i.Ore >= bp.ObsidianRobot.Ore && i.Clay >= bp.ObsidianRobot.Clay;
}

bool CanBuildGeodeRobot(Blueprint bp, Inventory i) {
    return i.Ore >= bp.GeodeRobot.Ore && i.Obsidian >= bp.GeodeRobot.Obsidian;
}

Console.Out.WriteLine("");



internal record Inventory
{
    public int OreRobot { get; init; }
    public int ClayRobot { get; init; }
    public int ObsidianRobot { get; init; }
    public int GeodeRobot { get; init; }
    public int Ore { get; init; }
    public int Clay { get; init; }
    public int Obsidian { get; init; }
    public int Geode { get; init; }
}

internal enum Robot {
    Ore,
    Clay,
    Obsidian,
    Geode
}

internal record Blueprint {
    public int Index { get; init; }
    public Costs OreRobot { get; init; }
    public Costs ClayRobot { get; init; }
    public Costs ObsidianRobot { get; init; }
    public Costs GeodeRobot { get; init; }
}

internal record Costs
{
    public int Ore { get; init; }
    public int Clay { get; init; }
    public int Obsidian { get; init; }
}