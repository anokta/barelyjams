extends Node2D

@export var ball_scene: PackedScene
@export var max_ball_init_velocity = Vector2.ZERO

var _screen_center = Vector2.ZERO

func _ready() -> void:
	BarelyEngine.lookahead = 0.0
	BarelyEngine.reverb_damping = 0.0
	BarelyEngine.reverb_room_size = 0.8
	
	_screen_center = 0.5 * get_viewport_rect().size

func _process(delta: float) -> void:
	pass

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			get_tree().quit()
			return
		
		if event.keycode == KEY_SPACE:
			var ball = ball_scene.instantiate()
			ball.position = _screen_center
			ball.linear_velocity = Vector2(
				randf_range(-max_ball_init_velocity.x, max_ball_init_velocity.x),
				randf_range(0.0, -max_ball_init_velocity.y)
			)
			add_child(ball)
