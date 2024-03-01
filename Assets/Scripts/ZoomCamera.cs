using System;
using System.Globalization;
using Godot;

public partial class ZoomCamera : Camera2D {
	private Camera2D camera;

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventMouseButton) {
			if (@event.IsPressed()) {
				var mouseEvent = @event as InputEventMouseButton;
				if (mouseEvent.ButtonIndex == MouseButton.WheelUp) {
					if (Zoom.X < 1.0f && Zoom.Y < 1.0f) {
						Zoom += new Vector2(0.1f, 0.1f);
					}
				} else if (mouseEvent.ButtonIndex == MouseButton.WheelDown) {
					if (Zoom.X > 0.1f && Zoom.Y > 0.1f) {
						Zoom -= new Vector2(0.1f, 0.1f);
					}
				}
			}
		}
		if (@event is InputEventMouseMotion) {
			var mouseEvent = @event as InputEventMouseMotion;
			if (mouseEvent.ButtonMask == MouseButtonMask.Middle) {
				Position -= mouseEvent.Relative * 1.5f;
			}
		}
	}

}
