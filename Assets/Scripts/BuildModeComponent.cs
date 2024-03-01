using System;
using Godot;

public partial class BuildModeComponent : Node2D {
	private int playerMoney = 100;
	private TileMap tileMap;
	private RichTextLabel selectedModeTextBox;
	private RichTextLabel selectedTileTextBox;
	private RichTextLabel selectedMoneyTextBox;

	private Vector2I tileCoords;

	private Vector2I? selectedTileAtlasCoords = null;

	private short mode = 0;

	private static readonly Vector2I[] tileAtlasCoords = {
		new Vector2I(0, 0), // Grass
		new Vector2I(1, 0), // Hill
		new Vector2I(2, 0), // Mountain
		new Vector2I(0, 1), // Path
		new Vector2I(1, 1), // Water
		new Vector2I(0, 2), // Field
	};

	private static readonly Vector2I[] wheatPlantStages = {
		new Vector2I(0, 0), // Seed
		new Vector2I(1, 0), // Sprout
		new Vector2I(2, 0), // Young1
		new Vector2I(3, 0), // Young2
		new Vector2I(4, 0), // Mature
	};

	private bool isOutOfBounds (Vector2I coords) {
		return coords.X < 0 || coords.Y < 0 || coords.X >= tileMap.GetUsedRect().Size.X || coords.Y >= tileMap.GetUsedRect().Size.Y;
	}

	private void highlightTile() {
		tileCoords = tileMap.LocalToMap(GetGlobalMousePosition());
		tileMap.ClearLayer(3);
		if (isOutOfBounds(tileCoords)) {
			return;
		} else {
			tileMap.SetCell(3, tileCoords, 1, new Vector2I(0, 0));
		}
	}

	private void handleClick() {
		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
			if (mode == 0) {
				return;
			} else if (mode == 1) {
				if (isOutOfBounds(tileCoords) || selectedTileAtlasCoords == null) {
					return;
				} else {
					if ((tileMap.GetCellAtlasCoords(0, tileCoords).Equals(tileAtlasCoords[0]) || tileMap.GetCellAtlasCoords(0, tileCoords).Equals(tileAtlasCoords[1])) && tileMap.GetCellTileData(1, tileCoords) == null) {
						if (playerMoney >= 2) {
							tileMap.SetCell(1, tileCoords, 0, selectedTileAtlasCoords);
							playerMoney -= 2;
							changeMoneyText();
						} else {
							return;
						}
					}
				}
			} else if (mode == 2) {
				if (isOutOfBounds(tileCoords) || selectedTileAtlasCoords == null) {
					return;
				} else {
					if (tileMap.GetCellAtlasCoords(1, tileCoords).Equals(tileAtlasCoords[5]) && tileMap.GetCellTileData(2, tileCoords) == null) {
						if (playerMoney >= 20) {
							tileMap.SetCell(2, tileCoords, 2, new Vector2I(0, 0));
							playerMoney -= 20;
							changeMoneyText();
						} else {
							return;
						}
					}	
				}
			} else if (mode == 3) {
				if (isOutOfBounds(tileCoords)) {
					return;
				} else {
					if (tileMap.GetCellAtlasCoords(2, tileCoords).Equals(wheatPlantStages[4])) {
						tileMap.SetCell(2, tileCoords, -1);
						playerMoney += 30;
						changeMoneyText();
					} else {
						return;
					}
				}
			}
		}
		
		if (Input.IsMouseButtonPressed(MouseButton.Right)) {
			if (mode == 0) {
				return;
			} else if (mode == 1) {
				if (isOutOfBounds(tileCoords)) {
					return;
				} else {
					tileMap.SetCell(1, tileCoords, -1);
				}
			}
		}	
	}

	private void handleKeyboardInput() {
		if (Input.IsActionJustPressed("keyboard_select_field")) {
			selectedTileAtlasCoords = tileAtlasCoords[5];
			changeTileText("Field");
		}
		if (Input.IsActionJustPressed("keyboard_select_path")) {
			selectedTileAtlasCoords = tileAtlasCoords[3];
			changeTileText("Path");
		}
		if (Input.IsActionJustPressed("keyboard_switch_mode")) {
			switch (mode) {
				case 0: // None Mode
					mode = 1;
					chaneModeText("Build");
					break;
				case 1: // Build Mode
					mode = 2;
					chaneModeText("Plant");
					break;
				case 2: // Plant Mode
					mode = 3;
					chaneModeText("Harvest");
					break;
				case 3: // Harvest Mode
					mode = 0;
					chaneModeText("None");
					break;
			}
		}
	}

	private void HandleTimeUp(int dayCounterParam) {
		playerMoney += 10;
		changeMoneyText();
		handlePlantGrowth();
	}

	private void handlePlantGrowth() {
		var usedCells = tileMap.GetUsedCellsById(2);
		foreach (var cell in usedCells) {
			var tileData = tileMap.GetCellAtlasCoords(2, cell);
			if (tileData.Equals(wheatPlantStages[0])) {
				tileMap.SetCell(2, cell, 2, wheatPlantStages[1]);
			} else if (tileData.Equals(wheatPlantStages[1])) {
				tileMap.SetCell(2, cell, 2, wheatPlantStages[2]);
			} else if (tileData.Equals(wheatPlantStages[2])) {
				tileMap.SetCell(2, cell, 2, wheatPlantStages[3]);
			} else if (tileData.Equals(wheatPlantStages[3])) {
				tileMap.SetCell(2, cell, 2, wheatPlantStages[4]);
			}
		}
	}

	private void changeTileText(string text) {
		selectedTileTextBox.Text = "Selected Tile: " + text;
	}

	private void chaneModeText(string text) {
		selectedModeTextBox.Text = "Mode: " + text;
	}

	private void changeMoneyText() {
		selectedMoneyTextBox.Text = "Money: " + playerMoney;
	}

	public override void _Ready() {
		tileMap = GetTree().CurrentScene.GetNode<TileMap>("Map");
		selectedModeTextBox = GetTree().CurrentScene.GetNode<RichTextLabel>("UI_Stuff/UI_Layer/ModeText");
		selectedTileTextBox = GetTree().CurrentScene.GetNode<RichTextLabel>("UI_Stuff/UI_Layer/TileText");
		selectedMoneyTextBox = GetTree().CurrentScene.GetNode<RichTextLabel>("UI_Stuff/UI_Layer/MoneyText");

		chaneModeText("None");
		changeTileText("None");
		changeMoneyText();
	}

    public override void _Process(double delta) {
		handleKeyboardInput();
		highlightTile();
		handleClick();
	}
}
