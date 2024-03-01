using Godot;

public partial class InGameTimer : Node2D {
	[Signal]
	public delegate void TimeUpEventHandler(int dayCounter);

	[Export]
	public float minTime = 0;

	[Export]
	public float maxTime = 10;

	[Export]
	public float gameTime = 0;

	[Export]
	public bool isRunning = false;

	private int dayCounter = 0;

	private ProgressBar progressBarOfTime;

	public float GameTime {
		get {
			return gameTime;
		}
		set {
			gameTime = value;
			progressBarOfTime.Value = gameTime;
			if (gameTime >= maxTime) {
				gameTime = minTime;
				dayCounter++;
				EmitSignal(nameof(this.TimeUp), dayCounter);
			}
		}
	}

	public void Start() {
		isRunning = true;
	}

	public void Stop() {
		isRunning = false;
	}

	public void timeControl() {
		if (isRunning) {
			Stop();
		} else {
			Start();
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventKey eventKey) {
			if (eventKey.IsActionPressed("keyboard_time_control")) {
				timeControl();
			}
		}
	}
	
	public override void _Ready() {
		isRunning = true;
		progressBarOfTime = GetTree().CurrentScene.GetNode<ProgressBar>("UI_Stuff/UI_Layer/DayBar");
	}

    public override void _PhysicsProcess(double delta) {
        if (isRunning) {
			GameTime += (float)delta;
		}
    }
}
