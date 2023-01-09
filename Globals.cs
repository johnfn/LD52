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

  public static int MatchstickCount { get; set; } = Util.DEBUG ? 100 : 0;
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
  };
}