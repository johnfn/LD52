public interface ISelectable {
  public bool isHoverable { get; set; }
  public int priority { get; set; }

  public void OnHoverEnd();
  public void OnHoverStart();
}

public interface IResource {
  public int amount { get; set; }
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