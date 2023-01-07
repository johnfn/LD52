using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Util {
  public static int CELL_SIZE = 32;

  public static Vector2 MousePosition(SceneTree tree) {
    return tree.Root.GetNode<Node2D>("Root").GetLocalMousePosition();
  }

  public static Vector2 RoundToCell(Vector2 vector) {
    return new Vector2(
        (int)(vector.x / CELL_SIZE) * CELL_SIZE,
        (int)(vector.y / CELL_SIZE) * CELL_SIZE
    );
  }

  public static List<Vector2> Pathfind(
    SceneTree tree,
    Vector2 initialStart,
    Vector2 initialEnd
  ) {
    var allColliders = tree.GetNodesInGroup("collider");

    var start = RoundToCell(initialStart);
    var end = RoundToCell(initialEnd);

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

      return new List<Vector2>();
    }

    return astar.GetPointPath(pointIds[start], pointIds[end]).ToList();
  }
}