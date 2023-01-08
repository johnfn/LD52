using System.Collections.Generic;
using System.Linq;
using Godot;



public partial class UiPanel : Control {
  public ISelectable hoveredUnit = null;
  // public Sprite2D selectedBuilding = null;
  // public BuildingType selectedBuildingType = BuildingType.None;

  VBoxContainer genericPanel;


  Control townHallPanel;
  Control resourceDepotPanel;
  Control buildPanel;
  Label progressLabel;
  Control unitPanel;
  Control builderPanel;
  Control barracksPanel;
  Button townHallButton;
  Button townHallBuyGrasshopperButton;
  Button depotButton;
  Button barracksButton;

  Panel selectionInfoPanel;
  Panel selectionCommandsPanel;

  Label selectionNameLabel;

  Label selectionLabel;
  Label healthLabel;

  Actions actions;

  public override void _Ready() {
    actions = GetNode<Actions>("/root/Actions");

    // Panels
    selectionCommandsPanel = GetNode<Panel>("VBoxContainer/SelectionDataAndCommands/SelectionCommands");
    selectionLabel = GetNode<Label>("VBoxContainer/SelectionDataAndCommands/SelectionData/Label");
    healthLabel = GetNode<Label>("VBoxContainer/SelectionDataAndCommands/SelectionData/HealthLabel");
    genericPanel = selectionCommandsPanel.GetNode<VBoxContainer>("GenericPanel");
    townHallPanel = selectionCommandsPanel.GetNode<Control>("TownHallPanel");
    resourceDepotPanel = selectionCommandsPanel.GetNode<Control>("ResourceDepotPanel");
    buildPanel = selectionCommandsPanel.GetNode<Control>("BuildPanel");
    unitPanel = selectionCommandsPanel.GetNode<Control>("UnitPanel");
    builderPanel = selectionCommandsPanel.GetNode<Control>("BuilderPanel");
    barracksPanel = selectionCommandsPanel.GetNode<Control>("BarracksPanel");


    // NameTag
    selectionInfoPanel = GetNode<Panel>("VBoxContainer/SelectionNameTag/IconPanel");
    selectionInfoPanel.ThemeTypeVariation = "RoundedPanel";
    selectionNameLabel = GetNode<Label>("VBoxContainer/SelectionNameTag/NamePanel/Label");

    // Misc stuff I havent looked at yet
    progressLabel = buildPanel.GetNode<Label>("ProgressLabel");
    townHallButton = builderPanel.GetNode<Button>("TownHallButton");
    depotButton = builderPanel.GetNode<Button>("ResourceDepot");
    townHallBuyGrasshopperButton = townHallPanel.GetNode<Button>("BuyGrasshopper");
    barracksButton = builderPanel.GetNode<Button>("BarracksButton");

    _showCommandUi();

    // townHallBuyGrasshopperButton.Connect("pressed", Callable.From(() => {
    //     if (Globals.selectedUnit is TownHall th) {
    //         th.BuyUnit(UnitType.Ant);
    //     }
    // }));

    // townHallButton.Connect("pressed", Callable.From(() => {
    //     _beginBuilding(BuildingType.TownHall);
    // }));

    // depotButton.Connect("pressed", Callable.From(() => {
    //     _beginBuilding(BuildingType.ResourceDepot);
    // }));
    // depotButton.Connect("pressed", Callable.From(() => {
    //   _beginBuilding(BuildingType.ResourceDepot);
    // }));

    // barracksButton.Connect("pressed", Callable.From(() => {
    //   _beginBuilding(BuildingType.Barrachnid);
    // }));

    // barracksPanel.GetNode<Button>("BeetleButton").Connect("pressed", Callable.From(() => {
    //   if (selectedUnit is TrainingBuilding b) {
    //     b.BuyUnit(UnitType.Beetle);
    //   }
    // }));

    // barracksPanel.GetNode<Button>("ScoutButton").Connect("pressed", Callable.From(() => {
    //   if (selectedUnit is TrainingBuilding b) {
    //     b.BuyUnit(UnitType.Scout);
    //   }
    // }));

    // barracksPanel.GetNode<Button>("SpitButton").Connect("pressed", Callable.From(() => {
    //   if (selectedUnit is TrainingBuilding b) {
    //     b.BuyUnit(UnitType.Spit);
    //   }
    // }));
  }



