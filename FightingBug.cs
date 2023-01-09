using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum AttackStatus {
  None,
  GoingToTarget,
  Attacking
}

public partial class FightingBug : Node2D, IDamageable, ISelectable, IDealsDamage {
  [Export]
  public UnitType unitType;

  private UnitStatus _status = UnitStatus.Idle;
  private AttackStatus _attackStatus = AttackStatus.None;
  private Enemy _attackTarget = null;
  private int _attackCooldownMax = 0;
  private int _attackCooldownCurrent = 0;
  public int Strength {
    get {
      var stats = Util.UnitStats[unitType];
      return stats.damage + Upgrades.BugStrengthLevel * 2;
    }
  }
  private List<Vector2> _path = new List<Vector2>();
  public Node2D node { get => this; }
  private Color _originalModColor;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");
  public ProgressBar healthBar => GetNode<ProgressBar>("HealthBar");
  public AnimationPlayer animationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

  public string selectionText {
    get {
      if (unitType == UnitType.Beetle) {
        return "This is a beetle, a melee bug fighter.";
      }

      if (unitType == UnitType.Scout) {
        return "This is a scout: Fast, but weak.";
      }

      if (unitType == UnitType.Spit) {
        return "This is a spit bug, a long range fighter!";
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
  private bool _ranged = false;

  private Tween attackTween;
  private Tween takeDamageTween;

  private int _distanceRequiredToAttackEnemy {
    get {
      if (_ranged) {
        return 600;
      }

      return 100;
    }
  }

  public override void _Ready() {
    _originalModColor = Modulate;
    //attackTween = CreateTween();
    //takeDamageTween = CreateTween();

    var stats = Util.UnitStats[unitType];

    health = stats.health;
    maxHealth = stats.health;

    _speed = stats.speed;
    _attackCooldownMax = stats.attackCooldown;
    _ranged = stats.ranged;
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
      _path.Count == 0 || (
        _attackStatus != AttackStatus.GoingToTarget &&
        _attackTarget.GlobalPosition.DistanceTo(GlobalPosition) > _distanceRequiredToAttackEnemy
      )
    ) {
      _attackStatus = AttackStatus.GoingToTarget;
      _path = Util.Pathfind(GetTree(), GlobalPosition, _attackTarget.GlobalPosition);
    }

    // Otherwise, attack!
    if (_attackStatus == AttackStatus.GoingToTarget) {
      Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (
        GlobalPosition.DistanceTo(_attackTarget.GlobalPosition) <= _distanceRequiredToAttackEnemy
      ) {
        _attackStatus = AttackStatus.Attacking;
      }
    } else if (_attackStatus == AttackStatus.Attacking) {
      if (_attackCooldownCurrent > 0) {
        _attackCooldownCurrent -= 1;
      } else {
        _attackCooldownCurrent = _attackCooldownMax;

        if (_ranged) {
          var bullet = GD.Load<PackedScene>("res://scenes/bullet.tscn").Instantiate<Bullet>();

          bullet.GlobalPosition = GlobalPosition;
          bullet.Target = _attackTarget;
          bullet.Damage = Strength;
          bullet.Source = this;

          Util.Add(GetTree(), bullet);
        } else {
          var hitSprite = GetNode<Sprite2D>("Graphics/HitEffect/Sprite2D");
          // rotate hitSprite towards enemy.

          hitSprite.GlobalRotation = (_attackTarget.GlobalPosition - GlobalPosition).Angle();

          animationPlayer.Play("Attack");

          if (_attackTarget is IDamageable id) {
            id.Damage(Strength, this);
          }
        }

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

    _path = _path.SkipLast(2).ToList();
    _attackTarget = target;

    Util.FlashNodeWhite(target);
  }

  public void Damage(int amount, Node2D source) {
    // This crashes Godot instantly!
    // IDamageable id = this;
    // id.Damage(amount, source);

    health -= amount;

    this.healthBar.SetProgress((float)health / (float)maxHealth);

    if (health <= 0) {
      node.QueueFree();
    } else {
      node.GetNode<Node2D>("Graphics").Modulate = new Color(5000f, 0f, 0f, 1f);

      var tween = node.GetTree().CreateTween();
      tween.TweenProperty(
          node.GetNode<Node2D>("Graphics"),
          "modulate",
          new Color(1, 1, 1, 1),
          0.3f
      );
      tween.SetEase(Tween.EaseType.Out);
    }

    if (this._status != UnitStatus.Attacking) {
      if (source is Enemy e) {
        Attack(e);
      }
    }
  }
}
