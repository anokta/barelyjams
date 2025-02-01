using Barely;
using UnityEngine;

public class ThumperAutomaton : Automaton {
  public AnimationClip animationClip;
  public Light spotLight;

  public float animationLerpSpeed = 24.0f;
  public double thumpPosition = 0.0;

  private float _animationPosition = 0.0f;

  void Start() {
    _performer.Tasks.Add(new Task(thumpPosition, 0.25, delegate(TaskState state) {
      float pitch = GameManager.Instance.GetPitch(0);
      if (state == TaskState.BEGIN) {
        _instrument.SetNoteOn(pitch);
      } else if (state == TaskState.END) {
        _instrument.SetNoteOff(pitch);
      }
    }));
  }

  protected override void Update() {
    base.Update();

    // Animate the automaton.
    _animationPosition =
        Mathf.Lerp(_animationPosition,
                   (float)(((_performer.Position - thumpPosition + _performer.LoopLength) /
                            _performer.LoopLength) %
                           1.0),
                   animationLerpSpeed * Time.deltaTime);
    animationClip.SampleAnimation(gameObject, _animationPosition);
    spotLight.range /= transform.localScale.x;  // should be uniform scale
  }

  public override void Toggle() {
    if (_performer.IsPlaying) {
      _performer.Stop();
      _performer.Position = _performer.LoopLength * _animationPosition;
    } else {
      _performer.Position = GameManager.Instance.Performer.Position;
      _performer.Play();
    }
  }
}
