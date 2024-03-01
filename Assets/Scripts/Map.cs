using System.Runtime.CompilerServices;
using Godot;

public partial class Map : TileMap {
	[Export]
	private int tileSize = 32;
	[Export]
	private int mapWidth= 100;
	[Export]
	private int mapHeight = 100;

	private int currentDay = 0;

	private InGameTimer _inGameTimer;

	private RichTextLabel dayCounterElementText;

	private static readonly Vector2I[] tileAtlasCoords = {
		new Vector2I(0, 0), // Grass
		new Vector2I(1, 0), // Hill
		new Vector2I(2, 0), // Mountain
		new Vector2I(0, 1), // Path
		new Vector2I(1, 1), // Water
		new Vector2I(0, 2), // Bridge
	};

	private void GenerateMap(FastNoiseLite noise) {
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				var tile = noise.GetNoise2D(x, y) * 10;
				Vector2I tileCoords = tileAtlasCoords[0];
				if (tile < -3.15) {
					tileCoords = tileAtlasCoords[4]; // Water
				} else if (tile < 1.75 && tile > -3.15) {
					tileCoords = tileAtlasCoords[0]; // Grass
				} else if (tile < 4.25 && tile > 1.75) {
					tileCoords = tileAtlasCoords[1]; // Hill
				} else if (tile < 6 && tile > 4.25) {
					tileCoords = tileAtlasCoords[2]; // Mountain
				} else if (tile > 6) {
					tileCoords = tileAtlasCoords[2]; // Mountain
				}
				SetCell(0, new Vector2I(x, y), 0, tileCoords);
			}
		}
	}

	private void ChangeUIElementText() {
		dayCounterElementText.Text = "Day: " + currentDay;
	}

	private void HandleTimeUp(int dayCounterParam) {
		currentDay = dayCounterParam;
		ChangeUIElementText();
	}


	public override void _Ready() {
		_inGameTimer = GetTree().CurrentScene.GetNode<InGameTimer>("Gameplay_Stuff/InGameTimer");
		_inGameTimer.TimeUp += HandleTimeUp;

		dayCounterElementText = GetTree().CurrentScene.GetNode<RichTextLabel>("UI_Stuff/UI_Layer/DayCounterText");
		ChangeUIElementText();

		var noise = new FastNoiseLite();
		var seed = new RandomNumberGenerator();
		noise.Seed = seed.RandiRange(0, 1000);
		noise.Frequency = 0.01f;

		GenerateMap(noise);
	}
}
