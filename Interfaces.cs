using Godot;
using System.Collections.Generic;
using System;

public enum ResourceType {
  Twig,
  Meat,
}

public enum BuildingType {
  TownHall,
  ResourceDepot,
  Barrachnid,
  None,
}

public struct BuildingStats {
  public float buildTime;
  public string name;
  public int health;
  public int twigCost;
  public int meatCost;
  public string resourcePath;
  public string description;
}

public struct UnitStats {
  public float buildTime;
  public int health;
  public int twigCost;
  public int meatCost;
  public string resourcePath;
  public string description;
  public string name;
  public int attackCooldown;
  public int damage;
}

public interface ISelectable {
  public Dictionary<string, Action> actions { get; }

  public bool isHoverable { get; set; }
  /** Unused today */
  public int priority { get; set; }

  public void OnHoverEnd();
  public void OnHoverStart();

  public string name { get; set; }

  public string selectionText { get; }
}


public interface IResource {
  public int amount { get; set; }
  public float harvestTime { get; set; }
  public Vector2 resourceGlobalPosition { get; }
  public ResourceType resourceType { get; set; }
  public AnimationPlayer animationPlayer { get; }
}

public interface IDamageable {
  public string unitName { get; }
  public int health { get; set; }
  public int maxHealth { get; set; }

  public void Damage(int amount, Node2D source) {
    health -= amount;

    this.healthBar.SetProgress((float)health / (float)maxHealth);

    if (health <= 0) {
      node.QueueFree();
    } else {
      node.GetNode<Node2D>("Graphics").Modulate = new Color(5000f, 0f, 0f, 1f);

      // Tween modulate back to normal

      var tween = node.GetTree().CreateTween();
      tween.TweenProperty(
          node.GetNode<Node2D>("Graphics"),
          "modulate",
          new Color(1, 1, 1, 1),
          0.3f
      );
      tween.SetEase(Tween.EaseType.Out);
    }
  }

  public Node2D node { get; }
  public ProgressBar healthBar { get; }
}

public interface IBuilding {
  public float buildProgress { get; set; }
  public float buildTime { get; set; }
  public string unitName { get; }
  public BuildingStatus status { get; set; }
}

public interface ICollider {
  public Area2D colliderShape { get; }
}