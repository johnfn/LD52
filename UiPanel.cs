using Godot;

public enum GameMode {
  Build,
  Command,
}

public partial class UiPanel : Panel {
  public GameMode gameMode = GameMode.Command;
  public Sprite2D selectedUnit = null;
  public Sprite2D selectedBuilding = null;

  Panel townHallPanel;
  Panel buildPanel;
  Label progressLabel;
  Panel unitPanel;
  Label statsLabel;
  Label selectedUnitLabel;
  Label builderPanelLabel;
  Panel builderPanel;
  Button townHallButton;
  Button townHallBuyGrasshopperButton;

  public override void _Ready() {
    townHallPanel = GetNode<Panel>("TownHallPanel");
    buildPanel = GetNode<Panel>("BuildPanel");
    unitPanel = GetNode<Panel>("UnitPanel");
    progressLabel = GetNode<Label>("BuildPanel/ProgressLabel");
    statsLabel = GetNode<Label>("UnitPanel/StatsLabel");
    selectedUnitLabel = GetNode<Label>("UnitPanel/SelectedUnitLabel");
    builderPanel = GetNode<Panel>("BuilderPanel");
    builderPanelLabel = GetNode<Label>("BuilderPanel/SelectedUnitLabel");
    townHallButton = GetNode<Button>("BuilderPanel/TownHallButton");
    townHallBuyGrasshopperButton = GetNode<Button>("TownHallPanel/BuyGrasshopper");

    showCommandUi();

    townHallBuyGrasshopperButton.Connect("pressed", Callable.From(() => {
      if (selectedUnit is TownHall th) {
        th.BuyUnit(UnitType.Ant);
      }
    }));

    townHallButton.Connect("pressed", Callable.From(() => {
      selectedBuilding = GD.Load<PackedScene>("res://scenes/town_hall.tscn").Instantiate<Sprite2D>();

      selectedBuilding.Modulate = new Color(1, 1, 1, 0.5f);

      GetNode("/root/Root").AddChild(selectedBuilding);
      gameMode = GameMode.Build;
    }));
  }

  private void showCommandUi() {
    var townHallPanelVisible = false;
    var buildPanelVisible = false;
    var unitPanelVisible = false;
    var builderPanelVisible = false;

    if (selectedUnit == null) {
      selectedUnitLabel.Text = "Selected Unit: None";
    }

    // if (selectedUnit is IUnit u) {
    //   unitPanelVisible = true;
    //   selectedUnitLabel.Text = "Selected Unit: " + u.unitName;
    //   selectedUnitName = u.unitName;
    // }

    if (selectedUnit is TownHall th) {
      if (th.status == BuildingStatus.Building) {
        buildPanelVisible = true;
      } else {
        townHallPanelVisible = true;
      }
    }

    if (selectedUnit is Ant a) {
      builderPanelVisible = true;
      builderPanelLabel.Text = "Selected Unit: " + a.unitName;
    }

    townHallPanel.Visible = townHallPanelVisible;
    buildPanel.Visible = buildPanelVisible;
    unitPanel.Visible = unitPanelVisible;
    builderPanel.Visible = builderPanelVisible;
  }

  public void Select(Sprite2D unit) {
    selectedUnit = unit;
    showCommandUi();
  }

  public void move(Vector2 position) {
    if (selectedUnit == null) {
      return;
    }

    if (selectedUnit is Ant antUnit) {
      antUnit.move(position);
    }
  }

  // TODO: All mouse input should be handled through here; things are going to get
  // too confusing otherwise with every node trying to do its own thing!

  public override void _Input(InputEvent @event) {
    base._Input(@event);

    if (@event is InputEventMouseButton mouseEvent) {
      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        if (gameMode == GameMode.Build) {
          selectedBuilding.Modulate = new Color(1, 1, 1, 1);
          selectedBuilding = null;
          gameMode = GameMode.Command;

          return;
        }
      }
    }

    if (@event is InputEventMouseMotion mouseMotionEvent) {
      if (gameMode == GameMode.Build) {
        var pos = mouseMotionEvent.GlobalPosition;
        var roundedTo32 = new Vector2((int)(pos.x / 32) * 32, (int)(pos.y / 32) * 32);

        selectedBuilding.Position = roundedTo32;
      }
    }
  }

  public override void _Process(double delta) {
    if (gameMode == GameMode.Command) {
      showCommandUi();
    }
  }
}
