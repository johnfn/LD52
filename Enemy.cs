using Godot;
using System.Collections.Generic;
using System;
using System.Linq;
using static Godot.Tween;

public enum EnemyStatus {
  SeekingTarget,
  WalkingTowardsTarget,
  Attacking
}


public partial class Enemy : Sprite2D, ISelectable, IDamageable {

  public Node2D node { get => this; }
  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() { };

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public int health { get; set; }
  public int maxHealth { get; set; }

  public string name { get; set; } = "Enemy";

  public EnemyStatus _status = EnemyStatus.SeekingTarget;

  public int _speed = 500;
  private float _attackCooldownMax;
  private float _attackCooldownCurrent;
  private int _damageAmount;

  public string selectionText {
    get {
      return "Just some enemy";
    }
  }

  public string unitName => "Enemy";
  private Tween attackTween;
  private Tween takeDamageTween;
  private Color _originalModColor;

  public override void _Ready() {
    health = 10;
    maxHealth = 10;
    _damageAmount = 5;

    _attackCooldownCurrent = 0;
    _attackCooldownMax = 1;
    _originalModColor = Modulate;
    takeDamageTween = CreateTween();
    attackTween = CreateTween();
  }

  private List<Vector2> _path;

  private IDamageable _target;

  public override void _Process(double delta) {
    GD.Print(_status);

    if (
      _status == EnemyStatus.Attacking &&
      (_target == null || _target.health <= 0 || !IsInstanceValid(_target.node))
    ) {
      _status = EnemyStatus.SeekingTarget;
      _target = null;

      return;
    }

    if (_status == EnemyStatus.SeekingTarget) {
      var buildings = GetTree().GetNodesInGroup(GroupNames.Building).ToList<Node>().Select(t => (Node2D)t);
      var goodUnits = GetTree().GetNodesInGroup(GroupNames.GoodUnit).ToList<Node>().Select(t => (Node2D)t);

      var targets = new List<Node2D>();

      targets.AddRange(buildings);
      targets.AddRange(goodUnits);

      var closestTargets = targets.OrderBy(t => t.GlobalPosition.DistanceTo(GlobalPosition));

      IDamageable closestTarget = null;

      foreach (var target in closestTargets) {
        if (target is IDamageable unit) {
          closestTarget = unit;
          break;
        }
      }

      if (closestTarget == null) {
        GD.Print("No target");

        return;
      }

      _path = Util.Pathfind(
        GetTree(),
        GlobalPosition,
        closestTarget.node.GlobalPosition
      );
      _status = EnemyStatus.WalkingTowardsTarget;
      _target = closestTarget;

      return;
    }

    if (_status == EnemyStatus.WalkingTowardsTarget) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _status = EnemyStatus.Attacking;
      }

      return;
    }

    if (_status == EnemyStatus.Attacking) {
      if (_attackCooldownCurrent > 0) {
        _attackCooldownCurrent -= (float)delta;
      } else {
        _target.Damage(_damageAmount);
        _attackCooldownCurrent = _attackCooldownMax;
        
        attackTween.TweenProperty(this, "scale", 1.2f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);
        attackTween.TweenInterval(.05);
        attackTween.TweenProperty(this, "scale", 1f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);

        if (_target.health <= 0) {
          _status = EnemyStatus.SeekingTarget;
          _target = null;
        }
      }
    }
  }

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public void Damage(int amount) {
    health -= amount;

    takeDamageTween.TweenProperty(this, "modulate", new Color(1.0f, 0.0f, 0.0f, 1.0f), .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);
    takeDamageTween.TweenInterval(.1);
    takeDamageTween.TweenProperty(this, "modulate", _originalModColor, .05).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);

    GD.Print("Enemy health is now: " + health);

    if (health <= 0) {
      QueueFree();
    }
  }
}
