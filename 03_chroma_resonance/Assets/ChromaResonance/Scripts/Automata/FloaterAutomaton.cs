using Barely;
using UnityEngine;

public class FloaterAutomaton : Automaton {
  public int[] degrees;

  public float speed = 4.0f;

  public float minDetuneDistance = 8.0f;

  private bool _isPlaying = false;

  private int _degreeOffset = 0;
  private Vector3 _direction = Vector3.zero;

  protected override void Update() {
    base.Update();

    transform.Translate(speed * Time.deltaTime * _direction);

    if (_isPlaying) {
      foreach (int degree in degrees) {
        float pitch = GameManager.Instance.GetPitch(_degreeOffset + degree);
        _instrument.SetNoteControl(pitch, NoteControlType.PITCH_SHIFT,
                                   PlayerDistance > minDetuneDistance
                                       ? 0.0f
                                       : (-1.0f + PlayerDistance / minDetuneDistance));
      }
    }
  }

  public override void Toggle() {
    if (_isPlaying) {
      _isPlaying = false;
      _instrument.SetAllNotesOff();
      _direction = Vector3.zero;
    } else {
      _isPlaying = true;
      _degreeOffset = Random.Range(-GameManager.Instance.scale.PitchCount / 2,
                                   GameManager.Instance.scale.PitchCount);
      foreach (int degree in degrees) {
        float pitch = GameManager.Instance.GetPitch(_degreeOffset + degree);
        _instrument.SetNoteOn(pitch);
      }
      _direction =
          new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;
    }
  }
}
