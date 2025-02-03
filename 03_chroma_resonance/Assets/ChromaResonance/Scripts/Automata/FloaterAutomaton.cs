using Barely;
using UnityEngine;

public class FloaterAutomaton : Automaton {
  public int[] degrees;

  public float speed = 4.0f;

  private bool _isPlaying = false;

  private int _degreeOffset = 0;
  private Vector3 _direction = Vector3.zero;

  void OnEnable() {
    GameManager.Instance.Performer.OnBeat += OnBeat;
  }

  void OnDisable() {
    GameManager.Instance.Performer.OnBeat -= OnBeat;
  }

  protected override void Update() {
    base.Update();

    transform.Translate(speed * Time.deltaTime * _direction);

    if (_isPlaying) {
      foreach (int degree in degrees) {
        float pitch = GameManager.Instance.GetPitch(_degreeOffset + degree);
        _instrument.SetNoteControl(pitch, NoteControlType.PITCH_SHIFT,
                                   PlayerDistance > minAttackDistance
                                       ? 0.0f
                                       : (-1.0f + PlayerDistance / minAttackDistance));
      }
    }
  }

  public override void Toggle() {
    if (_isPlaying) {
      _instrument.SetAllNotesOff();
      _isPlaying = false;
      _direction = Vector3.zero;
    } else {
      _isPlaying = true;
      _degreeOffset = Random.Range(-GameManager.Instance.scale.PitchCount / 2,
                                   GameManager.Instance.scale.PitchCount);
      StartPlaying(-1.0f);
    }
  }

  private void OnBeat() {
    if (_isPlaying) {
      if (_direction == Vector3.zero) {
        _instrument.SetAllNotesOff();
        StartPlaying(0.0f);
      }
      if (GameManager.Instance.Performer.Position == 0.0) {
        _direction =
            (_direction + new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)))
                .normalized;
      }
    }
  }

  private void StartPlaying(float pitchOffset) {
    foreach (int degree in degrees) {
      float pitch = pitchOffset + GameManager.Instance.GetPitch(_degreeOffset + degree);
      _instrument.SetNoteOn(pitch);
    }
  }
}
