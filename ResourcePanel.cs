using Godot;

public partial class ResourcePanel : Panel {

  Label twigLabel;
  Label meatLabel;

  public override void _Ready() {
	twigLabel = GetNode<Label>("TwigLabel");
	meatLabel = GetNode<Label>("MeatLabel");
  }

  public override void _Process(double delta) {
	twigLabel.Text = "Twigs: " + Globals.TwigCount.ToString();
	meatLabel.Text = "Meat: " + Globals.MeatCount.ToString();
  }
}
