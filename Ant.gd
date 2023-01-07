extends Sprite2D

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
