using Godot;

public partial class FloatingResource : Node2D {
  public AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

  public override void _Ready() {
    AnimationPlayer.Play("Play");

    AnimationPlayer.Connect("animation_finished", Callable.From(() => QueueFree()));
  }
}
