extends Node2D

func _ready() -> void:
	pass

func _process(delta: float) -> void:
	print(BarelyEngine.get_timestamp())

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			get_tree().quit()
			return
