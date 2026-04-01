extends Node2D

func _ready() -> void:
	BarelyEngine.lookahead = 0.0
	BarelyEngine.reverb_damping = 0.0
	BarelyEngine.reverb_room_size = 0.8

func _process(delta: float) -> void:
	pass

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			get_tree().quit()
			return
