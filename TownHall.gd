extends Sprite2D

# "Idle" or "Building"
var status = "Idle"
var build_progress = 0
var build_time = 0

@onready
var shape = $Area

@onready
var ui_panel = $"/root/Root/Static/UiPanel"

var ant_scene = preload("res://scenes/ant.tscn")

var unit_name = "Town Hall"

func _ready():
  shape.connect("mouse_entered", func ():
    self.modulate.a = 0.5
  )

  shape.connect("mouse_exited", func ():
    self.modulate.a = 1
  )

  shape.connect("mouse_button_pressed", func (pos):
    ui_panel.Select(self)
  )

func _process(delta):
  if status == "Building":
    build_progress += delta

    if build_progress >= build_time:
      create_unit(unit_name)

      status = "Idle"
      build_progress = 0
      build_time = 0

func create_unit(unit_name: String):
  var unit = ant_scene.instantiate()

  unit.position = self.position + Vector2(0, 150)
  get_parent().add_child(unit)

func buy_unit(name: String): 
  if name == "Grasshopper":
    status = "Building"

    build_progress = 0
    build_time = 0.1
