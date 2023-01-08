using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Util {
    public static int CELL_SIZE = 32;
    public static bool DEBUG = true;
    public static bool DEBUG_PATH = false;

    public static Vector2 MousePosition(SceneTree tree) {
        return tree.Root.GetNode<Node2D>("Root").GetLocalMousePosition();
    }

    public static Vector2 RoundToCell(Vector2 vector) {
        return new Vector2(
            (int)(vector.x / CELL_SIZE) * CELL_SIZE,
            (int)(vector.y / CELL_SIZE) * CELL_SIZE
        );
    }

    // If end is already on top of a collision, find a safe end point which is not
    // colliding with anything instead.
    private static Vector2 _getSafeEnd(
      SceneTree tree,
      Vector2 initialEnd
    ) {
        var f = new PhysicsPointQueryParameters2D();

        f.Position = initialEnd;
        f.Exclude = new Godot.Collections.Array<RID>();
        f.CollideWithAreas = true;
        f.CollideWithBodies = true;
        f.CollisionMask = 2;

        var collisions = tree.Root.World2d.DirectSpaceState.IntersectPoint(f);

        if (collisions.Count == 0) {
            GD.Print("Nothing to worry about!");

            return initialEnd;
        }

        // spiral around initialEnd until we find a safe point

        var end = initialEnd;

        for (var i = 0; i < 20; i++) {
            foreach (var direction in new List<Vector2>() {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
      }) {
                var nextEnd = end + i * direction * CELL_SIZE;

                f.Position = nextEnd;

                if (tree.Root.World2d.DirectSpaceState.IntersectPoint(f).Count == 0) {
                    return nextEnd;
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
            var dbgs = tree.GetNodesInGroup("Debug");

            foreach (Node2D dbg in dbgs) {
                dbg.QueueFree();
            }
        }

        var allColliders = tree.GetNodesInGroup("collider");

        var start = RoundToCell(initialStart);
        var end = RoundToCell(initialEnd);

        end = _getSafeEnd(tree, end);

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
        int lastId = 0;

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

                pointIds[point] = lastId++;
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

        return astar.GetPointPath(pointIds[start], pointIds[end]).ToList();
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

        node.GlobalPosition += direction * speed;

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
          health = 100,
          twigCost = 0,
          meatCost = 0,
          resourcePath = "res://scenes/town_hall.tscn",
          description = "The town hall is the center of your town. It is where you can build new buildings and recruit new villagers."
        }
      },

      {
        BuildingType.ResourceDepot,
        new BuildingStats() {
          buildTime = 5f,
          health = 100,
          twigCost = 20,
          meatCost = 0,
          resourcePath = "res://scenes/resource_depot.tscn",
          description = "Your ants can drop off resources at a resource depot."
        }
      },
    };
        }
    }
}
