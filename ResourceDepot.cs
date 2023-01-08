using Godot;
using System.Collections.Generic;
using System;
public partial class ResourceDepot : Sprite2D, IBuilding, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>();

  public string name { get; set; } = "Resource Depot";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "Resource Depot";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  public string selectionText {
    get {
      return "Resource Depot!";
    }
  }

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }
}
