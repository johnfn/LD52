extends Node2D

@onready
var ui_panel = $"/root/Root/UiPanel"

func _unhandled_input(event):
  if event is InputEventMouseButton:
    if event.is_pressed():
      var position = event.position

      ui_panel.move(position)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
  pass