  private void _showCommandUi() {
    if (Globals.selectedUnit == null) {
      selectionNameLabel.Text = "Selected Unit: None";
    }

    // if (Globals.selectedUnit is IUnit u) {
    //   unitPanelVisible = true;
    //   selectionNameLabel.Text = "Selected Unit: " + u.unitName;
    //   Globals.selectedUnitName = u.unitName;
    // }

    if (Globals.selectedUnit is ISelectable s) {
      foreach (var child in genericPanel.GetChildren()) {
        genericPanel.RemoveChild(child);
      }

      foreach (var item in Globals.selectedUnit.actions) {
        var label = new Button();
        label.Text = item.Key;
        genericPanel.AddChild(label);
        label.Connect("pressed", Godot.Callable.From(item.Value));
      }
    }
  }

  private void _updateLabels() {
    selectionNameLabel.Text = "Unknown";

    if (Globals.selectedUnit is IBuilding b) {
      selectionNameLabel.Text = b.unitName;
    }

    if (Globals.selectedUnit is IDamageable u) {
      selectionNameLabel.Text = u.unitName;

      healthLabel.Text = "Health: " + u.health + "/" + u.maxHealth;
    }

    if (Globals.selectedUnit is TrainingBuilding th) {
      if (th.status == BuildingStatus.Building) {
        selectionNameLabel.Text += "Producing units...)";
      }
    }

    if (Globals.selectedUnit is Ant a) {
      selectionNameLabel.Text = a.unitName;

      if (a.InventoryItem != null) {
        selectionNameLabel.Text += " - Holding " + a.InventoryItem.resourceType;
      }
    }

    if (Globals.selectedUnit is FightingBug fb) {
      selectionNameLabel.Text = fb.unitName;
    }

    if (Globals.selectedUnit is ISelectable ss) {
      selectionLabel.Text = ss.selectionText;
    }
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

    if (Globals.gameMode == GameMode.Build) {
      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        Actions.placeBuilding();

        return;
      }
    }

    if (Globals.gameMode == GameMode.Command) {
      // Issue action like Move

      if (mouseEvent.ButtonIndex == MouseButton.Right) {
        if (Globals.selectedUnit == null) {
          return;
        }

        var hoveredUnit = GetHoveredUnit();

        if (hoveredUnit is IResource resource) {
          if (Globals.selectedUnit is Ant a) {
            a.Harvest(resource);
          }
        } else if (hoveredUnit is ConstructionSite c) {
          if (Globals.selectedUnit is Ant a) {
            a.ContinueToBuild(c);
          } else {
            Actions.errorPopup.ShowError("Only a worker ant can build.");
          }
        } else {
          if (Globals.selectedUnit is Ant a) {
            a.Move(Util.MousePosition(GetTree()));
          }

          if (Globals.selectedUnit is FightingBug fb) {
            if (hoveredUnit is Enemy e) {
              fb.Attack(e);
            } else {
              fb.Move(Util.MousePosition(GetTree()));

            }
          }
        }

        return;
      }

      // Select

      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        var unit = GetHoveredUnit();

        if (unit is ISelectable s) {
          Globals.selectedUnit = s;
          _showCommandUi();
        }
      }
    }
  }

  private void _handleMouseMove(InputEventMouseMotion mouseMotionEvent) {
    if (Globals.gameMode == GameMode.Build) {
      var pos = Util.MousePosition(GetTree());
      var roundedTo32 = new Vector2((int)(pos.x / 32) * 32, (int)(pos.y / 32) * 32);

      Globals.selectedBuilding.Position = roundedTo32;
    }

    if (Globals.gameMode == GameMode.Command) {
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
    if (Globals.gameMode == GameMode.Build) {
      _showCommandUi();
    }

    _updateLabels();
  }
}
