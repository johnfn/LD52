using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GuardTower : Node2D, IBuilding, ISelectable {
  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>();

  public string name { get; set; } = "Guard Tower";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "Guard Tower";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");

  public string selectionText {
    get {
      return "Pew pew pew";
    }
  }

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  private int _damage = 5;
  private float _range = 500f;
  private float _cooldown = 0f;
  private float _maxCooldown = 3f;

  public override void _Ready() {
    SelectionCircle.Unit = this;
  }

  public override void _Process(double delta) {
    _cooldown -= (float)delta;

    if (_cooldown <= 0) {
      _cooldown = _maxCooldown;

      var enemies = GetTree().GetNodesInGroup(GroupNames.BadUnit);
      var closestEnemyDumb = enemies.ToList().OrderBy(x => ((Node2D)x).GlobalPosition.DistanceTo(Position)).FirstOrDefault();
      var closestEnemy = (Node2D)closestEnemyDumb;

      if (closestEnemy == null) {
        return;
      }

      if (closestEnemy.GlobalPosition.DistanceTo(GlobalPosition) > _range) {
        return;
      }

      if (closestEnemy is IDamageable id) {
        var bullet = GD.Load<PackedScene>("res://scenes/bullet.tscn").Instantiate<Bullet>();

        bullet.GlobalPosition = GlobalPosition;
        bullet.Target = id;
        bullet.Damage = _damage;
        bullet.Source = this;

        Util.Add(GetTree(), bullet);
      }

    }
  }

  public void OnHoverStart() {
  }

  public void OnHoverEnd() {
  }
}
