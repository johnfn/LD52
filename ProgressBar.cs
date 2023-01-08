using Godot;

public partial class ProgressBar : Node2D {
  ColorRect Background;
  ColorRect Progress;

  public override void _Ready() {
    Background = GetNode<ColorRect>("Background");
    Progress = GetNode<ColorRect>("Progress");
  }

  public void SetProgress(float progress) {
    Progress.Scale = new Vector2(progress, 1);
  }
}
