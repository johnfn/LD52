using System.Collections.Generic;
using Godot;

public enum UnitStatus {
  Idle,
  Moving,
  Harvesting
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

public partial class Ant : Sprite2D, IUnit, ISelectable {

  // IHoverable
  public bool isHoverable { get; set; } = true;
  public int priority { get; set; } = 0;

  public void OnHoverStart() {
    Modulate = new Color(1, 1, 1, 0.5f);
  }

  public void OnHoverEnd() {
    Modulate = new Color(1, 1, 1, 1f);
  }


  // IUnit
  public string unitName { get; set; } = "Ant";
  public int health { get; set; } = 10;

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


  public override void _Ready() {
    _shape = GetNode<Area2D>("Area");
    _uiPanel = GetNode<UiPanel>("/root/Root/Static/UiPanel");
  }

  public override void _Process(double delta) {
    if (_status == UnitStatus.Moving) {
      var done = Util.WalkAlongPath(this, _path, _speed * (float)delta);

      if (done) {
        _status = UnitStatus.Idle;
      }
    } else if (_status == UnitStatus.Harvesting) {
      _processHarvest(delta);
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

          // harvestState.status = HarvestStatus.Returning;
          // harvestState.path = Util.Pathfind(GetTree(), GlobalPosition, _uiPanel.townHallGlobalPosition);

          // TODO: Return to town hall, or another building.

          InventoryItem = new InventoryItem {
            resourceType = harvestState.resource.resourceType
          };

          harvestState = null;
          _status = UnitStatus.Idle;
        }
      }
    }
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

    GD.Print("Harvesting");
  }
}
