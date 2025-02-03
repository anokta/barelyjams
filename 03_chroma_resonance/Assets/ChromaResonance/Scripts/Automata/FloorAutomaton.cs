using Barely;
using UnityEngine;

public class FloorAutomaton : Automaton {
  public int beatCount = 4;
  public double duration = 1.0;

  // public float teleportSpeed = 4.0f;
  // public float zOffset = 1.0f;

  // private Vector3 _origin = Vector3.zero;
  // private int _beat = 0;

  void Start() {
    // _origin = transform.position;
    _performer.Tasks.Add(new Task(0.0, duration, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        // transform.position = _origin + Vector3.forward * (_beat * zOffset);
        _instrument.SetNoteOn(0.0f);
        _instrument.SetNoteControl(0.0f, NoteControlType.PITCH_SHIFT,
                                   Random.Range(-0.001f, 0.001f));
        body.gameObject.SetActive(true);
        // _beat = (_beat + 1) % beatCount;
      } else if (state == TaskState.END) {
        _instrument.SetNoteOff(0.0f);
        body.gameObject.SetActive(false);
      }
    }));
  }

  protected override void Update() {
    base.Update();

    // transform.position =
    //     Vector3.Lerp(transform.position, _origin + Vector3.forward * (_beat * zOffset),
    //                  Time.deltaTime * teleportSpeed);
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
