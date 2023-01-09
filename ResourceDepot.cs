using Godot;
using System.Collections.Generic;
using System;
public partial class ResourceDepot : Node2D, IBuilding, ISelectable {
  public Dictionary<string, Action> actions {
    get {
      var result = new Dictionary<string, Action>();

      if (_currentUpgrade != null) {
        return result;
      }

      if (Upgrades.MatchstickSpeedLevel == 0) {
        result.Add("Matchstick Harvesting I", () => {
          BuyUpgrade(UpgradeType.FastMatchstickI);
        });
      }

      if (Upgrades.MatchstickSpeedLevel == 1) {
        result.Add("Matchstick Harvesting II", () => {
          BuyUpgrade(UpgradeType.FastMatchstickII);
        });
      }

      if (Upgrades.MatchstickSpeedLevel == 2) {
        result.Add("Matchstick Harvesting III", () => {
          BuyUpgrade(UpgradeType.FastMatchstickIII);
        });
      }

      if (Upgrades.GummySpeedLevel == 0) {
        result.Add("Gummy Harvesting I", () => {
          BuyUpgrade(UpgradeType.FastGummyI);
        });
      }

      if (Upgrades.GummySpeedLevel == 1) {
        result.Add("Gummy Harvesting II", () => {
          BuyUpgrade(UpgradeType.FastGummyII);
        });
      }

      if (Upgrades.GummySpeedLevel == 2) {
        result.Add("Gummy Harvesting III", () => {
          BuyUpgrade(UpgradeType.FastGummyIII);
        });
      }


      return result;
    }
  }

  public string name { get; set; } = "Resource Depot";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "Resource Depot";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");
  public ProgressBar progressBar => GetNode<ProgressBar>("ProgressBar");
  private UiPanel _uiPanel => GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");

  public string selectionText {
    get {
      if (_currentUpgrade != null) {
        return "Researching: " + _currentUpgrade.name;
      }

      return "A handy place for bugs to drop off resources.";
    }
  }

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public override void _Ready() {
    SelectionCircle.Unit = this;
  }

  public void OnHoverStart() {
  }

  public void OnHoverEnd() {
  }

  private Upgrade _currentUpgrade = null;
  private float _upgradeTimeLeft = 0f;

  public override void _Process(double delta) {
    if (_currentUpgrade == null) {
      return;
    }

    progressBar.SetProgress(1.0f - _upgradeTimeLeft / _currentUpgrade.time);

    _upgradeTimeLeft -= (float)delta;

    if (_upgradeTimeLeft <= 0) {
      Actions.errorPopup.ShowError("Upgrade complete: " + _currentUpgrade.name);

      _currentUpgrade.onComplete.Invoke();
      _currentUpgrade = null;
      _upgradeTimeLeft = 0f;

      _uiPanel._showCommandUi();
    }
  }

  public void BuyUpgrade(UpgradeType upgradeType) {
    var potentialUpgrade = Upgrades.AllUpgrades[upgradeType];

    if (Globals.MatchstickCount < potentialUpgrade.matchstickCost) {
      Actions.errorPopup.ShowError("Not enough matchsticks");
      return;
    }

    if (Globals.GummyCount < potentialUpgrade.gummyCost) {
      Actions.errorPopup.ShowError("Not enough gummies.");
      return;
    }

    Globals.MatchstickCount -= potentialUpgrade.matchstickCost;
    Globals.GummyCount -= potentialUpgrade.gummyCost;

    _currentUpgrade = potentialUpgrade;
    _upgradeTimeLeft = _currentUpgrade.time;

    _uiPanel._showCommandUi();
  }
}
