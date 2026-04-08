extends RigidBody2D

@onready var collision_shape: CollisionShape2D = $CollisionShape2D
@onready var sprite: Sprite2D = $Sprite2D
@onready var instrument: BarelyInstrument = $BarelyInstrument
@onready var performer: BarelyPerformer = $BarelyPerformer
@onready var particles: CPUParticles2D = $CPUParticles2D
@onready var label: Label = $Label

@export var max_init_velocity_x = 10
@export var octave_offset = -1.0

@export_range(0, 1, 0.01) var stereo_width = 0.8

@export var pitch_multiplier = 1.0

@export var destroy_timeout = 2.0
@export var min_scale = 0.1
@export var scale_multiplier = 0.85
@export var multiplier_lerp_speed = 16.0

@export var pitch_shift_per_hit = 0.2
@export var delay_send_per_hit = 0.05
@export var reverb_send_per_hit = 0.125
@export var distortion_mix_per_hit = 0.05

@export_range(0, 1, 0.01) var color_modulate_ratio = 0.75

var pitch = 0.0
var multiplier = 0.0

var initial_mass = 0.0
var initial_scale = Vector2.ZERO

const IMPACT_VELOCITY_SCALE = 0.0000075

func activate():
	performer.stop()
	instrument.set_note_off(pitch)
	instrument.attack = 0.0
	freeze = false
	collision_shape.disabled = false
	linear_velocity.x = randf_range(-max_init_velocity_x, max_init_velocity_x)
	
	var color_offset = Color.WHITE * (1.0 - 0.1 * color_modulate_ratio)
	sprite.modulate = color_offset + color_modulate_ratio * Color.from_hsv(randf(), 1.0, 1.0)
	particles.color = 0.5 * particles.color + 0.25 * sprite.modulate

func _ready() -> void:
	initial_mass = mass
	initial_scale = scale

	pitch = pitch_multiplier * randf() + octave_offset

	sprite.modulate = 0.125 * Color.BLACK

	instrument.set_note_on(pitch)
	performer.start()

func _process(delta: float) -> void:
	var screen_size = get_viewport_rect().size

	instrument.stereo_pan = clamp(stereo_width * (2.0 * (position.x / screen_size.x) - 1.0), -1.0, 1.0)
	if performer.is_playing():
		instrument.pitch_shift = 0.5 * pitch_multiplier * sin(PI * performer.position)

	var new_multiplier = (1.0 - scale_multiplier * (pitch + instrument.pitch_shift))
	multiplier = lerpf(multiplier, new_multiplier, multiplier_lerp_speed * delta)
	if multiplier <= min_scale or position.y > screen_size.y * 2.0:
		freeze = true
		collision_shape.disabled = true
		sprite.visible = false
		await get_tree().create_timer(destroy_timeout).timeout
		queue_free()
		return
	mass = initial_mass * multiplier
	scale = initial_scale * multiplier
	if freeze:
		scale *= 4.0

func _on_body_entered(body: Node) -> void:
	var gain = min(IMPACT_VELOCITY_SCALE * linear_velocity.length_squared(), 1.0)
	instrument.set_note_on(pitch, gain)
	particles.restart()

func _on_body_exited(body: Node) -> void:
	instrument.set_note_off(pitch)
	pitch = min(pitch + pitch_shift_per_hit, 4.0)
	instrument.delay_send = min(instrument.delay_send + delay_send_per_hit, 1.0)
	instrument.reverb_send = min(instrument.reverb_send + reverb_send_per_hit, 2.0)
	instrument.distortion_mix = min(instrument.distortion_mix + distortion_mix_per_hit, 1.0)
