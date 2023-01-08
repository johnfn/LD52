using System;
using System.Collections.Generic;
using Godot;
using static Godot.Tween;

public enum AttackStatus {
  None,
  GoingToTarget,
  Attacking
}

public partial class FightingBug : Sprite2D, IDamageable, ISelectable {
  [Export]
  public UnitType unitType;

  private UnitStatus _status = UnitStatus.Idle;
  private AttackStatus _attackStatus = AttackStatus.None;
  private Enemy _attackTarget = null;
  private int _attackCooldownMax = 0;
  private int _attackCooldownCurrent = 0;
  private int _damage = 0;
  private List<Vector2> _path = new List<Vector2>();
  public Node2D node { get => this; }
  private Color _originalModColor;

  public string selectionText {
    get {
      if (unitType == UnitType.Beetle) {
        return "This is a BEETLE!";
      }

      if (unitType == UnitType.Scout) {
        return "This is a SCOUT!";
      }

      if (unitType == UnitType.Spit) {
        return "This is a SPIT!";
      }

      return "Screw YOU!";
    }
  }

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {
    // ["Town Hall"] = (delegate () {
    //   Actions.selectBuilding(BuildingType.TownHall);
    // }),
    // ["Resource Depot"] = (delegate () {
    //   Actions.selectBuilding(BuildingType.ResourceDepot);
    // }),
  };

  public string unitName {
    get {
      return unitType.ToString();
    }
  }
  public int health { get; set; }
  public int maxHealth { get; set; }
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public string name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

  private int _speed = 500;

  private Tween attackTween;
  private Tween takeDamageTween;

  public override void _Ready()
  {
    _originalModColor = Modulate;
    attackTween = CreateTween();
    takeDamageTween = CreateTween();
    
    var stats = Util.UnitStats[unitType];

    health = stats.health;
    maxHealth = stats.health;

    _attackCooldownMax = stats.attackCooldown;
    _damage = stats.damage;

    GD.Print("damage is" + _damage);
  }

  public override void _Process(double delta) {
    if (_status == UnitStatus.Moving) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _status = UnitStatus.Idle;
      }
    }

    if (_status == UnitStatus.Attacking) {
      _ProcessAttack(delta);
    }
  }

  private void _ProcessAttack(double delta) {
    // If the target died, stop.
    if (_attackTarget == null || !IsInstanceValid(_attackTarget)) {
      _status = UnitStatus.Idle;
      return;
    }

    // If the target moved, pursue.
    if (
      _attackStatus != AttackStatus.GoingToTarget &&
      _attackTarget.GlobalPosition.DistanceTo(GlobalPosition) > 100
    ) {
      _attackStatus = AttackStatus.GoingToTarget;
      _path = Util.Pathfind(GetTree(), GlobalPosition, _attackTarget.GlobalPosition);
    }

    // Otherwise, attack!
    if (_attackStatus == AttackStatus.GoingToTarget) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _attackStatus = AttackStatus.Attacking;
      }
    } else if (_attackStatus == AttackStatus.Attacking) {
      if (_attackCooldownCurrent > 0) {
        _attackCooldownCurrent -= 1;
      } else {
        _attackCooldownCurrent = _attackCooldownMax;
        _attackTarget.Damage(_damage);
        AnimateAttack();
      }
    }
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void Move(Vector2 destination) {
    _status = UnitStatus.Moving;
    _path = Util.Pathfind(GetTree(), GlobalPosition, destination);
  }

  public void Attack(Enemy target) {
    _status = UnitStatus.Attacking;
    _attackStatus = AttackStatus.GoingToTarget;
    _path = Util.Pathfind(GetTree(), GlobalPosition, target.GlobalPosition);
    _attackTarget = target;
  }

  public void Damage(int amount) {
    health -= amount;

    takeDamageTween.TweenProperty(this, "modulate", new Color(1.0f, 0.0f, 0.0f, 1.0f), .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);
    takeDamageTween.TweenInterval(.1);
    takeDamageTween.TweenProperty(this, "modulate", _originalModColor, .05).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);

    if (health <= 0) {
      QueueFree();
    }
  }
  private void AnimateAttack()
  {
    attackTween.TweenProperty(this, "scale", 1.2f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);
    attackTween.TweenInterval(.05);
    attackTween.TweenProperty(this, "scale", 1f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);
  }
}
