using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum UnitStatus {
  Idle,
  Moving
}

public partial class Ant : Sprite2D, IUnit {
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

  public void move(Vector2 destination) {
    _status = UnitStatus.Moving;
    _path = FindPath(GlobalPosition, destination);
  }

  public Vector2 RoundTo32(Vector2 vector) {
    return new Vector2(
        (int)(vector.x / 32) * 32,
        (int)(vector.y / 32) * 32
    );
  }

  public List<Vector2> FindPath(Vector2 initialStart, Vector2 initialEnd) {
    int cellSize = 32;

    var allColliders = GetTree().GetNodesInGroup("collider");

    var start = RoundTo32(initialStart);
    var end = RoundTo32(initialEnd);

    var topLeft = new Vector2(
        Math.Min(start.x, end.x),
        Math.Min(start.y, end.y)
    );
    var bottomRight = new Vector2(
        Math.Max(start.x, end.x),
        Math.Max(start.y, end.y)
    );

    foreach (Node2D collider in allColliders) {
      topLeft.x = Math.Min(topLeft.x, collider.GlobalPosition.x);
      topLeft.y = Math.Min(topLeft.y, collider.GlobalPosition.y);

      bottomRight.x = Math.Max(bottomRight.x, collider.GlobalPosition.x);
      bottomRight.y = Math.Max(bottomRight.y, collider.GlobalPosition.y);
    }

    topLeft -= new Vector2(320, 320);
    bottomRight += new Vector2(320, 320);

    topLeft = RoundTo32(topLeft);
    bottomRight = RoundTo32(bottomRight);

    var astar = new AStar2D();
    var pointIds = new Dictionary<Vector2, int>();
    int lastId = 0;

    var f = new PhysicsPointQueryParameters2D();
    f.Exclude = new Godot.Collections.Array<RID>();
    f.CollideWithAreas = true;
    f.CollideWithBodies = true;
    f.CollisionMask = 2;

    for (int x = (int)topLeft.x; x < bottomRight.x; x += cellSize) {
      for (int y = (int)topLeft.y; y < bottomRight.y; y += cellSize) {
        var point = new Vector2(x, y);
        f.Position = point;

        var collisions = GetWorld2d().DirectSpaceState.IntersectPoint(f);

        if (collisions.Count > 0) {
          continue;
        }

        pointIds[point] = lastId++;
        astar.AddPoint(lastId, point);

        var neighbors = new Vector2[] {
          new Vector2(x + cellSize, y),
          new Vector2(x - cellSize, y),
          new Vector2(x, y + cellSize),
          new Vector2(x, y - cellSize),
          new Vector2(x + cellSize, y + cellSize),
          new Vector2(x - cellSize, y - cellSize),
          new Vector2(x + cellSize, y - cellSize),
          new Vector2(x - cellSize, y + cellSize),
        };

        foreach (Vector2 neighbor in neighbors) {
          if (pointIds.ContainsKey(neighbor)) {
            astar.ConnectPoints(pointIds[point], pointIds[neighbor]);
          }
        }
      }
    }

    if (!pointIds.ContainsKey(start) || !pointIds.ContainsKey(end)) {
      GD.Print("No path found");

      return new List<Vector2>();
    }

    return astar.GetPointPath(pointIds[start], pointIds[end]).ToList();
  }
}
