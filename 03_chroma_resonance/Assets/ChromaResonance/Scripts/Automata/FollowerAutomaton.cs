using Barely;
using UnityEngine;

[System.Serializable]
public struct Move {
  public double position;
  public double duration;
  public int degree;
  public Vector3 direction;
}

public class FollowerAutomaton : Automaton {
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
          _instrument.SetNoteOn(pitch);
        } else if (state == TaskState.END) {
          _instrument.SetNoteOff(pitch);
          _direction = Vector3.zero;
        }
      }));
    }
  }

  protected override void Update() {
    base.Update();

    transform.Translate(_direction * Time.deltaTime * speed);

    // Body hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                 Random.Range(-1.0f, 1.0f)),
                     Time.deltaTime * hoveringNoiseSpeed);

    if (PlayerDistance < minAttackDistance) {
      transform.position = Vector3.Lerp(
          transform.position, GameManager.Instance.player.transform.position,
          Time.deltaTime * (minAttackDistance / (PlayerDistance + 0.5f * minAttackDistance)));
      _instrument.BitCrusherRate = Mathf.Pow(PlayerDistance / minAttackDistance, 2.0f);
    } else {
      _instrument.BitCrusherRate = 1.0f;
    }
  }

  public void Toggle() {
    float pitch = performer.IsPlaying ? -2.0f : -1.0f;
    if (performer.IsPlaying) {
      _instrument.SetNoteOn(pitch);
      _instrument.SetNoteOff(pitch);
    } else {
      _instrument.SetNoteOn(pitch);
      _instrument.SetNoteOff(pitch);
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
