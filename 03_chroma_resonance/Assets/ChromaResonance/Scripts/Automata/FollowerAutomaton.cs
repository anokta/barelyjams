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
  public float minAttackDistance = 10.0f;

  public int degreeOffset = 0;
  public float speed = 1.0f;

  public Move[] moves;

  private Vector3 _direction = Vector3.zero;

  private bool _toggleNextBeat = false;

  void OnEnable() {
    GameManager.Instance.Performer.OnBeat += OnBeat;
  }

  void OnDisable() {
    GameManager.Instance.Performer.OnBeat -= OnBeat;
  }

  void Start() {
    _performer.Loop = true;
    _performer.LoopLength = moves[moves.Length - 1].position + moves[moves.Length - 1].duration;
    foreach (var move in moves) {
      _performer.Tasks.Add(new Task(move.position, move.duration, delegate(TaskState state) {
        float pitch = GameManager.Instance.GetPitch(move.degree + degreeOffset);
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

    if (_performer.IsPlaying && PlayerDistance < minAttackDistance) {
      transform.position = Vector3.Lerp(
          transform.position, GameManager.Instance.player.transform.position,
          Time.deltaTime * (minAttackDistance / (PlayerDistance + 0.5f * minAttackDistance)));
      _instrument.BitCrusherRate = Mathf.Pow(PlayerDistance / minAttackDistance, 2.0f);
    } else {
      _instrument.BitCrusherRate = 1.0f;
    }
  }

  public override void Toggle() {
    float pitch =
        (_performer.IsPlaying ? -2.0f : -1.0f) + GameManager.Instance.GetPitch(degreeOffset);
    if (_performer.IsPlaying) {
      _instrument.SetNoteOn(pitch);
      _instrument.SetNoteOff(pitch);
    } else {
      _instrument.SetNoteOn(pitch);
      _instrument.SetNoteOff(pitch);
    }
    _toggleNextBeat = true;
  }

  private void OnBeat() {
    if (_toggleNextBeat && GameManager.Instance.Performer.Position == 0.0) {
      _toggleNextBeat = false;
      if (_performer.IsPlaying) {
        _performer.Stop();
        _performer.Position = 0.0;
      } else {
        _performer.Play();
      }
    }
  }
}
