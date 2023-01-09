using Godot;

public partial class ResourcePanel : VBoxContainer {
  Label twigLabel;
  Label meatLabel;
  Label supplyLabel => GetNode<Label>("SupplyContainer/SupplyLabel");

  public override void _Ready() {
    twigLabel = GetNode<Label>("TwigContainer/TwigLabel");
    meatLabel = GetNode<Label>("MeatContainer/MeatLabel");
  }

  public override void _Process(double delta) {
    twigLabel.Text = Globals.MatchstickCount.ToString();
    meatLabel.Text = Globals.GummyCount.ToString();

    // calculate supply 

    var allBuildings = GetTree().GetNodesInGroup(GroupNames.Building);
    var usedSupply = 0;
    var totalSupply = 0;

    foreach (Node2D building in allBuildings) {
      if (building is IProvidesSupply s) {
        totalSupply += s.Supply;
      }
    }

    supplyLabel.Text = $"{usedSupply} / {totalSupply}";
  }
}
