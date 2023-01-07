using System.Collections.Generic;
using Godot;

public enum UnitStatus {
  Idle,
  Moving,
  Harvesting
}

public partial class Ant : Sprite2D, IUnit, ISelectable {

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }


  // IUnit
  public string unitName { get; set; } = "Ant";
  public int health { get; set; } = 10;

  private int _speed = 500;
  private UnitStatus _status = UnitStatus.Idle;
  private List<Vector2> _path = new List<Vector2>();

  private Area2D _shape;

  private UiPanel _uiPanel;


  public override void _Ready() {
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UiPanel");
  }

  public override void _Process(double delta) {
    if (_status == UnitStatus.Moving) {
      if (_path.Count > 0) {
        var target = _path[0];
        var distance = Position.DistanceTo(target);
        var direction = (target - Position).Normalized();

        if (distance > 1) {
          Position += direction * _speed * (float)delta;
        } else {
          _path.RemoveAt(0);
        }
      } else {
        _status = UnitStatus.Idle;
      }
    }
  }

  public void Move(Vector2 destination) {
    _status = UnitStatus.Moving;
    _path = Util.Pathfind(GetTree(), GlobalPosition, destination);
  }

  public void Harvest(IResource resource) {
    _status = UnitStatus.Harvesting;

    GD.Print("Harvesting");
  }
}
