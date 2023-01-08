using Godot;
using System.Collections.Generic;
using System;

public partial class Enemy : Sprite2D, ISelectable, IUnit {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {

  };

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public int health { get; set; }
  public int maxHealth { get; set; }

  public string name { get; set; } = "Enemy";

  public string selectionText {
    get {
      return "Just some enemy";
    }
  }

  public string unitName => "Enemy";

  public override void _Ready() {
    health = 10;
    maxHealth = 10;
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
    health -= amount;

    if (health <= 0) {
      QueueFree();
    }
  }
}
