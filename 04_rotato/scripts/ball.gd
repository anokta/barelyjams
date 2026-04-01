extends RigidBody2D

@onready var instrument: BarelyInstrument = $BarelyInstrument

var pitch = 0.0

const IMPACT_VELOCITY_SCALE = 0.00001
const MIN_GAIN = 0.1

var _screenWidth = 0

func _ready() -> void:
	pitch = randf()
	_screenWidth = get_viewport_rect().size.x


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	instrument.stereo_pan = 2.0 * (position.x / _screenWidth) - 1.0


func _on_body_entered(body: Node) -> void:
	var gain = min(IMPACT_VELOCITY_SCALE * linear_velocity.length_squared(), 1.0)
	if (gain < MIN_GAIN):
		return
	instrument.set_note_on(pitch, gain)


func _on_body_exited(body: Node) -> void:
	instrument.set_note_off(pitch)
