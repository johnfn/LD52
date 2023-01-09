using Godot;

public partial class Bullet : Node2D {
  public IDamageable Target { get; set; }
  public Node2D Source;
  public int Damage = 1;
  public Vector2 LastTargetLocation;

  public override void _Process(double delta) {
    Vector2 destination;

    if (
      Target == null ||
      !IsInstanceValid(Target.node)
    ) {
      destination = LastTargetLocation;
    } else {
      LastTargetLocation = Target.node.GlobalPosition;
    }

    GlobalPosition += (LastTargetLocation - GlobalPosition).Normalized() * 400 * (float)delta;

    if (GlobalPosition.DistanceTo(LastTargetLocation) < 50f) {
      if (Target != null && IsInstanceValid(Target.node)) {
        Target.Damage(Damage, Source);
      }

      QueueFree();
    }
  }
}
