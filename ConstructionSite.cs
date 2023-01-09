using System;
using System.Collections.Generic;
using Godot;

public class BuildingState {
  public BuildingType SelectedBuildingType = BuildingType.None;
  public float BuildProgress = 0;
  public float BuildTime = 0;
}


public partial class ConstructionSite : Node2D, ISelectable {
  public Dictionary<string, Action> actions => new Dictionary<string, Action>();

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public string name { get; set; } = "Construction Site";

  public string selectionText {
    get {
      if (BuildingState == null) {
        return "";
      }

      return "A construction site." + (100f * BuildingState.BuildProgress / BuildingState.BuildTime).ToString(
        "0"
      ) + "% done.";
    }
  }

  public BuildingState BuildingState = null;
  public ProgressBar ProgressBar => GetNode<ProgressBar>("ProgressBar");

  public void OnHoverEnd() {
  }

  public void OnHoverStart() {
  }

  public override void _Process(double delta) {
    if (BuildingState == null) {
      return;
    }

    ProgressBar.SetProgress(BuildingState.BuildProgress / BuildingState.BuildTime);
  }
}
