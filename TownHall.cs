using Godot;
using System.Collections.Generic;
using System;

public enum UnitType {
  Ant
}

public enum BuildingStatus {
  Idle,
  Building
}

public partial class TownHall : Sprite2D, IBuilding, ISelectable, ICollider {

  // ISelectable
  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {
    ["Ant"] = (delegate () {
      if (Globals.selectedUnit is TownHall th) {
        th.BuyUnit(UnitType.Ant);
      }
    }),
  };


  // IBuilding
  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 0.1f;
  public string unitName { get; set; } = "Town Hall";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public Area2D colliderShape {
    get {
      return _shape;
    }
  }

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  // Locals

  private Area2D _shape;
  private UiPanel _uiPanel;
  private PackedScene _antScene;

  public override void _Ready() {
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");
    _antScene = GD.Load<PackedScene>("res://Scenes/ant.tscn");
  }

  public override void _Process(double delta) {
    if (status == BuildingStatus.Building) {
      buildProgress += (float)delta;

      if (buildProgress >= buildTime) {
        status = BuildingStatus.Idle;
        buildProgress = 0;

        var ant = _antScene.Instantiate<Ant>();

        ant.Position = Position + new Vector2(0, 150);
        GetParent().AddChild(ant);
      }
    }
  }

  public void BuyUnit(UnitType unit) {
    if (unit == UnitType.Ant) {
      status = BuildingStatus.Building;

      buildProgress = 0;
      buildTime = 0.1f;
    }
  }
}
