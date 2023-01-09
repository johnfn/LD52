using System.Collections.Generic;
using Godot;

public partial class HoverPanel : Panel {
  public Label TitleLabel => GetNode<Label>("TitleLabel");
  public Label DescriptionLabel => GetNode<Label>("DescriptionLabel");
  public Label ToothpickCost => GetNode<Label>("ToothpickCost");
  public Label GummyCost => GetNode<Label>("GummyCost");

  public Sprite2D HeartIcon => GetNode<Sprite2D>("HeartIcon");
  public Label Health => GetNode<Label>("Health");
  public Sprite2D AttackIcon => GetNode<Sprite2D>("AttackIcon");
  public Label Damage => GetNode<Label>("Damage");

  public List<Node2D> Graphics => new List<Node2D> {
    GetNode<Node2D>("Graphics/BuildingTownhall"),
    GetNode<Node2D>("Graphics/BuildingDepot"),
    GetNode<Node2D>("Graphics/UnitAnt"),
    GetNode<Node2D>("Graphics/UnitWarrior"),
    GetNode<Node2D>("Graphics/UnitHealer"),
  };

  public void Initialize(string name) {
    foreach (var graphic in Graphics) {
      graphic.Visible = false;
    }

    // Buildings

    if (name == "Town Hall" || name == "Resource Depot") {
      BuildingType type = BuildingType.None;

      if (name == "Town Hall") {
        type = BuildingType.TownHall;
        GetNode<Node2D>("Graphics/BuildingTownhall").Visible = true;
      } else if (name == "Resource Depot") {
        type = BuildingType.ResourceDepot;
        GetNode<Node2D>("Graphics/BuildingDepot").Visible = true;
      }

      var stats = Util.BuildingStats[type];

      TitleLabel.Text = stats.name;
      DescriptionLabel.Text = stats.description;
      ToothpickCost.Text = stats.twigCost.ToString();
      GummyCost.Text = stats.meatCost.ToString();

      HeartIcon.Visible = false;
      Health.Visible = false;
      AttackIcon.Visible = false;
      Damage.Visible = false;

      return;
    }

    if (
      name == "Ant" ||
      name == "Beetle" ||
      name == "Scout" ||
      name == "Spit"
    ) {
      UnitType type = UnitType.None;

      if (name == "Ant") {
        type = UnitType.Ant;
        GetNode<Node2D>("Graphics/UnitAnt").Visible = true;
      } else if (name == "Beetle") {
        type = UnitType.Beetle;
        GetNode<Node2D>("Graphics/UnitWarrior").Visible = true;
      } else if (name == "Scout") {
        type = UnitType.Scout;
        GetNode<Node2D>("Graphics/UnitWarrior").Visible = true;
      } else if (name == "Spit") {
        type = UnitType.Spit;
        GetNode<Node2D>("Graphics/UnitHealer").Visible = true;
      }

      var stats = Util.UnitStats[type];

      TitleLabel.Text = stats.name;
      DescriptionLabel.Text = stats.description;
      ToothpickCost.Text = stats.twigCost.ToString();
      GummyCost.Text = stats.meatCost.ToString();

      HeartIcon.Visible = true;
      Health.Visible = true;
      AttackIcon.Visible = true;
      Damage.Visible = true;

      Health.Text = stats.health.ToString();
      Damage.Text = stats.damage.ToString();

      return;
    }
  }
}
