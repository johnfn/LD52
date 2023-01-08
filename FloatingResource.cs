using Godot;

public partial class FloatingResource : Node2D {
  public AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
  public Label Label => GetNode<Label>("Graphics/Label");

  public override void _Ready() {
    Label.Text = $"+{Globals.TwigHarvestRate}";
    AnimationPlayer.Play("Play");

    AnimationPlayer.Connect("animation_finished", Callable.From(() => QueueFree()));
  }
}
