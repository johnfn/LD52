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

}


public interface IResource {
  public int amount { get; set; }
  public float harvestTime { get; set; }
  public Vector2 resourceGlobalPosition { get; }
  public ResourceType resourceType { get; set; }
}

public interface IUnit {
  public string unitName { get; }
  public int health { get; set; }
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