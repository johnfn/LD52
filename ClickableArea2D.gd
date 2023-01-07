extends Area2D

func _ready():
  # Add custom signal named mouse_button_pressed
  self.add_user_signal("mouse_button_pressed")

  connect("input_event", func (viewport, input_event, shape_idx):
    if (
      input_event is InputEventMouseButton and 
      input_event.is_pressed() and 
      input_event.get_button_index() == MOUSE_BUTTON_LEFT):

      self.emit_signal("mouse_button_pressed", input_event.get_position())
  )
