using Godot;



public partial class Actions : Node {

  public static SceneTree tree;
  public static ErrorPopup errorPopup => tree.Root.GetNode<ErrorPopup>("Root/Static/UIRoot/error_panel");

  public static void selectBuilding(BuildingType buildingType) {
    var stats = Util.BuildingStats[buildingType];

    if (
      Globals.TwigCount >= stats.twigCost &&
      Globals.MeatCount >= stats.meatCost
    ) {
      Globals.selectedBuildingType = buildingType;
      Globals.selectedBuilding = GD.Load<PackedScene>(stats.resourcePath).Instantiate<Node2D>();
      Globals.selectedBuilding.Modulate = new Color(1, 1, 1, 0.5f);


      tree.Root.GetNode<Node2D>("Root").AddChild(Globals.selectedBuilding);
      Globals.gameMode = GameMode.Build;
    } else {
      if (Globals.TwigCount < stats.twigCost) {
        errorPopup.ShowError("Not enough Twigs to build this.");
      } else if (Globals.MeatCount < stats.meatCost) {
        errorPopup.ShowError("Not enough Meat to build this.");
      }
    }
  }


  public static void placeBuilding() {
    if (Globals.selectedBuildingType == BuildingType.None) {
      return;
    }

    if (Globals.selectedUnit is Ant a) {
      var stats = Util.BuildingStats[Globals.selectedBuildingType];

      if (
        Globals.TwigCount >= stats.twigCost &&
        Globals.MeatCount >= stats.meatCost
      ) {
        Globals.TwigCount -= stats.twigCost;
        Globals.MeatCount -= stats.meatCost;

        a.Build(Globals.selectedBuildingType, Globals.selectedBuilding.Position);

        Globals.selectedBuilding.QueueFree();

        Globals.gameMode = GameMode.Command;
      } else {
        errorPopup.ShowError("Not enough Twigs or Meat to build this.");
      }

    }
  }

}

