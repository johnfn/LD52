using System;
using System.Collections.Generic;
using Godot;

public class BuildingState {
  public BuildingType SelectedBuildingType = BuildingType.None;
  public float BuildProgress = 0;
  public float BuildTime = 0;
}


public partial class ConstructionSite : Sprite2D, ISelectable {
  public Dictionary<string, Action> actions => new Dictionary<string, Action>();

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public string name { get; set; } = "Construction Site";

  public string selectionText => "A construction site." + System.Environment.NewLine + "Build time: " + BuildingState.BuildTime + System.Environment.NewLine + "Build progress: " + BuildingState.BuildProgress;

  public BuildingState BuildingState;

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }
}
