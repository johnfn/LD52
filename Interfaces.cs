using Godot;

public interface ISelectable {
  public bool isHoverable { get; set; }
  public int priority { get; set; }

  public void OnHoverEnd();
  public void OnHoverStart();
}

public enum ResourceType {
  Twig,
}

public interface IResource {
  public int amount { get; set; }
  public float harvestTime { get; set; }
  public Vector2 resourceGlobalPosition { get; }
  public ResourceType resourceType { get; set; }
}

public interface IUnit {
  public string unitName { get; set; }
  public int health { get; set; }
}

public interface IBuilding {
  public float buildProgress { get; set; }
  public float buildTime { get; set; }
  public string unitName { get; set; }
  public BuildingStatus status { get; set; }
}

public interface ICollider {
  public Area2D colliderShape { get; }
}