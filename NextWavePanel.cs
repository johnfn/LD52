using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public enum MonsterType {
  NormalMonster,
}

public class Wave {
  public int TimeTillWave = 5;
  public Dictionary<MonsterType, int> Monsters = new Dictionary<MonsterType, int>();
  public string Tutorial;
}

public partial class NextWavePanel : Panel {
  public Label TimeLeftLabel => GetNode<Label>("Label");
  public float TimeTillNextWave;
  public float InitialTimeTillNextWave;
  public ColorRect Progress => GetNode<ColorRect>("Progress");
  public List<IDamageable> Monsters = new List<IDamageable>();
  public Panel NextLevelTutorial => GetNode<Panel>("NextLevelTutorial");

  public int CurrentWave = Util.DEBUG ? 0 : -1;


  public override void _Ready() {
    base._Ready();

    _showNextTutorialPrompt();

    NextLevelTutorial.GetNode<Button>("Button").Connect("pressed", Callable.From(() => {
      _advanceWave();
    }));
  }

  private void _showNextTutorialPrompt() {
    ++CurrentWave;
    NextLevelTutorial.Visible = true;
    var tutorialText = Util.Waves[CurrentWave].Tutorial;

    if (tutorialText != "" && tutorialText != null) {
      NextLevelTutorial.GetNode<Label>("TutorialText").Text = tutorialText;
    } else {
      _advanceWave();
    }
  }

  private void _advanceWave() {
    var wave = Util.Waves[CurrentWave];

    NextLevelTutorial.Visible = false;
    TimeTillNextWave = wave.TimeTillWave;
    InitialTimeTillNextWave = wave.TimeTillWave;
    Monsters.Clear();
  }

  private int _prevMonsterCount = 0;

  public override void _Process(double delta) {
    if (NextLevelTutorial.Visible) {
      return;
    }

    var aliveMonsters = Monsters.Where(m => IsInstanceValid(m.node)).Count();
    var monsterDiedThisTick = aliveMonsters != _prevMonsterCount;

    _prevMonsterCount = aliveMonsters;

    if (aliveMonsters == 0) {
      if (monsterDiedThisTick) {
        // wave just completed.
        _showNextTutorialPrompt();
      }

      Progress.Scale = new Vector2(
        1 - (TimeTillNextWave / InitialTimeTillNextWave),
        1
      );

      if (TimeTillNextWave < 0) {
        // Already started next wave
        return;
      }

      TimeTillNextWave -= (float)delta;

      if (TimeTillNextWave < 0) {
        _startNextWave();
        TimeLeftLabel.Text = "Next wave started!";
      } else {
        TimeLeftLabel.Text = $"Next wave in {TimeTillNextWave.ToString("0.0")}s";
      }
    }


  }

  private Vector2 _getPointInDarkness(
    Godot.Collections.Array<Node> allColliders
  ) {
    var spawnPosition = Vector2.Zero;

    for (var tries = 0; tries < 50; tries++) {
      var topLeft = Vector2.Zero;
      var bottomRight = Vector2.Zero;

      foreach (Node2D collider in allColliders) {
        topLeft.x = Math.Min(topLeft.x, collider.GlobalPosition.x);
        topLeft.y = Math.Min(topLeft.y, collider.GlobalPosition.y);

        bottomRight.x = Math.Max(bottomRight.x, collider.GlobalPosition.x);
        bottomRight.y = Math.Max(bottomRight.y, collider.GlobalPosition.y);
      }

      var candidatePosition = new Vector2(
        (float)GD.RandRange((double)topLeft.x - 1000, (double)bottomRight.x + 1000),
        (float)GD.RandRange((double)topLeft.y - 1000, (double)bottomRight.y + 1000)
      );

      spawnPosition = Util.FindSafeSpaceNear(
        GetTree(),
        candidatePosition,
        true
      );

      // Ensure we aren't too close to any building, try to spawn in the dark

      var tooClose = false;

      foreach (Node2D collider in allColliders) {
        if (collider.GlobalPosition.DistanceTo(spawnPosition) < 800) {
          tooClose = true;
          break;
        }
      }

      if (!tooClose) {
        break;
      }
    }

    return spawnPosition;
  }

  private void _startNextWave() {
    var allColliders = GetTree().GetNodesInGroup(GroupNames.Collider);
    var wave = Util.Waves[CurrentWave];

    for (var i = 0; i < wave.Monsters[MonsterType.NormalMonster]; i++) {
      var monster = GD.Load<PackedScene>("res://scenes/enemy.tscn").Instantiate<Enemy>();

      Util.Add(GetTree(), monster);

      monster.GlobalPosition = _getPointInDarkness(allColliders);

      Monsters.Add(monster);
    }
  }
}
