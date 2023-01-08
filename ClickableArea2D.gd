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
  pass
