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
  if status == UnitStatus.MOVING:
    var distance = move_destination - self.position
    var delta = speed * get_process_delta_time()

    if distance.length() < delta:
      self.position = move_destination
      status = UnitStatus.IDLE
    else:
      self.position += distance.normalized() * delta

func move(destination: Vector2):
  move_destination = destination

  status = UnitStatus.MOVING
