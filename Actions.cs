using Godot;
using System;



public partial class Actions : Node {

  public static SceneTree tree;

  public static void selectBuilding(BuildingType buildingType) {
    var stats = Util.BuildingStats[buildingType];

    if (
      Globals.TwigCount >= stats.twigCost &&
      Globals.MeatCount >= stats.meatCost
    ) {
      Globals.selectedBuildingType = buildingType;
      Globals.selectedBuilding = GD.Load<PackedScene>(stats.resourcePath).Instantiate<Sprite2D>();
      Globals.selectedBuilding.Modulate = new Color(1, 1, 1, 0.5f);


      tree.Root.GetNode<Node2D>("Root").AddChild(Globals.selectedBuilding);
      Globals.gameMode = GameMode.Build;
    } else {
      GD.Print("Not enough Twigs or Meat to build this.");
    }
  }


  public static void placeBuilding() {
    if (Globals.selectedBuildingType == BuildingType.None) {
      return;
    }

    var stats = Util.BuildingStats[Globals.selectedBuildingType];

    if (
      Globals.TwigCount >= stats.twigCost &&
      Globals.MeatCount >= stats.meatCost
    ) {
      Globals.TwigCount -= stats.twigCost;
      Globals.MeatCount -= stats.meatCost;

      Globals.selectedBuilding.Modulate = new Color(1, 1, 1, 1);
      Globals.selectedBuilding = null;
      Globals.selectedBuildingType = BuildingType.None;
      Globals.gameMode = GameMode.Command;
    } else {
      // TODO

      GD.Print("Not enough twigs");
    }
  }

}

