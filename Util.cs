using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Util {
  public static int CELL_SIZE = 32;
  public static bool DEBUG = true;
  public static bool DEBUG_PATH = false;

  public static uint BUILDING_BITMASK = 0b10;
  public static uint UNIT_BITMASK = 0b100;

  public static Vector2 MousePosition(SceneTree tree) {
    return tree.Root.GetNode<Node2D>("Root").GetLocalMousePosition();
  }

  public static Vector2 RoundToCell(Vector2 vector) {
    return new Vector2(
        (int)(vector.x / CELL_SIZE) * CELL_SIZE,
        (int)(vector.y / CELL_SIZE) * CELL_SIZE
    );
  }

  // If end is already on top of a collision, find a safe point near it, which
  // is not colliding with anything instead.
  public static Vector2 FindSafeSpaceNear(
    SceneTree tree,
    Vector2 point,
    bool avoidUnits = false
  ) {
    var f = new PhysicsPointQueryParameters2D();

    f.Position = point;
    f.Exclude = new Godot.Collections.Array<RID>();
    f.CollideWithAreas = true;
    f.CollideWithBodies = true;
    f.CollisionMask = avoidUnits ? BUILDING_BITMASK | UNIT_BITMASK : BUILDING_BITMASK;

    var collisions = tree.Root.World2d.DirectSpaceState.IntersectPoint(f);

    if (collisions.Count == 0) {
      GD.Print("Early");
      return point;
    }

    // spiral around point until we find a new point which does not collide with anything

    var end = point;

    for (var i = 0; i < 20; i++) {
      foreach (var direction in new List<Vector2>() {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
      }) {
        var candidatePoint = end + i * direction * CELL_SIZE;

        var pointsToCheck = new List<Vector2>() {
          candidatePoint,
          candidatePoint + new Vector2(-100, -100),
          candidatePoint + new Vector2(100, -100),
          candidatePoint + new Vector2(-100, 100),
          candidatePoint + new Vector2(100, 100),
        };

        var safe = true;

        foreach (var p in pointsToCheck) {
          f.Position = p;

          if (tree.Root.World2d.DirectSpaceState.IntersectPoint(f).Count != 0) {
            safe = false;
            break;
          }
        }

        if (safe) {
          return candidatePoint;
        }
      }
    }

    GD.Print("Could not find a safe end point");
    return Vector2.Inf;
  }

  private static void DbgPoint(Vector2 p, SceneTree tree, Color color) {
    var debug = GD.Load<PackedScene>("res://Debug.tscn").Instantiate<Node2D>();
    tree.Root.AddChild(debug);
    debug.GlobalPosition = p;
    debug.Modulate = color;
  }

  public static List<Vector2> Pathfind(
    SceneTree tree,
    Vector2 initialStart,
    Vector2 initialEnd
  ) {
    if (DEBUG_PATH) {
      var dbgs = tree.GetNodesInGroup(GroupNames.Debug);

      foreach (Node2D dbg in dbgs) {
        dbg.QueueFree();
      }
    }

    var allColliders = tree.GetNodesInGroup(GroupNames.Collider);

    var start = RoundToCell(initialStart);
    var end = RoundToCell(initialEnd);

    end = FindSafeSpaceNear(tree, end);

    if (end == Vector2.Inf) {
      GD.Print("Could not find a path");

      return new List<Vector2>() { };
    }

    if (DEBUG_PATH) {
      DbgPoint(start, tree, new Color(1, 1, 0, 1f));
    }

    if (DEBUG_PATH) {
      DbgPoint(end, tree, new Color(1, 1, 0, 1f));
    }

    var topLeft = new Vector2(
        Math.Min(start.x, end.x),
        Math.Min(start.y, end.y)
    );
    var bottomRight = new Vector2(
        Math.Max(start.x, end.x),
        Math.Max(start.y, end.y)
    );

    foreach (Node2D collider in allColliders) {
      topLeft.x = Math.Min(topLeft.x, collider.GlobalPosition.x);
      topLeft.y = Math.Min(topLeft.y, collider.GlobalPosition.y);

      bottomRight.x = Math.Max(bottomRight.x, collider.GlobalPosition.x);
      bottomRight.y = Math.Max(bottomRight.y, collider.GlobalPosition.y);
    }

    topLeft -= new Vector2(320, 320);
    bottomRight += new Vector2(320, 320);

    topLeft = RoundToCell(topLeft);
    bottomRight = RoundToCell(bottomRight);

    var astar = new AStar2D();
    var pointIds = new Dictionary<Vector2, int>();
    int lastId = -1;

    var f = new PhysicsPointQueryParameters2D();
    f.Exclude = new Godot.Collections.Array<RID>();
    f.CollideWithAreas = true;
    f.CollideWithBodies = true;
    f.CollisionMask = 2;

    for (int x = (int)topLeft.x; x < bottomRight.x; x += CELL_SIZE) {
      for (int y = (int)topLeft.y; y < bottomRight.y; y += CELL_SIZE) {
        var point = new Vector2(x, y);
        f.Position = point;

        var collisions = tree.Root.World2d.DirectSpaceState.IntersectPoint(f);

        if (collisions.Count > 0) {
          continue;
        }

        if (DEBUG_PATH) {
          DbgPoint(point, tree, new Color(0, 1, 0, 0.5f));
        }

        pointIds[point] = ++lastId;
        astar.AddPoint(lastId, point);

        var neighbors = new Vector2[] {
          new Vector2(x + CELL_SIZE, y),
          new Vector2(x - CELL_SIZE, y),
          new Vector2(x, y + CELL_SIZE),
          new Vector2(x, y - CELL_SIZE),
          new Vector2(x + CELL_SIZE, y + CELL_SIZE),
          new Vector2(x - CELL_SIZE, y - CELL_SIZE),
          new Vector2(x + CELL_SIZE, y - CELL_SIZE),
          new Vector2(x - CELL_SIZE, y + CELL_SIZE),
        };

        foreach (Vector2 neighbor in neighbors) {
          if (pointIds.ContainsKey(neighbor)) {
            astar.ConnectPoints(pointIds[point], pointIds[neighbor]);
          }
        }
      }
    }

    if (!pointIds.ContainsKey(start) || !pointIds.ContainsKey(end)) {
      GD.Print("No path found");

      GD.Print("start: " + start);
      GD.Print("end: " + end);

      return new List<Vector2>();
    }

    var astarList = astar.GetPointPath(pointIds[start], pointIds[end]).ToList();

    if (DEBUG_PATH) {
      foreach (Vector2 point in astarList) {
        DbgPoint(point, tree, new Color(1, 1, 1, 1f));
      }
    }

    return astarList;
  }

  /** Returns true when done */
  public static bool WalkAlongPath(
    Node2D node,
    List<Vector2> path,
    float speed
  ) {
    if (path.Count == 0) {
      return true;
    }

    var target = path[0];
    var direction = (target - node.GlobalPosition).Normalized();

    if (node.GlobalPosition.DistanceTo(target) < speed) {
      node.GlobalPosition = target;
    } else {
      node.GlobalPosition += direction * speed;
    }

    if (node.GlobalPosition.DistanceTo(target) < 1) {
      path.RemoveAt(0);
    }

    return path.Count == 0;
  }

  public static Dictionary<BuildingType, BuildingStats> BuildingStats {
    get {
      return new Dictionary<BuildingType, BuildingStats>() {
      {
        BuildingType.TownHall,
        new BuildingStats() {
          buildTime = 5f,
          health = 10,
          twigCost = 100,
          meatCost = 0,
          name = "Town Hall",
          resourcePath = "res://scenes/buildings/town_hall.tscn",
          description = "The town hall is the center of your town. It is where you can build new buildings and recruit new villagers."
        }
      },

      {
        BuildingType.ResourceDepot,
        new BuildingStats() {
          buildTime = 5f,
          health = 10,
          twigCost = 15,
          meatCost = 0,
          name = "Resource Depot",
          resourcePath = "res://scenes/buildings/resource_depot.tscn",
          description = "Your ants can drop off resources at a resource depot."
        }
      },

      {
        BuildingType.Barracks,
        new BuildingStats() {
          buildTime = 5f,
          health = 10,
          twigCost = 25,
          meatCost = 0,
          resourcePath = "res://scenes/buildings/bug_barracks.tscn",
          description = "Train bug warriors to fight bug battles!"
        }
      },

      {
        BuildingType.GuardTower,
        new BuildingStats() {
          buildTime = 1f,
          health = 10,
          twigCost = 20,
          meatCost = 0,
          name = "Guard Tower",
          resourcePath = "res://scenes/buildings/guard_tower.tscn",
          description = "A guard tower will fire at enemies automatically!"
        }
      },

      {
        BuildingType.UpgradeFacility,
        new BuildingStats() {
          buildTime = 1f,
          health = 10,
          twigCost = 35,
          meatCost = 0,
          name = "Upgrade Facility",
          resourcePath = "res://scenes/buildings/guard_tower.tscn",
          description = "Here you can bug-research new bug-technologies to make bugs bug-stronger! Bug."
        }
      },

      {
        BuildingType.House,
        new BuildingStats() {
          buildTime = 1f,
          health = 10,
          twigCost = 20,
          meatCost = 0,
          name = "House",
          resourcePath = "res://scenes/buildings/supply.tscn",
          description = "Provides 4 housing for your bugs."
        }
      },
    };
    }
  }

  public static Dictionary<UnitType, UnitStats> UnitStats {
    get {
      return new Dictionary<UnitType, UnitStats>() {
        [UnitType.Ant] = new UnitStats() {
          buildTime = 4f,
          name = "Ant",
          speed = 300,
          health = 10,
          twigCost = 0,
          meatCost = 5,
          attackCooldown = 100,
          ranged = false,
          damage = 1,
          resourcePath = "res://scenes/units/ant.tscn",
          description = "Ants harvest resources and build buildings."
        },

        [UnitType.Beetle] =
          new UnitStats() {
            name = "Beetle",
            buildTime = 8f,
            speed = 300,
            health = 10,
            twigCost = 0,
            meatCost = 10,
            ranged = false,
            attackCooldown = 100,
            damage = 3,
            resourcePath = "res://scenes/units/beetle.tscn",
            description = "Beetles are strong, resilient fighters!"
          },

        [UnitType.Scout] =
          new UnitStats() {
            name = "Scout",
            speed = 600,
            buildTime = 6f,
            health = 10,
            twigCost = 20,
            meatCost = 0,
            attackCooldown = 200,
            ranged = false,
            damage = 2,
            resourcePath = "res://scenes/units/scout.tscn",
            description = "Scouts are not strong fighters, but they make it up with speed and agility! Which is a synonym for speed."
          },

        [UnitType.Spit] =
          new UnitStats() {
            name = "Spit Bug",
            buildTime = 10f,
            speed = 400,
            health = 10,
            twigCost = 20,
            meatCost = 20,
            ranged = true,
            attackCooldown = 200,
            damage = 3,
            resourcePath = "res://scenes/units/spitbug.tscn",
            description = "Spit bugs are your long range militia!"
          }
      };
    }
  }

  public static List<Wave> Waves = new List<Wave> {
    new Wave {
      TimeTillWave = 15,
      Monsters = new Dictionary<MonsterType, int> {
        [MonsterType.NormalMonster] = 1
      },
      Tutorial = "It was just an average day in BugTopia, when suddenly, a bunch of evil bugs started to attack!\n\n\nFend them off by clicking on your beetle, then right clicking the bad bug to attack it.",
    },

    new Wave {
      TimeTillWave = 30,
      Monsters = new Dictionary<MonsterType, int> {
        [MonsterType.NormalMonster] = 1
      },
      Tutorial = "Well done! But there are more bugs coming, and a single beetle might not be enough.\n\nClick on an ant, and then order it to build a Barracks.",
    },

    new Wave {
      TimeTillWave = 40,
      Monsters = new Dictionary<MonsterType, int> {
        [MonsterType.NormalMonster] = 3
      },
      Tutorial = "Great! But your resources (see the top left) are dwindling. We'll need matchsticks and gummies to train a bug army - order your ants to harvest them by right clicking on them.",
    },

    new Wave {
      TimeTillWave = 20,
      Monsters = new Dictionary<MonsterType, int> {
        [MonsterType.NormalMonster] = 3
      },
      Tutorial = "Don't forget, you can move the camera with WASD, and zoom in and out with scroll wheel.",
    },
  };

  public static void FlashNodeWhite(Node2D node) {
    node.GetNode<Node2D>("Graphics").Modulate = new Color(3f, 3f, 3f, 1f);

    var tween = node.GetTree().CreateTween();
    tween.TweenProperty(
        node.GetNode<Node2D>("Graphics"),
        "modulate",
        new Color(1, 1, 1, 1),
        0.2f
    );

    tween.SetEase(Tween.EaseType.Out);
  }

  public static void Add(
    SceneTree tree,
    Node2D thing
  ) {
    tree.Root.GetNode<Node2D>("Root").AddChild(thing);
  }
}
