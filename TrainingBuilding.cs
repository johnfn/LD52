using Godot;

public enum UnitType {
  None,
  Ant,
  Beetle,
  Scout,
  Spit
}

public enum BuildingStatus {
  Idle,
  Building
}

// This is currently either TownHall or BugBarracks
public partial class TrainingBuilding : Sprite2D, IBuilding, ISelectable, ICollider {

  [Export]
  public bool IsBugBarracks;

  // IBuilding
  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 0.1f;
  public string unitName { get; } = "Town Hall";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public UnitType currentBuildingUnitType { get; set; } = UnitType.None;
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

  public override void _Ready() {
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");
  }

  public override void _Process(double delta) {
    if (status == BuildingStatus.Building) {
      buildProgress += (float)delta;

      if (buildProgress >= buildTime) {
        status = BuildingStatus.Idle;
        buildProgress = 0;

        var unitStats = Util.UnitStats[currentBuildingUnitType];
        var scene = GD.Load<PackedScene>(unitStats.resourcePath);
        var newUnit = scene.Instantiate<Node2D>();

        newUnit.Position = Util.FindSafeSpaceNear(GetTree(), GlobalPosition);

        GetParent().AddChild(newUnit);

        currentBuildingUnitType = UnitType.None;
      }
    }
  }

  public void BuyUnit(UnitType unit) {
    status = BuildingStatus.Building;

    buildProgress = 0;
    buildTime = 0.1f;
    currentBuildingUnitType = unit;
  }
}
