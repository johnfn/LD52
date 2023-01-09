using System;
using Godot;

public partial class NextWavePanel : Panel {
  public Label TimeLeftLabel => GetNode<Label>("Label");
  public float TimeTillNextWave = 5;
  public float InitialTimeTillNextWave = 5;
  public ColorRect Progress => GetNode<ColorRect>("Progress");

  public override void _Ready() {
  }

  public override void _Process(double delta) {
    Progress.Scale = new Vector2(
      1 - (TimeTillNextWave / InitialTimeTillNextWave),
      1
    );

    if (TimeTillNextWave < 0) {
      // Already started next wave
      return;
    }

    TimeTillNextWave -= (float)delta;

    if (TimeTillNextWave < 0) {
      _startNextWave();
      TimeLeftLabel.Text = "Next wave started!";
    } else {
      TimeLeftLabel.Text = $"Next wave in {TimeTillNextWave.ToString("0.0")}s";
    }
  }

  private Vector2 _getPointInDarkness(
    Godot.Collections.Array<Node> allColliders
  ) {
    var spawnPosition = Vector2.Zero;

    for (var tries = 0; tries < 50; tries++) {
      var topLeft = Vector2.Zero;
      var bottomRight = Vector2.Zero;

      foreach (Node2D collider in allColliders) {
        topLeft.x = Math.Min(topLeft.x, collider.GlobalPosition.x);
        topLeft.y = Math.Min(topLeft.y, collider.GlobalPosition.y);

        bottomRight.x = Math.Max(bottomRight.x, collider.GlobalPosition.x);
        bottomRight.y = Math.Max(bottomRight.y, collider.GlobalPosition.y);
      }

      var candidatePosition = new Vector2(
        (float)GD.RandRange((double)topLeft.x - 1000, (double)bottomRight.x + 1000),
        (float)GD.RandRange((double)topLeft.y - 1000, (double)bottomRight.y + 1000)
      );

      spawnPosition = Util.FindSafeSpaceNear(
        GetTree(),
        candidatePosition,
        true
      );

      // Ensure we aren't too close to any building, try to spawn in the dark

      var tooClose = false;

      foreach (Node2D collider in allColliders) {
        if (collider.GlobalPosition.DistanceTo(spawnPosition) < 800) {
          tooClose = true;
          break;
        }
      }

      if (!tooClose) {
        break;
      }
    }

    return spawnPosition;
  }

  private void _startNextWave() {
    var allColliders = GetTree().GetNodesInGroup(GroupNames.Collider);

    for (var i = 0; i < 15; i++) {
      var enemy = GD.Load<PackedScene>("res://scenes/enemy.tscn").Instantiate<Enemy>();

      GetTree().Root.AddChild(enemy);

      enemy.GlobalPosition = _getPointInDarkness(allColliders);
    }
  }
}
