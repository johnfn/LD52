using Godot;

public partial class ResourcePanel : VBoxContainer {
  Label twigLabel;
  Label meatLabel;
  Label supplyLabel => GetNode<Label>("SupplyContainer/SupplyLabel");

  public override void _Ready() {
    twigLabel = GetNode<Label>("TwigContainer/TwigLabel");
    meatLabel = GetNode<Label>("MeatContainer/MeatLabel");
  }

  public static (int usedSupply, int totalSupply) CalculateSupply(SceneTree tree) {
    var allBuildings = tree.GetNodesInGroup(GroupNames.Building);
    var usedSupply = tree.GetNodesInGroup(GroupNames.GoodUnit).Count;
    var totalSupply = 0;

    foreach (Node2D building in allBuildings) {
      if (building is IProvidesSupply s) {
        totalSupply += s.Supply;
      }
    }

    return (usedSupply, totalSupply);
  }

  public override void _Process(double delta) {
    twigLabel.Text = Globals.MatchstickCount.ToString();
    meatLabel.Text = Globals.GummyCount.ToString();

    // calculate supply 

    var (usedSupply, totalSupply) = CalculateSupply(GetTree());

    supplyLabel.Text = $"{usedSupply} / {totalSupply}";
  }
}
