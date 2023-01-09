using System;
using System.Collections.Generic;
using Godot;

public partial class House : Node2D, IBuilding, IProvidesSupply, ISelectable, IDamageable {
  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "House";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");

  public Dictionary<string, Action> actions {
    get {
      return new Dictionary<string, Action>() { };
    }
  }

  public int Supply => 4;

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public string name { get; set; } = "House";
  public string selectionText => "This is a house. It provides 4 supply.";

  public Node2D node => this;

  public int health { get; set; }
  public int maxHealth { get; set; }
  public ProgressBar healthBar => GetNode<ProgressBar>("HealthBar");

  public override void _Ready() {
    var stats = Util.BuildingStats[
      BuildingType.House
    ];

    health = stats.health;
    maxHealth = stats.health;
    SelectionCircle.Unit = this;
  }

  public override void _Process(double delta) {
  }

  public void OnHoverEnd() {
  }

  public void OnHoverStart() {
  }
}
