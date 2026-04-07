extends Node2D

@export var background_color: Color = Color.BLACK

@export_group("Level")
@export var wall_scene: PackedScene
@export var wall_transforms : Array[Vector3]
@export_group("")

@export var ball_scene: PackedScene
@export var max_balls: int = 24
@export var tempo: float = 60.0

@export var rotationSpeed: float = 0.0
@export var wallRotationSpeed: float = 0.0

var _level: Node2D = null
var _walls: Array[Node2D] = []

var _balls: Array[Node2D] = []
var _held_ball: Node2D = null

func _ready() -> void:
	RenderingServer.set_default_clear_color(background_color)

	BarelyEngine.lookahead = 0.0
	BarelyEngine.delay_time = 0.125
	BarelyEngine.delay_feedback = 0.2
	BarelyEngine.delay_ping_pong = 0.5
	BarelyEngine.delay_reverb_send = 1.0
	BarelyEngine.reverb_damping = 0.1
	BarelyEngine.reverb_room_size = 0.8
	
	var screen_size = get_viewport_rect().size
	_level = Node2D.new()
	_level.name = "Level"
	_level.position = 0.5 * screen_size
	var wall_scale = Vector2(1.0 / 2.96, 0.01)
	for wall_transform in wall_transforms:
		var wall = wall_scene.instantiate()
		wall.position = screen_size * Vector2(wall_transform.x, wall_transform.y)
		wall.rotation = deg_to_rad(wall_transform.z)
		wall.scale = wall_scale
		_level.add_child(wall)
	add_child(_level)

func _process(delta: float) -> void:
	RenderingServer.set_default_clear_color(background_color)
	BarelyEngine.tempo = tempo
	_level.rotate(delta * rotationSpeed)
	for wall in _level.get_children():
		wall.rotate(delta * wallRotationSpeed)
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
