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

@onready var performer: BarelyPerformer = $BarelyPerformer

var _level: Node2D = null
var _walls: Array[Node2D] = []

var _balls: Array[Node2D] = []
var _held_ball: Node2D = null

@export var wallRotationBeats = 16.0
var _wallRotationMultiplier = -1.0

func _ready() -> void:
	RenderingServer.set_default_clear_color(background_color)

	BarelyEngine.lookahead = 0.0
	BarelyEngine.delay_time = 0.125
	BarelyEngine.delay_feedback = 0.2
	BarelyEngine.delay_ping_pong = 0.5
	BarelyEngine.delay_reverb_send = 1.0
	BarelyEngine.reverb_damping = 0.1
	BarelyEngine.reverb_room_size = 0.8
	
	performer.loop_length = wallRotationBeats * 4.0
	performer.position = wallRotationBeats
	performer.tasks[0].connect("task_begin", Callable(self, "_on_task_begin"))
	
	var screen_size = get_viewport_rect().size
	_level = Node2D.new()
	_level.name = "Level"
	_level.position = 0.5 * screen_size
	var min_screen_size = min(screen_size.x, screen_size.y)
	var wall_scale = Vector2(1.0 / 2.8, 0.0125)
	for wall_transform in wall_transforms:
		var wall = wall_scene.instantiate()
		wall.position = min_screen_size * Vector2(wall_transform.x, wall_transform.y)
		wall.rotation = deg_to_rad(wall_transform.z)
		wall.scale = wall_scale
		#wall.get_node("Sprite2D").modulate = 0.55 * Color.WHITE
		_level.add_child(wall)
	add_child(_level)
	
	performer.start()

func _process(delta: float) -> void:
	RenderingServer.set_default_clear_color(background_color)
	BarelyEngine.tempo = tempo
	_level.rotate(delta * rotationSpeed)
	for i in range(_level.get_child_count()):
		_level.get_child(i).rotation = deg_to_rad(wall_transforms[i].z) + _wallRotationMultiplier * min(performer.position, wallRotationBeats) * PI / wallRotationBeats
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
				
	if event is InputEventMouseMotion:
		if _held_ball:
			_held_ball.position = event.position

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

func _on_task_begin():
	_wallRotationMultiplier *= -1.0
