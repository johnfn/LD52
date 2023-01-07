extends Panel

var selected_unit: Sprite2D = null

@onready var town_hall_panel: Panel = $TownHallPanel
@onready var build_panel: Panel = $BuildPanel
@onready var progress_label: Label = $BuildPanel/ProgressLabel
@onready var unit_panel: Panel = $UnitPanel

@onready var selected_unit_label: Label = $UnitPanel/SelectedUnitLabel
@onready var stats_label: Label = $UnitPanel/StatsLabel
@onready var town_hall_buy_grasshopper_button: Button = $TownHallPanel/BuyGrasshopper

# Called when the node enters the scene tree for the first time.
func _ready():
  update_ui()

  town_hall_buy_grasshopper_button.connect("pressed", func():
    if selected_unit != null and selected_unit.unit_name == "Town Hall":
      selected_unit.buy_unit("Grasshopper")
  ) 

func _process(_delta):
  update_ui()

func update_ui():
  var town_hall_panel_visible = false
  var build_panel_visible = false
  var unit_panel_visible = false

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
      unit_panel_visible = true
      stats_label.text = "Health: " + str(selected_unit.health)
  
  town_hall_panel.visible = town_hall_panel_visible
  build_panel.visible = build_panel_visible
  unit_panel.visible = unit_panel_visible

func select(unit: Sprite2D):
  selected_unit = unit

  update_ui()

func move(position: Vector2):
  if selected_unit != null and selected_unit.unit_name == "Ant":
    selected_unit.move(position)