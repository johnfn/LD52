using Godot;

public partial class ClickAnimation : Node2D {
  public AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

  public void Play() {
    AnimationPlayer.Play("Play");

    // when the animation is done, remove this node
    AnimationPlayer.Connect("animation_finished", Callable.From(() => {
      QueueFree();
    }));
  }
}
