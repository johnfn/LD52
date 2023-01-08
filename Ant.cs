using System.Collections.Generic;
using System.Linq;
using System;
using Godot;

public enum UnitStatus {
  Idle,
  Moving,
  Attacking,
  Building,
  Harvesting
}

public enum BuildBuildingStatus {
  None,
  MovingToBuild,
  Building
}

public enum HarvestStatus {
  GoingToResource,
  Harvesting,
  Returning
}

public class HarvestState {
  public IResource resource { get; set; }
  public float harvestProgress { get; set; }
  public float harvestTime { get; set; }
  public HarvestStatus status { get; set; }
  public List<Vector2> path { get; set; }
}

public class InventoryItem {
  public ResourceType resourceType { get; set; }
}

public partial class Ant : Node2D, IDamageable, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {
    ["Town Hall"] = (delegate () {
      Actions.selectBuilding(BuildingType.TownHall);
    }),
    ["Resource Depot"] = (delegate () {
      Actions.selectBuilding(BuildingType.ResourceDepot);
    }),
  };

  public ConstructionSite ConstructionSite = null;

  public Node2D node { get => this; }

  public string name { get; set; } = "Ant";


  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }

  public string selectionText {
    get {
      return "This is an ANT!";
    }
  }

  // IUnit
  public string unitName { get; set; } = "Ant";
  public int health { get; set; }
  public int maxHealth { get; set; }

  private int _speed = 500;
  private UnitStatus _status = UnitStatus.Idle;

  // Moving state
  private List<Vector2> _path = new List<Vector2>();

  // Harvesting state
  private HarvestState harvestState = null;

  // Worker stuff
  public InventoryItem InventoryItem { get; private set; } = null;

  private Area2D _shape;

  private UiPanel _uiPanel;
  private ResourcePanel _resourcePanel;
  public BuildBuildingStatus BuildingStatus = BuildBuildingStatus.None;
  public ProgressBar ProgressBar => GetNode<ProgressBar>("ProgressBar");
  public SelectionCircle SelectionCircle => GetNode<SelectionCircle>("SelectionCircle");

  public ProgressBar healthBar => GetNode<ProgressBar>("HealthBar");

  public override void _Ready() {
    var stats = Util.UnitStats[UnitType.Ant];

    health = stats.health;
    maxHealth = stats.health;

    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");
    _resourcePanel = GetNode<ResourcePanel>("/root/Root/Static/UIRoot/ResourcePanel");

    SelectionCircle.Unit = this;
  }

  public override void _Process(double delta) {
    ProgressBar.Visible = harvestState != null && harvestState.status == HarvestStatus.Harvesting;

    if (_status == UnitStatus.Moving) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _status = UnitStatus.Idle;
      }
    } else if (_status == UnitStatus.Harvesting) {
      _processHarvest(delta);
    } else if (_status == UnitStatus.Building) {
      _processBuild(delta);
    }
  }

  private void _processBuild(double delta) {
    if (BuildingStatus == BuildBuildingStatus.MovingToBuild) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        BuildingStatus = BuildBuildingStatus.Building;
      }

      return;
    }

    if (BuildingStatus == BuildBuildingStatus.Building) {
      ConstructionSite.BuildingState.BuildProgress += (float)delta;

      if (ConstructionSite.BuildingState.BuildProgress >= ConstructionSite.BuildingState.BuildTime) {
        _status = UnitStatus.Idle;

        var buildingPosition = ConstructionSite.GlobalPosition;
        ConstructionSite.QueueFree();

        var buildingStats = Util.BuildingStats[ConstructionSite.BuildingState.SelectedBuildingType];
        var building = GD.Load<PackedScene>(buildingStats.resourcePath).Instantiate() as Node2D;

        GetTree().Root.AddChild(building);
        building.GlobalPosition = buildingPosition;

        ConstructionSite.BuildingState = new BuildingState();
      }
    }
  }

  private void _processHarvest(double delta) {
    if (harvestState.status == HarvestStatus.GoingToResource) {
      var done = Util.WalkAlongPath(this, harvestState.path, _speed * (float)delta);

      if (done) {
        harvestState.status = HarvestStatus.Harvesting;
        return;
      }
    }

    if (harvestState.status == HarvestStatus.Harvesting) {
      harvestState.harvestProgress += (float)delta;

      ProgressBar.SetProgress(harvestState.harvestProgress / harvestState.harvestTime);

      if (harvestState.harvestProgress >= harvestState.harvestTime) {
        harvestState.resource.amount -= 1;
        harvestState.harvestProgress = 0;

        // Harvest complete!

        InventoryItem = new InventoryItem {
          resourceType = harvestState.resource.resourceType
        };

        var resourceDropoff = FindNearestResourceDropoff();

        if (resourceDropoff != null) {
          harvestState.path = Util.Pathfind(GetTree(), GlobalPosition, resourceDropoff.GlobalPosition);
          harvestState.status = HarvestStatus.Returning;
        } else {
          // TODO: Show some sort of error; probably it was destroyed or sth

          harvestState = null;
          _status = UnitStatus.Idle;
        }
      }
    }

    if (harvestState.status == HarvestStatus.Returning) {
      var done = Util.WalkAlongPath(this, harvestState.path, _speed * (float)delta);

      if (done) {
        // Add to resources.

        if (InventoryItem.resourceType == ResourceType.Twig) {
          Globals.TwigCount += 5;
        } else if (InventoryItem.resourceType == ResourceType.Meat) {
          Globals.MeatCount += 5;
        }

        InventoryItem = null;

        harvestState.status = HarvestStatus.GoingToResource;
        harvestState.path = Util.Pathfind(GetTree(), GlobalPosition, harvestState.resource.resourceGlobalPosition);

        return;
      }
    }
  }

  public Node2D FindNearestResourceDropoff() {
    var results = GetTree().GetNodesInGroup(GroupNames.ResourceDropoff).ToList().OrderBy((a_) => {
      if (a_ is Node2D a) {
        return GlobalPosition.DistanceTo(a.GlobalPosition);
      }

      return 99999999999;
    });

    return results.FirstOrDefault() as Node2D;
  }

  public void Move(Vector2 destination) {
    _status = UnitStatus.Moving;
    _path = Util.Pathfind(GetTree(), GlobalPosition, destination);
  }

  public void Harvest(IResource resource) {
    _status = UnitStatus.Harvesting;

    harvestState = new HarvestState {
      resource = resource,
      harvestProgress = 0,
      harvestTime = resource.harvestTime,
      status = HarvestStatus.GoingToResource,
      path = Util.Pathfind(GetTree(), GlobalPosition, resource.resourceGlobalPosition)
    };
  }

  public void Build(BuildingType buildingType, Vector2 buildingPosition) {
    var stats = Util.BuildingStats[buildingType];

    _status = UnitStatus.Building;

    ConstructionSite = GD.Load<PackedScene>("res://Scenes/construction.tscn").Instantiate<ConstructionSite>();

    ConstructionSite.BuildingState = new BuildingState {
      BuildProgress = 0,
      BuildTime = stats.buildTime,
      SelectedBuildingType = buildingType
    };

    BuildingStatus = BuildBuildingStatus.MovingToBuild;

    GetTree().Root.AddChild(ConstructionSite);

    ConstructionSite.GlobalPosition = buildingPosition;
    _path = Util.Pathfind(GetTree(), GlobalPosition, buildingPosition);
  }

  public void ContinueToBuild(ConstructionSite c) {
    _status = UnitStatus.Building;
    ConstructionSite = c;
    BuildingStatus = BuildBuildingStatus.MovingToBuild;
    _path = Util.Pathfind(GetTree(), GlobalPosition, ConstructionSite.GlobalPosition);
  }
}
