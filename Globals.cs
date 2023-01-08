using Godot;
public enum GameMode {
  Build,
  Command,
}

public class Globals {

  public static BuildingType selectedBuildingType;
  public static Node2D selectedBuilding;

  public static ISelectable selectedUnit;
  public static ISelectable hoveredUnit = null;
  public static GameMode gameMode = GameMode.Command;

  public static int TwigCount { get; set; } = Util.DEBUG ? 100 : 0;
  public static int MeatCount { get; set; } = 0;

}

public class GroupNames {

  public static string Building = "building";
  public static string GoodUnit = "good_unit";
  public static string Collider = "collider";
  public static string ResourceDropoff = "resource_dropoff";
  public static string Debug = "Debug";
}