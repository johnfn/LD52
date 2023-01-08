using Godot;

public partial class ProgressBar : Node2D {
  ColorRect Background;
  ColorRect Progress;

  private double _sinceLastSetProgress = 0;

  public override void _Ready() {
    Background = GetNode<ColorRect>("Background");
    Progress = GetNode<ColorRect>("Progress");
    Visible = false;
  }

  public void SetProgress(float progress) {
    Progress.Scale = new Vector2(progress, 1);

    Modulate = new Color(1, 1, 1, 1f);
    Visible = true;
    _sinceLastSetProgress = 0;
  }

  public override void _Process(double delta) {
    if (Visible) {
      _sinceLastSetProgress += delta;

      Visible = _sinceLastSetProgress < 3;

      if (_sinceLastSetProgress > 2) {
        Modulate = new Color(1, 1, 1, 1.0f - ((float)_sinceLastSetProgress - 2.0f));
      }
    }
  }
}
