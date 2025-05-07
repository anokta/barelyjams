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
  public Metronome metronome;

  public float playbackMultiplier = 0.5f;

  public BlobAnimState initState;
  public BlobAnimState barState;

  private string _currentState = null;

  private void OnEnable() {
    metronome.OnBeat += OnBeat;
    _currentState = GetAnimStateString(initState);
  }

  private void OnDisable() {
    metronome.OnBeat -= OnBeat;
  }

  private void Update() {
    float playbackTime = Mathf.Repeat(playbackMultiplier * (float)metronome.Position, 1.0f);
    anim.Play(_currentState, 0, playbackTime);
  }

  private void OnBeat(int bar, int beat) {
    if (beat == 0) {
      _currentState = GetAnimStateString(barState);
    } else {
      _currentState = GetAnimStateString(initState);
    }
  }

  private string GetAnimStateString(BlobAnimState state) {
    return "CharacterArmature|" + state.ToString();
  }
}
