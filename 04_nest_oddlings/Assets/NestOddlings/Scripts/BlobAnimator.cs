using Barely;
using Barely.Examples;
using UnityEngine;

public enum BlobAnimState {
  Idle = 0,
  Bite_Front,
  Dance,
  Death,
  HitRecieve,
  Jump,
  No,
  Walk,
  Yes,
}

public class BlobAnimator : MonoBehaviour {
  public Animator anim;
  public Instrument instrument;
  public Metronome metronome;

  public float playbackMultiplier = 0.5f;
  public float animCrossfadeTime = 0.1f;

  public BlobAnimState idleState;
  public BlobAnimState noteOnState;

  private string _currentState = null;
  private string _targetState = null;

  private void OnEnable() {
    metronome.OnBeat += OnBeat;
    instrument.OnNoteOn += OnNoteOn;
    instrument.OnNoteOff += OnNoteOff;
    _currentState = GetAnimStateString(idleState);
    _targetState = _currentState;
  }

  private void OnDisable() {
    metronome.OnBeat -= OnBeat;
    instrument.OnNoteOn -= OnNoteOn;
    instrument.OnNoteOff -= OnNoteOff;
  }

  private void Update() {
    float playbackTime = Mathf.Repeat(playbackMultiplier * (float)metronome.Position, 1.0f);
    if (_currentState != _targetState) {
      anim.CrossFade(_targetState, animCrossfadeTime, 0, playbackTime);
    } else {
      anim.Play(_currentState, 0, playbackTime);
    }
  }

  private string GetAnimStateString(BlobAnimState state) {
    return "CharacterArmature|" + state.ToString();
  }

  private void OnBeat(int bar, int beat) {
    _currentState = _targetState;
  }

  private void OnNoteOn(float pitch) {
    _targetState = GetAnimStateString(noteOnState);
    // _currentState = _targetState;  // immediately transition for better responsivity.
  }

  private void OnNoteOff(float pitch) {
    _targetState = GetAnimStateString(idleState);
  }
}
