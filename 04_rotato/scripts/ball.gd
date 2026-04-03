extends RigidBody2D

@onready var instrument: BarelyInstrument = $BarelyInstrument

@export var init_pitch_offset = -1.0

var pitch = 0.0

const IMPACT_VELOCITY_SCALE = 0.000005

var _screenWidth = 0

func _ready() -> void:
	pitch = randf()
	_screenWidth = get_viewport_rect().size.x
	instrument.set_note_on(pitch + init_pitch_offset)
	instrument.set_note_off(pitch + init_pitch_offset)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	instrument.stereo_pan = 2.0 * (position.x / _screenWidth) - 1.0


func _on_body_entered(body: Node) -> void:
	var gain = min(IMPACT_VELOCITY_SCALE * linear_velocity.length_squared(), 1.0)
	instrument.set_note_on(pitch, gain)


func _on_body_exited(body: Node) -> void:
	instrument.set_note_off(pitch)
