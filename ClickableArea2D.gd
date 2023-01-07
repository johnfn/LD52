extends Area2D

var is_mouse_inside = false

func _ready():
  # Add custom signal named mouse_button_pressed
  self.add_user_signal("mouse_button_pressed")

  connect("mouse_entered", func():
    is_mouse_inside = true
  )

  connect("mouse_exited", func():
    is_mouse_inside = false
  )

func _input(event):
  if event is InputEventMouseButton and event.is_pressed() and event.get_button_index() == MOUSE_BUTTON_LEFT:
    if is_mouse_inside:
      get_tree().get_root().set_input_as_handled()

      self.emit_signal("mouse_button_pressed", event.get_position())