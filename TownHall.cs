using Godot;

public enum UnitType {
  Ant
}

public enum BuildingStatus {
  Idle,
  Building
}

public interface IBuilding {
  public float buildProgress { get; set; }
  public float buildTime { get; set; }
  public string unitName { get; set; }
  public BuildingStatus status { get; set; }
}


public partial class TownHall : Sprite2D, IBuilding {
  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 0.1f;
  public string unitName { get; set; } = "Town Hall";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  private Area2D _shape;
  private UiPanel _uiPanel;
  private PackedScene _antScene;

  public override void _Ready() {
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UiPanel");
    _antScene = GD.Load<PackedScene>("res://Scenes/ant.tscn");

    _shape.Connect("mouse_entered", Callable.From(() => {
      Modulate = new Color(1, 1, 1, 0.5f);
    }));

    _shape.Connect("mouse_exited", Callable.From(() => {
      Modulate = new Color(1, 1, 1, 1f);
    }));

    _shape.Connect("mouse_button_pressed", Callable.From((Vector2 pos) => {
      _uiPanel.Select(this);
    }));
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
