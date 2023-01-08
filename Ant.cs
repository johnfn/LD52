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

public class BuildingState {
  public BuildingType SelectedBuildingType = BuildingType.None;
  public float BuildProgress = 0;
  public float BuildTime = 0;
  public Node2D ConstructionNode = null;
  public BuildBuildingStatus BuildingStatus = BuildBuildingStatus.None;
}

public partial class Ant : Sprite2D, IDamageable, ISelectable {

  public Dictionary<string, Action> actions { get; set; } = new Dictionary<string, Action>() {
    ["Town Hall"] = (delegate () {
      Actions.selectBuilding(BuildingType.TownHall);
    }),
    ["Resource Depot"] = (delegate () {
      Actions.selectBuilding(BuildingType.ResourceDepot);
    }),
  };

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

  private BuildingState _buildingState;


  public override void _Ready() {
    var stats = Util.UnitStats[UnitType.Ant];

    health = stats.health;
    maxHealth = stats.health;

    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UIRoot/UiPanel");
    _resourcePanel = GetNode<ResourcePanel>("/root/Root/Static/UIRoot/ResourcePanel");
  }

  public override void _Process(double delta) {
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
    if (_buildingState.BuildingStatus == BuildBuildingStatus.MovingToBuild) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _buildingState.BuildingStatus = BuildBuildingStatus.Building;
      }

      return;
    }

    if (_buildingState.BuildingStatus == BuildBuildingStatus.Building) {
      _buildingState.BuildProgress += (float)delta;

      if (_buildingState.BuildProgress >= _buildingState.BuildTime) {
        _status = UnitStatus.Idle;

        var buildingPosition = _buildingState.ConstructionNode.GlobalPosition;
        _buildingState.ConstructionNode.QueueFree();

        var buildingStats = Util.BuildingStats[_buildingState.SelectedBuildingType];
        var building = GD.Load<PackedScene>(buildingStats.resourcePath).Instantiate() as Node2D;

        GetTree().Root.AddChild(building);
        building.GlobalPosition = buildingPosition;

        _buildingState = new BuildingState();
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

      if (harvestState.harvestProgress >= harvestState.harvestTime) {
        harvestState.resource.amount -= 1;
        harvestState.harvestProgress = 0;

        if (harvestState.resource.amount <= 0) {
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

  public void Damage(int amount) {
    health -= amount;

    if (health <= 0) {
      QueueFree();
    }
  }

  public void Build(BuildingType buildingType, Vector2 buildingPosition) {
    var stats = Util.BuildingStats[buildingType];

    _status = UnitStatus.Building;

    _buildingState = new BuildingState {
      BuildingStatus = BuildBuildingStatus.MovingToBuild,
      BuildProgress = 0,
      BuildTime = stats.buildTime,
      ConstructionNode = GD.Load<PackedScene>("res://Scenes/construction.tscn").Instantiate<Node2D>(),
      SelectedBuildingType = buildingType
    };


    GetTree().Root.AddChild(_buildingState.ConstructionNode);

    _buildingState.ConstructionNode.GlobalPosition = buildingPosition;
    _path = Util.Pathfind(GetTree(), GlobalPosition, buildingPosition);
  }
}
