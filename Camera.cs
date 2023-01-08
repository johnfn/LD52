using Godot;

public partial class Camera : Camera2D {
  public bool mouseMoveLeft = false;
  public bool mouseMoveRight = false;
  public bool mouseMoveUp = false;
  public bool mouseMoveDown = false;

  public Area2D moveLeftArea;
  public Area2D moveRightArea;
  public Area2D moveUpArea;
  public Area2D moveDownArea;

  public override void _Ready() {
    moveLeftArea = GetNode<Area2D>("MoveLeft");
    moveRightArea = GetNode<Area2D>("MoveRight");
    moveUpArea = GetNode<Area2D>("MoveUp");
    moveDownArea = GetNode<Area2D>("MoveDown");

    moveLeftArea.Connect("mouse_entered", Callable.From(() => {
      mouseMoveLeft = true;
    }));
    moveLeftArea.Connect("mouse_exited", Callable.From(() => {
      mouseMoveLeft = false;
    }));

    moveRightArea.Connect("mouse_entered", Callable.From(() => {
      mouseMoveRight = true;
    }));
    moveRightArea.Connect("mouse_exited", Callable.From(() => {
      mouseMoveRight = false;
    }));

    moveUpArea.Connect("mouse_entered", Callable.From(() => {
      mouseMoveUp = true;
    }));
    moveUpArea.Connect("mouse_exited", Callable.From(() => {
      mouseMoveUp = false;
    }));

    moveDownArea.Connect("mouse_entered", Callable.From(() => {
      mouseMoveDown = true;
    }));
    moveDownArea.Connect("mouse_exited", Callable.From(() => {
      mouseMoveDown = false;
    }));
  }

  public override void _Process(double delta) {
    if (mouseMoveLeft || Input.IsActionPressed("CamLeft")) {
      Position -= new Vector2(500, 0) * (float)delta;
    }

    if (mouseMoveRight || Input.IsActionPressed("CamRight")) {
      Position += new Vector2(500, 0) * (float)delta;
    }

    if (mouseMoveUp || Input.IsActionPressed("CamUp")) {
      Position -= new Vector2(0, 500) * (float)delta;
    }

    if (mouseMoveDown || Input.IsActionPressed("CamDown")) {
      Position += new Vector2(0, 500) * (float)delta;
    }
  }
}
