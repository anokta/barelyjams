using Barely;
using UnityEngine;

public class FloorAutomaton : Automaton {
  public double duration = 1.0;

  public float lightIntensity = 1000.0f;
  public float lightSpeed = 16.0f;

  private Light _spotLight;
  private float _targetIntensity = 0.0f;

  public double Pos {
    get { return _performer.Position; }
  }

  void Start() {
    _spotLight = body.GetComponent<Light>();
    _performer.Tasks.Add(new Task(0.0, duration, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        _instrument.SetNoteOn(0.0f);
        _instrument.SetNoteControl(0.0f, NoteControlType.PITCH_SHIFT,
                                   Random.Range(-0.001f, 0.001f));
        _targetIntensity = lightIntensity;
      } else if (state == TaskState.END) {
        _instrument.SetNoteOff(0.0f);
        _targetIntensity = 0.0f;
      }
    }));
  }

  protected override void Update() {
    base.Update();
    _spotLight.intensity =
        Mathf.Lerp(_spotLight.intensity, _targetIntensity, Time.deltaTime * lightSpeed);
  }

  public void Play() {
    _performer.Position = 0.0;
    _performer.Play();
  }

  public void Stop() {
    _performer.Stop();
  }

  public override void Toggle() {
    float pitch = Random.Range(-1.0f, 1.0f);
    _instrument.SetNoteOn(pitch);
    _instrument.SetNoteOff(pitch);
  }
}
