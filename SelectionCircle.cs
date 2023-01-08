using Godot;

public partial class SelectionCircle : Node2D {
  public Sprite2D Semicircle => GetNode<Sprite2D>("Semicircle");
  public Sprite2D Fullcircle => GetNode<Sprite2D>("Fullcircle");

  public ISelectable Unit = null;

  public override void _Ready() {
    Semicircle.Visible = false;
    Fullcircle.Visible = false;
  }

  public override void _Process(double delta) {
    if (Unit == null) {
      return;
    }

    if (Globals.selectedUnit == Unit) {
      Fullcircle.Visible = true;
      Semicircle.Visible = false;
    } else {
      if (Globals.hoveredUnit == Unit) {
        Semicircle.Visible = true;
      } else {
        Semicircle.Visible = false;
      }

      Fullcircle.Visible = false;
    }
  }
}
