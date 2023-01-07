using Godot;

public partial class ResourcePanel : Panel {
  public int TwigCount { get; set; } = Util.DEBUG ? 100 : 0;
  public int MeatCount { get; set; } = 0;

  Label twigLabel;
  Label meatLabel;

  public override void _Ready() {
    twigLabel = GetNode<Label>("TwigLabel");
    meatLabel = GetNode<Label>("MeatLabel");
  }

  public override void _Process(double delta) {
    twigLabel.Text = "Twigs: " + TwigCount.ToString();
    meatLabel.Text = "Meat: " + MeatCount.ToString();
  }
}
