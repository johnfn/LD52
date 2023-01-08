using Godot;

public partial class Enemy : Sprite2D, ISelectable {
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

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
}
