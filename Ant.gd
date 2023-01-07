extends Sprite2D

enum UnitStatus {
  IDLE,
  MOVING
}

var speed = 500
var status: UnitStatus = UnitStatus.IDLE
var move_destination: Vector2 = Vector2.ZERO # godot does not allow null for Vector2

@onready
var shape = $Area

@onready
var ui_panel = $"/root/Root/UiPanel"

var unit_name = "Ant"

func _ready():
  shape.connect("mouse_entered", func ():
    self.modulate.a = 0.5
  )

  shape.connect("mouse_exited", func ():
    self.modulate.a = 1
  )

  shape.connect("mouse_button_pressed", func (pos):
    ui_panel.select(self)
  )

func _process(_delta: float):
  pass

func move(destination: Vector2):
  move_destination = destination
  status = UnitStatus.MOVING
  queue_redraw()

  print("Yeet")

func round_to_32(vector: Vector2):
  return Vector2(
    round(vector.x / 32) * 32,
    round(vector.y / 32) * 32
  )

func _draw():
  if status != UnitStatus.MOVING:
    return

  var cell_size = 32

  var all_colliders = get_tree().get_nodes_in_group("collider")

  var start = round_to_32(global_position)
  var end = round_to_32(move_destination)

  var top_left = Vector2(min(start.x, end.x), min(start.y, end.y))
  var bottom_right = Vector2(max(start.x, end.x), max(start.y, end.y))

  for collider in all_colliders:
    top_left.x = min(top_left.x, collider.global_position.x)
    top_left.y = min(top_left.y, collider.global_position.y)

    bottom_right.x = max(bottom_right.x, collider.global_position.x)
    bottom_right.y = max(bottom_right.y, collider.global_position.y)

  top_left -= Vector2(320, 320)
  bottom_right += Vector2(320, 320)

  top_left = round_to_32(top_left)
  bottom_right = round_to_32(bottom_right)

  var astar = AStar2D.new()
  var point_ids = {}
  var last_id = 0

  var f: PhysicsPointQueryParameters2D = PhysicsPointQueryParameters2D.new()
  f.exclude = []
  f.collide_with_areas = true
  f.collide_with_bodies = true
  f.collision_mask = 0b00000000000000000000000000000010

  for x in range(top_left.x, bottom_right.x, cell_size):
    for y in range(top_left.y, bottom_right.y, cell_size):
      last_id += 1

      var point = Vector2(x, y)
      f.position = point

      var collisions = get_world_2d().direct_space_state.intersect_point(f)

      if collisions.size() > 0:
        continue

      point_ids[point] = last_id
      astar.add_point(last_id, Vector2(x, y))

      var neighbors = [
        Vector2(x + cell_size, y),
        Vector2(x - cell_size, y),
        Vector2(x, y + cell_size),
        Vector2(x, y - cell_size)
      ]

      for neighbor in neighbors:
        if point_ids.has(neighbor):
          astar.connect_points(point_ids[point], point_ids[neighbor])

          draw_line(
            point - global_position, 
            neighbor - global_position, 
            Color(0, 1, 0, 0.8)
          )

  if !point_ids.has(start) or !point_ids.has(end):
    print("No path found")
    return

  var path = astar.get_point_path(
    point_ids[start], 
    point_ids[end]
  )

  for point in path:
    draw_rect(Rect2(point - global_position, Vector2(32, 32)), Color(1, 0, 0, 0.8), false)

  # var stack = []
  # var prev_cell = {}

  # # find the shortest path from pos to dest using A* pathfinding
  # # https://en.wikipedia.org/wiki/A*_search_algorithm

  # # add the starting position to the stack
  # stack.append(pos)

  # var tries = 0
  # while stack.size() > 0:
  #   tries += 1

  #   stack.sort_custom(func (a, b):
  #     return a.distance_squared_to(dest) - b.distance_squared_to(dest)
  #   )

  #   if tries > 1000:
  #     print("I give up")
  #     for cell in prev_cell:
  #       draw_rect(Rect2(cell - global_position, Vector2(32, 32)), Color(0, 1, 0, 0.8), false)

  #     return

  #   var current = stack.pop_front()

  #   # if the current position is the destination, we're done
  #   if current == dest:
  #     break

  #   # get the neighbors of the current position
  #   var neighbors = [
  #     Vector2(current.x + cell_size, current.y),
  #     Vector2(current.x - cell_size, current.y),
  #     Vector2(current.x, current.y + cell_size),
  #     Vector2(current.x, current.y - cell_size)
  #   ]

  #   for neighbor in neighbors:
  #     if prev_cell.has(neighbor):
  #       continue

  #     prev_cell[neighbor] = current

  #     var f: PhysicsPointQueryParameters2D = PhysicsPointQueryParameters2D.new()
  #     f.position = neighbor
  #     f.exclude = []
  #     f.collide_with_areas = true
  #     f.collide_with_bodies = true
  #     f.collision_mask = 0b00000000000000000000000000000010

  #     # Get all colliding areas at the neighbor position
  #     var results = get_world_2d().direct_space_state.intersect_point(f)

  #     if results.size() > 0:
  #       continue

  #     stack.append(neighbor)

  # print("TRIES", tries)

  # for cell in prev_cell:
  #   draw_rect(Rect2(cell - global_position, Vector2(32, 32)), Color(0, 1, 0, 0.8), false)

  # var path = []
  # if not prev_cell.has(dest):
  #   print("No path found")
  #   return
  
  # var current = dest
  # while current != pos:
  #   path.append(current)
  #   current = prev_cell[current]

  # print("I did it")
  # print(
  #   path
  # )

  # for cell in path:
  #   draw_rect(Rect2(cell - global_position, Vector2(32, 32)), Color(1, 0, 0, 0.8), false)
