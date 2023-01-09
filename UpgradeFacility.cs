using Godot;
using System;
using System.Collections.Generic;

public enum UpgradeType {
  None,
  BugStrengthI,
  BugStrengthII,
  BugStrengthIII,

  FastMatchstickI,
  FastMatchstickII,
  FastMatchstickIII,

  FastGummyI,
  FastGummyII,
  FastGummyIII,
}

public class Upgrade {
  public UpgradeType type;
  public int matchstickCost;
  public int gummyCost;
  public float time;
  public string description;
  public string name;
  public Action onComplete;
}

public partial class UpgradeFacility : Node2D, IBuilding, ISelectable {
  public string name { get; set; } = "Upgrade Facility";

  public float buildProgress { get; set; } = 0;
  public float buildTime { get; set; } = 5f;
  public string unitName { get; set; } = "Upgrade Facility";
  public BuildingStatus status { get; set; } = BuildingStatus.Idle;
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");
  public ProgressBar progressBar => GetNode<ProgressBar>("ProgressBar");
  private UiPanel _uiPanel => GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");

  public string selectionText {
    get {
      if (_currentUpgrade == null) {
        return "Research new bug-technologies...";
      }

      return "Researching: " + _currentUpgrade.name;
    }
  }

  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  private Upgrade _currentUpgrade = null;
  private float _upgradeTimeLeft = 0f;

  public override void _Ready() {
    SelectionCircle.Unit = this;
  }

  public void OnHoverStart() {
  }

  public void OnHoverEnd() {
  }

  public Dictionary<string, Action> actions {
    get {
      var result = new Dictionary<string, Action>();

      if (_currentUpgrade != null) {
        return result;
      }

      if (Upgrades.BugStrengthLevel == 0) {
        result.Add("Bug Strength I", () => {
          BuyUpgrade(UpgradeType.BugStrengthI);
        });
      }

      if (Upgrades.BugStrengthLevel == 1) {
        result.Add("Bug Strength II", () => {
          BuyUpgrade(UpgradeType.BugStrengthII);
        });
      }

      if (Upgrades.BugStrengthLevel == 2) {
        result.Add("Bug Strength III", () => {
          BuyUpgrade(UpgradeType.BugStrengthIII);
        });
      }

      return result;
    }
  }

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
  }
}
