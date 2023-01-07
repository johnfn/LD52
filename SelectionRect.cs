using Godot;

public partial class SelectionRect : Node2D {

  Line2D Rect;

  public override void _Ready() {
    Rect = GetNode<Line2D>("Rect");

    DrawRectangle(new Vector2(0, 0), new Vector2(100, 100));
  }

  public void DrawRectangle(
    Vector2 topLeft,
    Vector2 bottomRight
  ) {
    Rect.ClearPoints();

    Rect.AddPoint(topLeft);
    Rect.AddPoint(new Vector2(bottomRight.x, topLeft.y));
    Rect.AddPoint(bottomRight);
    Rect.AddPoint(new Vector2(topLeft.x, bottomRight.y));
    Rect.AddPoint(topLeft);
  }

  public override void _Process(double delta) {
  }
}
