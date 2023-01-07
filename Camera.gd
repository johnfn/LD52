extends Camera2D

var mouse_move_left = false
var mouse_move_right = false
var mouse_move_up = false
var mouse_move_down = false

@onready var move_left = $MoveLeft
@onready var move_right = $MoveRight
@onready var move_up = $MoveUp
@onready var move_down = $MoveDown

# Called when the node enters the scene tree for the first time.
func _ready():
  move_left.connect("mouse_entered", func () : mouse_move_left = true)
  move_left.connect("mouse_exited", func () : mouse_move_left = false)

  move_right.connect("mouse_entered", func () : mouse_move_right = true)
  move_right.connect("mouse_exited", func () : mouse_move_right = false)

  move_up.connect("mouse_entered", func () : mouse_move_up = true)
  move_up.connect("mouse_exited", func () : mouse_move_up = false)

  move_down.connect("mouse_entered", func () : mouse_move_down = true)
  move_down.connect("mouse_exited", func () : mouse_move_down = false)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
  if mouse_move_left:
    position.x -= 500 * delta
  if mouse_move_right:
    position.x += 500 * delta
  if mouse_move_up:
    position.y -= 500 * delta
  if mouse_move_down:
    position.y += 500 * delta
