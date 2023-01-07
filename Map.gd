extends Node2D

@onready
var ui_panel = $"/root/Root/Static/UIRoot/UiPanel"

func _unhandled_input(event):
  pass
  # if (event is InputEventMouseButton and 
  #   event.button_index == MOUSE_BUTTON_RIGHT):

  #   if event.is_pressed():
  #     ui_panel.move(event.position)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
  pass
