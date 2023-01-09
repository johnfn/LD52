using Godot;
using System.Collections.Generic;
using System;

public enum UnitType {
  None,
  Ant,
  Beetle,
  Scout,
  Spit
}

public enum BuildingStatus {
  Idle,
  Building
}

// This is currently either TrainingBuilding or BugBarracks
public partial class TrainingBuilding : Node2D, IBuilding, ISelectable, ICollider, IDamageable, IProvidesSupply {
  public ErrorPopup errorPopup => GetTree().Root.GetNode<ErrorPopup>("Root/Static/UIRoot/error_panel");

  [Export]
  public bool IsBugBarracks;
  public ProgressBar healthBar => GetNode<ProgressBar>("HealthBar");

  // ISelectable
  public Dictionary<string, Action> actions {
    get {
      if (!IsBugBarracks) {
        return new Dictionary<string, Action>() {
          ["Ant"] = () => {
            BuyUnit(UnitType.Ant);
          },
        };
      }

      return new Dictionary<string, Action>() {
        ["Beetle"] = () => {
          BuyUnit(UnitType.Beetle);
        },

        ["Scout"] = () => {
          BuyUnit(UnitType.Scout);
        },

        ["Spit"] = () => {
          BuyUnit(UnitType.Spit);
        },
      };
    }
  }

  public Node2D node { get => this; }

  public string selectionText {
    get {
      if (!IsBugBarracks) {
        return "This is a Town Hall, where you can train ants to build and harvest. This building provides 8 supply.";
      } else {
        return "This is a Bug Barracks.";
      }
    }
  }


  public string name { get; set; } = "Training Building";

  // IBuilding
  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 0.1f;
  public string unitName {
    get {
      if (IsBugBarracks) {
        return "Bug Barracks";
      } else {
        return "Town Hall";
      }
    }
  }
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;
  public UnitType currentBuildingUnitType { get; set; } = UnitType.None;
  public Area2D colliderShape {
    get {
      return _shape;
    }
  }

  public int health { get; set; }
  public int maxHealth { get; set; }

  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");

  public int Supply {
    get {
      if (IsBugBarracks) {
        return 0;
      } else {
        return 6;
      }
    }
  }

  public void OnHoverStart() {
    // Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    // Modulate = new Color(1, 1, 1, 1f);
  }

  // Locals

  private Area2D _shape;
  private UiPanel _uiPanel;

  public override void _Ready() {
    var stats = Util.BuildingStats[
      IsBugBarracks ? BuildingType.Barracks : BuildingType.TownHall
    ];

    health = stats.health;
    maxHealth = stats.health;
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");
    SelectionCircle.Unit = this;
  }

  public override void _Process(double delta) {
    if (status == BuildingStatus.Building) {
      buildProgress += (float)delta;

      if (buildProgress >= buildTime) {
        status = BuildingStatus.Idle;
        buildProgress = 0;

        var unitStats = Util.UnitStats[currentBuildingUnitType];
        var scene = GD.Load<PackedScene>(unitStats.resourcePath);
        var newUnit = scene.Instantiate<Node2D>();

        newUnit.Position = Util.FindSafeSpaceNear(GetTree(), GlobalPosition, true);

        Util.Add(GetTree(), newUnit);

        currentBuildingUnitType = UnitType.None;
      }
    }
  }

  public void BuyUnit(UnitType unit) {
    var stats = Util.UnitStats[unit];

    if (stats.twigCost > Globals.MatchstickCount) {
      errorPopup.ShowError("Not enough matchsticks.");

      return;
    }

    if (stats.meatCost > Globals.GummyCount) {
      errorPopup.ShowError("Not enough gummies.");

      return;
    }

    var (usedSupply, totalSupply) = ResourcePanel.CalculateSupply(GetTree());

    if (usedSupply >= totalSupply) {
      errorPopup.ShowError("Build more houses!");

      return;
    }

    Globals.MatchstickCount -= stats.twigCost;
    Globals.GummyCount -= stats.meatCost;

    status = BuildingStatus.Building;

    buildProgress = 0;
    buildTime = 0.1f;
    currentBuildingUnitType = unit;
  }
}
