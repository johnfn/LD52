extends Panel

# "Build" or "Command"
var game_mode = "Command"
var selected_unit: Sprite2D = null
var selected_building: Sprite2D

@onready var town_hall_panel: Panel = $TownHallPanel

@onready var build_panel: Panel = $BuildPanel
@onready var progress_label: Label = $BuildPanel/ProgressLabel

@onready var unit_panel: Panel = $UnitPanel
@onready var stats_label: Label = $UnitPanel/StatsLabel
@onready var selected_unit_label: Label = $UnitPanel/SelectedUnitLabel

@onready var builder_panel: Panel = $BuilderPanel
@onready var town_hall_button: Button = $BuilderPanel/TownHallButton

@onready var town_hall_buy_grasshopper_button: Button = $TownHallPanel/BuyGrasshopper

# Called when the node enters the scene tree for the first time.
func _ready():
  show_command_ui()

  town_hall_buy_grasshopper_button.connect("pressed", func():
    if selected_unit != null and selected_unit.unit_name == "Town Hall":
      selected_unit.buy_unit("Grasshopper")
  ) 

  town_hall_button.connect("pressed", func():
    selected_building = preload("res://scenes/town_hall.tscn").instantiate()

    selected_building.modulate = Color(1, 1, 1, 0.5)

    get_node("/root/Root").add_child(selected_building)
    game_mode = "Build"
  )

func _process(_delta):
  if game_mode == "Move":
    show_command_ui()


# TODO: All mouse input should be handled through here; things are going to get
# too confusing otherwise with every node trying to do its own thing!

func _input(event):
  if event is InputEventMouseButton:
    if event.button_index == MOUSE_BUTTON_LEFT:
      if game_mode == "Build":
        if selected_building != null:
          selected_building.modulate = Color(1, 1, 1, 1)
          selected_building = null

          game_mode = "Command"
          return

  if event is InputEventMouseMotion:
    if game_mode == "Build":
      var mouse = get_viewport().get_mouse_position()
      var rounded = Vector2(round(mouse.x / 32) * 32, round(mouse.y / 32) * 32)

      selected_building.position = rounded

func show_command_ui():
  var town_hall_panel_visible = false
  var build_panel_visible = false
  var unit_panel_visible = false
  var builder_panel_visible = false

  if selected_unit == null:
    selected_unit_label.text = "Selected Unit: None"
  else:
    selected_unit_label.text = "Selected Unit: " + selected_unit.unit_name

    if selected_unit.unit_name == "Town Hall":
      if selected_unit.status == "Idle":
        town_hall_panel_visible = true
      else:
        build_panel_visible = true
        progress_label.text = "Progress: " + str(round(selected_unit.build_progress * 100 / selected_unit.build_time)) + "%"

    if selected_unit.unit_name == "Ant":
      builder_panel_visible = true
  
  town_hall_panel.visible = town_hall_panel_visible
  build_panel.visible = build_panel_visible
  unit_panel.visible = unit_panel_visible
  builder_panel.visible = builder_panel_visible

func select(unit: Sprite2D):
  selected_unit = unit

  show_command_ui()

func move(position: Vector2):
  if selected_unit != null and selected_unit.unit_name == "Ant":
    selected_unit.move(position)
