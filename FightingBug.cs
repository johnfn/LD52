using System;
using System.Collections.Generic;
using Godot;

public enum AttackStatus {
  None,
  GoingToTarget,
  Attacking
}

public partial class FightingBug : Node2D, IDamageable, ISelectable {
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
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");
  public ProgressBar healthBar => GetNode<ProgressBar>("HealthBar");

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

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() { };

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

  public override void _Ready() {
    _originalModColor = Modulate;
    //attackTween = CreateTween();
    //takeDamageTween = CreateTween();

    var stats = Util.UnitStats[unitType];

    health = stats.health;
    maxHealth = stats.health;

    _attackCooldownMax = stats.attackCooldown;
    _damage = stats.damage;
    SelectionCircle.Unit = this;
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

        if (_attackTarget is IDamageable id) {
          id.Damage(_damage, this);
        }

        AnimateAttack();
      }
    }
  }

  public void OnHoverEnd() {
    // Modulate = new Color(1, 1, 1, 1f);
  }

  public void OnHoverStart() {
    // Modulate = new Color(1, 1, 1, 0.5f);
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

  //takeDamageTween.TweenProperty(this, "modulate", new Color(1.0f, 0.0f, 0.0f, 1.0f), .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);
  //takeDamageTween.TweenInterval(.1);
  //takeDamageTween.TweenProperty(this, "modulate", _originalModColor, .05).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);

  private void AnimateAttack() {
    //attackTween.TweenProperty(this, "scale", 1.2f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.In);
    //attackTween.TweenInterval(.05);
    //attackTween.TweenProperty(this, "scale", 1f, .1).SetTrans(TransitionType.Elastic).SetEase(EaseType.Out);
  }

  public void Damage(int amount, Node2D source) {
    // This crashes Godot instantly!
    // IDamageable id = this;
    // id.Damage(amount, source);

    health -= amount;

    this.healthBar.SetProgress((float)health / (float)maxHealth);

    if (health <= 0) {
      node.QueueFree();
    }

    if (this._status != UnitStatus.Attacking) {
      if (source is Enemy e) {
        Attack(e);
      }
    }
  }
}
