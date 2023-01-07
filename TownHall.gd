extends Sprite2D

# "Idle" or "Building"
var status = "Idle"
var build_progress = 0
var build_time = 0

@onready
var shape = $Area

@onready
var ui_panel = $"/root/Root/UiPanel"

var unit_name = "Town Hall"

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

func _process(delta):
  if status == "Building":
    build_progress += delta

    if build_progress >= build_time:
      status = "Idle"
      build_progress = 0
      build_time = 0

func buy_unit(name: String): 
  status = "Building"

  build_progress = 0
  build_time = 3
