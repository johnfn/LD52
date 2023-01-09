using Godot;
using System.Collections.Generic;
using System;
public partial class ResourceDepot : Node2D, IBuilding, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>();

  public string name { get; set; } = "Resource Depot";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "Resource Depot";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");

  public string selectionText {
    get {
      return "Resource Depot!";
    }
  }

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public override void _Ready() {
    SelectionCircle.Unit = this;
  }

  public void OnHoverStart() {
  }

  public void OnHoverEnd() {
  }
}
