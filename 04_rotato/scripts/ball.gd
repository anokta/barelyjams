extends RigidBody2D

@onready var collision_shape: CollisionShape2D = $CollisionShape2D
@onready var sprite: Sprite2D = $Sprite2D
@onready var instrument: BarelyInstrument = $BarelyInstrument
@onready var performer: BarelyPerformer = $BarelyPerformer

@export var max_init_velocity_x = 10
@export var octave_offset = -1.0
@export var scale_multiplier = 0.75
@export_range(0, 1, 0.01) var color_modulate_ratio = 0.75

var pitch = 0.0

var initial_mass = 0.0
var initial_scale = Vector2.ZERO
var initial_shape_scale = Vector2.ZERO

const IMPACT_VELOCITY_SCALE = 0.000005

func activate():
	performer.stop()
	instrument.set_note_off(pitch)
	instrument.attack = 0.0
	freeze = false
	linear_velocity.x = randf_range(-max_init_velocity_x, max_init_velocity_x)

func _ready() -> void:
	initial_mass = mass
	initial_scale = scale
	initial_shape_scale = collision_shape.scale
	
	pitch = randf() + octave_offset
	
	var color_offset = Color.WHITE * (1.0 - color_modulate_ratio)
	sprite.modulate = color_offset + color_modulate_ratio * Color.from_hsv(randf(), 1.0, 1.0)

	instrument.set_note_on(pitch)
	performer.start()

func _process(delta: float) -> void:
	instrument.stereo_pan = 2.0 * (position.x / get_viewport_rect().size.x) - 1.0
	if performer.is_playing():
		instrument.pitch_shift = 0.5 * sin(PI * performer.position)
	
	var multiplier = (1.0 - scale_multiplier * (pitch + instrument.pitch_shift))
	mass = initial_mass * multiplier
	scale = initial_scale * multiplier
	collision_shape.scale = initial_shape_scale * multiplier

func _on_body_entered(body: Node) -> void:
	var gain = min(IMPACT_VELOCITY_SCALE * linear_velocity.length_squared(), 1.0)
	instrument.set_note_on(pitch, gain)

func _on_body_exited(body: Node) -> void:
	instrument.set_note_off(pitch)
