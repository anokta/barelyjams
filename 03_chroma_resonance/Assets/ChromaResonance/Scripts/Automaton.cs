using Barely;
using UnityEngine;

[System.Serializable]
public struct Move {
  public double position;
  public double duration;
  public float pitch;
  public Vector3 direction;
}

public class Automaton : MonoBehaviour {
  public Instrument instrument;
  public Performer performer;
  public Move[] moves;

  public float speed = 1.0f;

  private Vector3 _direction = Vector3.forward;

  void Start() {
    performer.Loop = true;
    performer.LoopLength = moves[moves.Length - 1].position + moves[moves.Length - 1].duration;
    foreach (var move in moves) {
      performer.Tasks.Add(new Task(move.position, move.duration, delegate(TaskState state) {
        if (state == TaskState.BEGIN) {
          _direction = move.direction;
          instrument.SetNoteOn(move.pitch);
        } else if (state == TaskState.END) {
          instrument.SetNoteOff(move.pitch);
          _direction = Vector3.zero;
        }
      }));
    }
    performer.Play();
  }

  void Update() {
    transform.Translate(_direction * Time.deltaTime * speed);
  }
}
