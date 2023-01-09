using Godot;



public partial class Actions : Node {

  public static SceneTree tree;
  public static ErrorPopup errorPopup => tree.Root.GetNode<ErrorPopup>("Root/Static/UIRoot/error_panel");

  public static void selectBuilding(BuildingType buildingType) {
    var stats = Util.BuildingStats[buildingType];

    if (
      Globals.MatchstickCount >= stats.twigCost &&
      Globals.GummyCount >= stats.meatCost
    ) {
      Globals.selectedBuildingType = buildingType;
      Globals.selectedBuilding = GD.Load<PackedScene>(stats.resourcePath).Instantiate<Node2D>();
      Globals.selectedBuilding.Modulate = new Color(1, 1, 1, 0.5f);


      tree.Root.GetNode<Node2D>("Root").AddChild(Globals.selectedBuilding);
      Globals.gameMode = GameMode.Build;
    } else {
      if (Globals.MatchstickCount < stats.twigCost) {
        errorPopup.ShowError("Not enough Twigs to build this.");
      } else if (Globals.GummyCount < stats.meatCost) {
        errorPopup.ShowError("Not enough Meat to build this.");
      }
    }
  }


  public static void placeBuilding(UiPanel uiPanel) {
    GD.Print("place");

    if (Globals.selectedBuildingType == BuildingType.None) {
      GD.Print("tried to build a None");

      return;
    }

    if (Globals.buildingUnit is Ant a) {
      var stats = Util.BuildingStats[Globals.selectedBuildingType];
      var isSafe = uiPanel.IsBuildingInSafeLocation(Globals.selectedBuilding);

      if (!isSafe) {
        errorPopup.ShowError("Make sure the area for your building is clear.");
        return;
      }

      if (Globals.MatchstickCount < stats.twigCost) {
        errorPopup.ShowError("Not enough Twigs to build this.");
        return;
      }

      if (Globals.GummyCount < stats.meatCost) {
        errorPopup.ShowError("Not enough Meat to build this.");
        return;
      }

      Globals.MatchstickCount -= stats.twigCost;
      Globals.GummyCount -= stats.meatCost;

      a.Build(Globals.selectedBuildingType, Globals.selectedBuilding.Position);

      Globals.selectedBuilding.QueueFree();

      Globals.gameMode = GameMode.Command;
    } else {
      GD.Print("What");
      GD.Print(Globals.selectedUnit);
    }
  }
}

