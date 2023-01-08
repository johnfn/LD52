using Godot;

public partial class ResourcePanel : VBoxContainer {

  Label twigLabel;
  Label meatLabel;

  public override void _Ready() {
    twigLabel = GetNode<Label>("TwigContainer/TwigLabel");
    meatLabel = GetNode<Label>("MeatContainer/MeatLabel");
  }

  public override void _Process(double delta) {
    twigLabel.Text = Globals.TwigCount.ToString();
    meatLabel.Text = Globals.MeatCount.ToString();
  }
}
