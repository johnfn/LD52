using Godot;
using System.Collections.Generic;
using System;

public partial class Enemy : Sprite2D, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {

  };

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  private int _health = 10;

  public string name { get; set; } = "Enemy";

  public override void _Ready() {
  }

  public override void _Process(double delta) {
  }

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public void Damage(int amount) {
    _health -= amount;

    GD.Print("Enemy health is now: " + _health);

    if (_health <= 0) {
      QueueFree();
    }
  }
}