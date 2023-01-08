using Godot;
using System.Collections.Generic;
using System;

public partial class Twig : Sprite2D, ISelectable, IResource {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>();

  public string name { get; set; } = "Twig";

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  // IResource
  public int amount { get; set; } = 10;
  public ResourceType resourceType { get; set; } = ResourceType.Twig;

  public Vector2 resourceGlobalPosition {
    get {
      return GlobalPosition;
    }
  }

  public string selectionText {
    get {
      return "Just a Twig!";
    }
  }

  public float harvestTime { get; set; } = Util.DEBUG ? 1f : 5f;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public override void _Ready() {
  }

  public override void _Process(double delta) {
  }
}
