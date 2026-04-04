extends Node2D

@export var ball_scene: PackedScene
@export var max_balls: int = 24
@export var tempo: float = 60.0

var _balls: Array[Node2D] = []
var _held_ball: Node2D = null

func _ready() -> void:
	BarelyEngine.lookahead = 0.0
	BarelyEngine.reverb_damping = 0.0
	BarelyEngine.reverb_room_size = 0.8

func _process(delta: float) -> void:
	BarelyEngine.tempo = tempo
	pass

func _input(event):
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_ESCAPE:
			get_tree().quit()
			return

	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT:
			if event.pressed:
				_spawn_ball(event.position)
			elif _held_ball:
				_held_ball.activate()
				_held_ball = null

func _spawn_ball(position: Vector2):
	if _balls.size() >= max_balls:
		var oldest_ball = _balls.pop_front()
		if is_instance_valid(oldest_ball):
			oldest_ball.queue_free()

	var ball = ball_scene.instantiate()
	ball.position = position
	add_child(ball)

	_balls.append(ball)
	_held_ball = ball
