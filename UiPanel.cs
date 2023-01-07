using System.Collections.Generic;
using System.Linq;
using Godot;

public enum GameMode {
  Build,
  Command,
}

public partial class UiPanel : Panel {
  public GameMode gameMode = GameMode.Command;
  public ISelectable selectedUnit = null;
  public ISelectable hoveredUnit = null;
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

      if (a.InventoryItem != null) {
        builderPanelLabel.Text += " - Holding " + a.InventoryItem.resourceType;
      }
    }

    townHallPanel.Visible = townHallPanelVisible;
    buildPanel.Visible = buildPanelVisible;
    unitPanel.Visible = unitPanelVisible;
    builderPanel.Visible = builderPanelVisible;
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);

    if (@event is InputEventMouseButton mouseEvent) {
      _handleMouseDown(mouseEvent);
    }

    if (@event is InputEventMouseMotion mouseMotionEvent) {
      _handleMouseMove(mouseMotionEvent);
    }
  }

  private void _handleMouseDown(InputEventMouseButton mouseEvent) {
    if (!mouseEvent.Pressed) {
      return;
    }

    if (gameMode == GameMode.Build) {
      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        selectedBuilding.Modulate = new Color(1, 1, 1, 1);
        selectedBuilding = null;
        gameMode = GameMode.Command;

        return;
      }
    }

    if (gameMode == GameMode.Command) {
      // Issue action like Move
      if (mouseEvent.ButtonIndex == MouseButton.Right) {
        if (selectedUnit == null) {
          return;
        }

        var hoveredUnit = GetHoveredUnit();

        if (hoveredUnit is IResource resource) {
          if (selectedUnit is Ant a) {
            a.Harvest(resource);
          }
        } else {
          if (selectedUnit is Ant a) {
            a.Move(Util.MousePosition(GetTree()));
          }
        }

        return;
      }

      // Select
      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        var unit = GetHoveredUnit();

        if (unit is ISelectable s) {
          selectedUnit = s;
          showCommandUi();
        }
      }
    }
  }

  private void _handleMouseMove(InputEventMouseMotion mouseMotionEvent) {
    if (gameMode == GameMode.Build) {
      var pos = Util.MousePosition(GetTree());
      var roundedTo32 = new Vector2((int)(pos.x / 32) * 32, (int)(pos.y / 32) * 32);

      selectedBuilding.Position = roundedTo32;
    }

    if (gameMode == GameMode.Command) {
      var prevHoveredUnit = hoveredUnit;

      hoveredUnit = GetHoveredUnit();

      if (prevHoveredUnit != hoveredUnit) {
        if (prevHoveredUnit != null) {
          prevHoveredUnit.OnHoverEnd();
        }

        if (hoveredUnit != null) {
          hoveredUnit.OnHoverStart();
        }
      }
    }
  }

  public ISelectable GetHoveredUnit() {
    var mousePosition = Util.MousePosition(GetTree());
    var param = new PhysicsPointQueryParameters2D();

    param.Exclude = new Godot.Collections.Array<RID>();
    param.CollideWithAreas = true;
    param.CollideWithBodies = true;
    param.Position = mousePosition;

    List<Node2D> collisions = GetWorld2d().DirectSpaceState.IntersectPoint(param)
      .Select(x => (Node2D)x["collider"])
      .ToList();

    ISelectable best = null;

    // TODO: Maybe handle priority or something

    foreach (var collision in collisions) {
      if (collision.GetParent() is ISelectable c) {
        best = c;
      }
    }

    return best;
  }

  public override void _Process(double delta) {
    if (gameMode == GameMode.Command) {
      showCommandUi();
    }
  }
}
