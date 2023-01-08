using Godot;
using System.Collections.Generic;
using System;


public partial class BugBarracks : Sprite2D, IBuilding, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>();

  public string selectionText {
    get {
      return "Boring bug barracks blorg";
    }
  }

  public string name { get; set; } = "Barracks";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "BugBarracks";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

}
