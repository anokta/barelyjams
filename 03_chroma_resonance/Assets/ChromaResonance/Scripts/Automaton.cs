using Barely;
using UnityEngine;

[System.Serializable]
public struct Move {
  public double position;
  public double duration;
  public int degree;
  public Vector3 direction;
}

public class Automaton : MonoBehaviour {
  public Transform body;

  public Instrument instrument;
  public Performer performer;

  public float minAttackDistance = 10.0f;

  public float speed = 1.0f;
  public float hoveringNoiseSpeed = 1.0f;

  public Move[] moves;

  private Vector3 _direction = Vector3.zero;

  bool toggleNextBeat = false;

  void OnEnable() {
    GameManager.Instance.Performer.OnBeat += OnBeat;
  }

  void OnDisable() {
    GameManager.Instance.Performer.OnBeat -= OnBeat;
  }

  void Start() {
    performer.Loop = true;
    performer.LoopLength = moves[moves.Length - 1].position + moves[moves.Length - 1].duration;
    foreach (var move in moves) {
      performer.Tasks.Add(new Task(move.position, move.duration, delegate(TaskState state) {
        float pitch = GameManager.Instance.GetPitch(move.degree);
        if (state == TaskState.BEGIN) {
          _direction = move.direction;
          instrument.SetNoteOn(pitch);
        } else if (state == TaskState.END) {
          instrument.SetNoteOff(pitch);
          _direction = Vector3.zero;
        }
      }));
    }
  }

  void Update() {
    transform.Translate(_direction * Time.deltaTime * speed);

    // Body hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                 Random.Range(-1.0f, 1.0f)),
                     Time.deltaTime * hoveringNoiseSpeed);

    float playerDistance =
        Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);
    if (playerDistance < minAttackDistance) {
      transform.position = Vector3.Lerp(
          transform.position, GameManager.Instance.player.transform.position,
          Time.deltaTime * (minAttackDistance / (playerDistance + 0.5f * minAttackDistance)));
      instrument.BitCrusherRate = Mathf.Pow(playerDistance / minAttackDistance, 2.0f);
    } else {
      instrument.BitCrusherRate = 1.0f;
    }
  }

  public void Toggle() {
    float pitch = performer.IsPlaying ? -2.0f : -1.0f;
    if (performer.IsPlaying) {
      instrument.SetNoteOn(pitch);
      instrument.SetNoteOff(pitch);
    } else {
      instrument.SetNoteOn(pitch);
      instrument.SetNoteOff(pitch);
    }
    toggleNextBeat = true;
  }

  private void OnBeat() {
    if (toggleNextBeat && GameManager.Instance.Performer.Position == 0.0) {
      toggleNextBeat = false;
      if (performer.IsPlaying) {
        performer.Stop();
        performer.Position = 0.0;
      } else {
        performer.Play();
      }
    }
  }
}
