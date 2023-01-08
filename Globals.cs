using Godot;
public enum GameMode {
    Build,
    Command,
}

public class Globals {

    public static BuildingType selectedBuildingType;
    public static Sprite2D selectedBuilding;

    public static ISelectable selectedUnit;
    public static GameMode gameMode = GameMode.Command;

    public static int TwigCount { get; set; } = Util.DEBUG ? 100 : 0;
    public static int MeatCount { get; set; } = 0;
}
