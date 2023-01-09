using System.Collections.Generic;
using Godot;
public enum GameMode {
  Build,
  Command,
}

public class Globals {
  public static BuildingType selectedBuildingType;
  public static Node2D selectedBuilding;

  public static ISelectable selectedUnit;
  public static ISelectable buildingUnit;
  public static ISelectable hoveredUnit = null;
  public static GameMode gameMode = GameMode.Command;

  public static int MatchstickCount { get; set; } = Util.DEBUG ? 500 : 0;
  public static int GummyCount { get; set; } = 0;

  public static int TwigHarvestRate = 1;
  public static int MeatHarvestRate = 1;
}

public class GroupNames {
  public static string Building = "building";
  public static string GoodUnit = "good_unit";
  public static string BadUnit = "bad_unit";
  public static string Collider = "collider";
  public static string ResourceDropoff = "resource_dropoff";
  public static string Debug = "Debug";
}

public class Upgrades {
  public static int BugStrengthLevel = 0;
  public static int MatchstickSpeedLevel = 0;
  public static int GummySpeedLevel = 0;

  public static Dictionary<UpgradeType, Upgrade> AllUpgrades = new Dictionary<UpgradeType, Upgrade> {
    [UpgradeType.BugStrengthI] = new Upgrade {
      type = UpgradeType.BugStrengthI,
      name = "Bug Strength I",
      matchstickCost = Util.DEBUG ? 0 : 50,
      gummyCost = Util.DEBUG ? 0 : 50,
      time = Util.DEBUG ? 1f : 10f,
      description = "Increase the strength of all fighting bugs by 2.",
      onComplete = () => {
        BugStrengthLevel = 1;
      }
    },

    [UpgradeType.BugStrengthII] = new Upgrade {
      type = UpgradeType.BugStrengthII,
      name = "Bug Strength II",
      matchstickCost = Util.DEBUG ? 0 : 100,
      gummyCost = Util.DEBUG ? 0 : 100,
      time = Util.DEBUG ? 1f : 20f,
      description = "Increase the strength of all fighting bugs by 2 more.",
      onComplete = () => {
        BugStrengthLevel = 2;
      }
    },

    [UpgradeType.BugStrengthIII] = new Upgrade {
      type = UpgradeType.BugStrengthIII,
      name = "Bug Strength I",
      matchstickCost = Util.DEBUG ? 0 : 200,
      gummyCost = Util.DEBUG ? 0 : 200,
      time = Util.DEBUG ? 1f : 30f,
      description = "Increase the strength of all fighting bugs by 2 more, for a total of 6.",
      onComplete = () => {
        BugStrengthLevel = 3;
      }
    },

    [UpgradeType.FastGummyI] = new Upgrade {
      type = UpgradeType.FastGummyI,
      name = "Gummy Harvesting I",
      matchstickCost = Util.DEBUG ? 0 : 50,
      gummyCost = Util.DEBUG ? 0 : 100,
      time = Util.DEBUG ? 1f : 10f,
      description = "Increase the speed of gummy harvesting.",
      onComplete = () => {
        GummySpeedLevel = 1;
      }
    },

    [UpgradeType.FastGummyII] = new Upgrade {
      type = UpgradeType.FastGummyII,
      name = "Gummy Harvesting II",
      matchstickCost = Util.DEBUG ? 0 : 50,
      gummyCost = Util.DEBUG ? 0 : 200,
      time = Util.DEBUG ? 1f : 10f,
      description = "Increase the speed of gummy harvesting even more.",
      onComplete = () => {
        GummySpeedLevel = 2;
      }
    },

    [UpgradeType.FastGummyIII] = new Upgrade {
      type = UpgradeType.FastGummyIII,
      name = "Gummy Harvesting III",
      matchstickCost = Util.DEBUG ? 0 : 50,
      gummyCost = Util.DEBUG ? 0 : 300,
      time = Util.DEBUG ? 1f : 10f,
      description = "Increase the speed of gummy harvesting to ridiculous degrees!!!",
      onComplete = () => {
        GummySpeedLevel = 3;
      }
    },

    [UpgradeType.FastMatchstickI] = new Upgrade {
      type = UpgradeType.FastMatchstickI,
      name = "Matchstick Harvesting I",
      matchstickCost = Util.DEBUG ? 0 : 100,
      gummyCost = Util.DEBUG ? 0 : 50,
      time = Util.DEBUG ? 1f : 10f,
      description = "Increase the speed of matchstick harvesting.",
      onComplete = () => {
        MatchstickSpeedLevel = 1;
      }
    },

    [UpgradeType.FastMatchstickII] = new Upgrade {
      type = UpgradeType.FastGummyII,
      name = "Matchstick Harvesting II",
      matchstickCost = Util.DEBUG ? 0 : 200,
      gummyCost = Util.DEBUG ? 0 : 50,
      time = Util.DEBUG ? 1f : 10f,
      description = "Teach ants the fine arts of harvesting matchsticks from a local zen master.",
      onComplete = () => {
        MatchstickSpeedLevel = 2;
      }
    },

    [UpgradeType.FastMatchstickIII] = new Upgrade {
      type = UpgradeType.FastGummyIII,
      name = "Matchstick Harvesting III",
      matchstickCost = Util.DEBUG ? 0 : 300,
      gummyCost = Util.DEBUG ? 0 : 50,
      time = Util.DEBUG ? 1f : 10f,
      description = "Open demonic portals to more matchsticks.",
      onComplete = () => {
        MatchstickSpeedLevel = 3;
      }
    },
  };
}