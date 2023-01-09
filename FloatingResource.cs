using Godot;

public partial class FloatingResource : Node2D {
  public AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
  public Label Label => GetNode<Label>("Graphics/Label");
  public ResourceType ResourceType;

  public Sprite2D GummyGraphic => GetNode<Sprite2D>("Graphics/GummyGraphic");
  public Sprite2D MatchGraphic => GetNode<Sprite2D>("Graphics/MatchGraphic");

  public override void _Ready() {
    if (ResourceType == ResourceType.Meat) {
      Label.Text = $"+{Globals.MeatHarvestRate}";

      GummyGraphic.Visible = true;
      MatchGraphic.Visible = false;
    } else {
      Label.Text = $"+{Globals.TwigHarvestRate}";

      GummyGraphic.Visible = false;
      MatchGraphic.Visible = true;
    }

    AnimationPlayer.Play("Play");
    AnimationPlayer.Connect("animation_finished", Callable.From(() => QueueFree()));
  }
}
