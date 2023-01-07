extends Node2D

func _unhandled_input(event):
  if event is InputEventMouseButton:
    if event.is_pressed():
      print("OK (Unhandled)")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
  pass
