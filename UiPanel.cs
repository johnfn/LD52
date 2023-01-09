using System.Collections.Generic;
using System.Linq;
using Godot;



public partial class UiPanel : Control {
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
  HoverPanel hoverPanel;

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

    hoverPanel = GetNode<HoverPanel>("HoverPanel");
    hoverPanel.Visible = false;

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

        label.Connect("pressed", Godot.Callable.From(() => {
          item.Value.Invoke();
          hoverPanel.Visible = false;
        }));

        label.Connect("mouse_entered", Godot.Callable.From(() => {
          if (Globals.gameMode == GameMode.Command) {
            hoverPanel.Visible = true;
            hoverPanel.GlobalPosition = GetGlobalMousePosition() + new Vector2(0, -400);
            hoverPanel.Initialize(item.Key);
          }
        }));

        label.Connect("mouse_exited", Godot.Callable.From(() => {
          hoverPanel.Visible = false;
        }));
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

    if (@event is InputEventKey keyEvent) {
      _handleKey(keyEvent);
    }
  }

  private void _handleKey(InputEventKey keyEvent) {
    if (keyEvent.Pressed) {
      if (keyEvent.Keycode == Key.Escape) {
        _cancelBuilding();
      }
    }
  }

  private void _cancelBuilding() {
    if (Globals.gameMode == GameMode.Build) {
      Globals.gameMode = GameMode.Command;
      Globals.selectedBuilding.QueueFree();
      Globals.selectedBuildingType = BuildingType.None;
      Globals.selectedBuilding = null;

      hoverPanel.Visible = false;
    }
  }

  private string _currentMouseCursor = "";

  private void _maybeUpdateMouseCursor() {
    var nextCursor = "normal";
    var hoveredUnit = Globals.hoveredUnit;

    if (hoveredUnit != null) {
      nextCursor = "select";
    }

    if (hoveredUnit is IResource ir) {
      if (Globals.selectedUnit is Ant a) {
        nextCursor = "harvest";
      } else if (Globals.selectedUnit is IBuilding b || Globals.selectedUnit is IResource r) {
        nextCursor = "normal";
      } else {
        nextCursor = "fail";
      }
    }

    if (hoveredUnit is Enemy e) {
      nextCursor = "attack";
    }

    if (nextCursor != _currentMouseCursor) {
      _currentMouseCursor = nextCursor;

      if (nextCursor == "normal") {
        Input.SetCustomMouseCursor(
          ResourceLoader.Load("res://assets/cursors/cursor_normal.png")
        );
      } else if (nextCursor == "attack") {
        Input.SetCustomMouseCursor(
          ResourceLoader.Load("res://assets/cursors/cursor_attack.png")
        );
      } else if (nextCursor == "harvest") {
        Input.SetCustomMouseCursor(
          ResourceLoader.Load("res://assets/cursors/cursor_harvest.png")
        );
      } else if (nextCursor == "fail") {
        Input.SetCustomMouseCursor(
          ResourceLoader.Load("res://assets/cursors/cursor_fail.png")
        );
      } else if (nextCursor == "select") {
        Input.SetCustomMouseCursor(
          ResourceLoader.Load("res://assets/cursors/cursor_select.png")
        );
      }
    }
  }

  private void _handleMouseDown(InputEventMouseButton mouseEvent) {
    if (!mouseEvent.Pressed) {
      return;
    }

    if (mouseEvent.ButtonIndex == MouseButton.Right && Globals.selectedUnit != null) {
      var clickAnimation = GD.Load<PackedScene>("res://scenes/click_animation.tscn").Instantiate<ClickAnimation>();
      Util.Add(GetTree(), clickAnimation);
      clickAnimation.Play();
      clickAnimation.GlobalPosition = Util.MousePosition(GetTree());
    }

    if (Globals.gameMode == GameMode.Build) {
      if (mouseEvent.ButtonIndex == MouseButton.Left) {
        Actions.placeBuilding(this);

        hoverPanel.Visible = false;
        return;
      }
    }

    if (Globals.gameMode == GameMode.Command) {
      // Issue action like Move

      if (mouseEvent.ButtonIndex == MouseButton.Right) {
        if (Globals.selectedUnit == null) {
          return;
        }

        Globals.hoveredUnit = GetHoveredUnit();

        if (Globals.hoveredUnit is IResource resource) {
          if (Globals.selectedUnit is Ant a) {
            resource.animationPlayer.Play("Target");
            a.Harvest(resource);
          }
        } else if (Globals.hoveredUnit is ConstructionSite c) {
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
            if (Globals.hoveredUnit is Enemy e) {
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
      _updateSelectedBuildingPosition(mouseMotionEvent);
    }

    if (Globals.gameMode == GameMode.Command) {
      var prevHoveredUnit = Globals.hoveredUnit;

      Globals.hoveredUnit = GetHoveredUnit();

      _maybeUpdateMouseCursor();

      if (prevHoveredUnit != Globals.hoveredUnit) {
        if (prevHoveredUnit != null) {
          prevHoveredUnit.OnHoverEnd();
        }

        if (Globals.hoveredUnit != null) {
          Globals.hoveredUnit.OnHoverStart();
        }
      }
    }
  }

  public bool IsBuildingInSafeLocation(Node2D building) {
    var buildingArea = building.GetNode<Area2D>("Area");
    var collisionShape = building.GetNode<CollisionShape2D>("Area/CollisionShape");
    var param = new PhysicsShapeQueryParameters2D();

    param.CollideWithAreas = true;
    param.CollideWithBodies = true;
    param.CollisionMask = Util.BUILDING_BITMASK;
    param.Transform = building.GlobalTransform;
    param.Exclude = new Godot.Collections.Array<RID>(new List<RID> { buildingArea.GetRid() });
    param.Shape = collisionShape.Shape;

    var collisions = GetWorld2d().DirectSpaceState.IntersectShape(param);

    foreach (var collision in collisions) {
      var n = (Node2D)collision["collider"];
    }

    return collisions.Count == 0;
  }

  private void _updateSelectedBuildingPosition(InputEventMouseMotion ev) {
    var pos = Util.MousePosition(GetTree());
    var roundedTo32 = new Vector2((int)(pos.x / 32) * 32, (int)(pos.y / 32) * 32);

    Globals.selectedBuilding.Position = roundedTo32;

    if (!IsBuildingInSafeLocation(Globals.selectedBuilding)) {
      Globals.selectedBuilding.Modulate = new Color(1, 0, 0, 0.5f);
    } else {
      Globals.selectedBuilding.Modulate = new Color(1, 1, 1, 0.5f);
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

    // Topmost first
    collisions.Sort((a, b) => {
      var result = a.IsGreaterThan(b);

      if (result) {
        return 1;
      } else {
        return -1;
      }
    });

    ISelectable best = null;

    // TODO: Maybe handle priority or something

    foreach (var collision in collisions) {
      if (collision.GetParent() is ISelectable c) {
        best = c;
      }
    }

    return best;
  }

  private GameMode _prevGameMode = GameMode.Build;
  public override void _Process(double delta) {
    if (Globals.gameMode != _prevGameMode) {
      _showCommandUi();

      _prevGameMode = Globals.gameMode;
    }

    _updateLabels();
  }
}
