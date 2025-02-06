using Barely;
using UnityEngine;

[System.Serializable]
public struct Move {
  public double position;
  public double duration;
  public int degree;
  public Vector3 direction;
}

// Instead of having separate types of Floater, Follower, and Thumper, this Automaton merges all
// into a single entity with a lifecycle continuum.
public class Automaton : MonoBehaviour {
  public Transform body;
  private Rigidbody _rigidBody;

  public Light haloLight;

  public Move[] followerMoves;

  public float floaterCorruptSpeed = 12.0f;
  public float floaterScaleSpeed = 2.0f;

  public int followerCountdownBeats = 32;
  private int _followerCountdownElapsedBeats = 0;
  private float _followerTransitionProgress = 0.0f;

  public AnimationClip thumperAnimation;
  public float thumperAnimationLerpSpeed = 24.0f;
  public double thumpPosition = 0.0;

  private float _thumperAnimationPosition = 0.0f;
  private bool _settingToThumper = false;
  private bool _transformToThumper = false;

  [System.Serializable]
  public struct StateProps {
    public Instrument instrument;
    public Color haloColor;
    public Color shadeColor;
    public Color rimLightColor;
    public float scale;
    public float hoveringNoiseSpeed;
    public float lowPassDistance;
    public float minInteractDistance;
    public float moveSpeed;
  }
  public StateProps floaterProps, followerProps, thumperProps;

  public int rootDegree = 0;

  public float scaleSpeed = 4.0f;

  private Material _material;
  private Performer _performer;
  private float _playerDistance = 0.0f;
  private float _rootPitch = 0.0f;

  private enum State {
    FLOATER,
    FOLLOWER,
    THUMPER,
  }
  private State _state = State.FLOATER;
  private StateProps _props;

  private Vector3 _direction = Vector3.zero;

  private Task[] _thumperTasks;
  private bool _isThumperPlaying = false;

  void Awake() {
    _rigidBody = body.GetComponent<Rigidbody>();
    _props = floaterProps;
    _material = body.GetComponent<Renderer>().material;

    _performer = GetComponent<Performer>();
    _thumperTasks = new Task[2];
    for (int i = 0; i < 2; ++i) {  // workaround 8 beat loop.
      _thumperTasks[i] = new Task(thumpPosition + 4.0 * i, 0.25, delegate(TaskState state) {
        if (_state == State.THUMPER && !_settingToThumper) {
          if (GameManager.Instance.State != GameState.OVER && state == TaskState.BEGIN) {
            thumperProps.instrument.SetNoteOn(_rootPitch);
            _isThumperPlaying = true;
          } else if (state == TaskState.END) {
            thumperProps.instrument.SetNoteOff(_rootPitch);
            _isThumperPlaying = false;
          }
        }
      });
      _performer.Tasks.Add(_thumperTasks[i]);
    }
    foreach (var move in followerMoves) {
      _performer.Tasks.Add(new Task(move.position, move.duration, delegate(TaskState state) {
        if (_state == State.FOLLOWER) {
          float pitch = GameManager.Instance.GetPitch(move.degree);
          if (GameManager.Instance.State != GameState.OVER && state == TaskState.BEGIN) {
            _direction = move.direction;
            followerProps.instrument.SetNoteOn(pitch);
          } else if (state == TaskState.END) {
            followerProps.instrument.SetNoteOff(pitch);
            _direction = Vector3.zero;
          }
        }
      }));
    }
  }

  void OnEnable() {
    _performer.OnBeat += OnBeat;
  }

  void OnDisable() {
    _performer.OnBeat -= OnBeat;
  }

  public bool CanInteract() {
    return GameManager.Instance.State == GameState.RUNNING && _performer.IsPlaying &&
           _playerDistance < _props.minInteractDistance && _state != State.THUMPER;
  }

  public float GetIntensity() {
    if (_state == State.THUMPER && !_settingToThumper && _isThumperPlaying) {
      return thumperProps.minInteractDistance / _playerDistance;
    } else if (_state == State.FOLLOWER) {
      return followerProps.minInteractDistance / _playerDistance;
    }
    return 0.0f;
  }

  // public bool IsThumperPlaying() {
  //   return _state == State.THUMPER && !_settingToThumper && _isThumperPlaying;
  // }

  public void TransformToThumper() {
    _rigidBody.isKinematic = false;

    float detune = Random.Range(-0.01f, 0.01f);
    floaterProps.instrument.SetNoteOn(_rootPitch + detune - 1.0f);

    float interactPitch = _rootPitch + detune + (_state == State.FOLLOWER ? 1.0f : 0.0f);
    followerProps.instrument.SetNoteOn(interactPitch, 0.75f);
    followerProps.instrument.SetNoteOff(interactPitch);
  }

  public void Play() {
    if (GameManager.Instance.IsDead()) {
      return;
    }
    // _performer.Stop();
    // TODO: This also can loop infinitely for some reason.
    // for (int i = 0; i < 2; ++i) {
    //   _thumperTasks[i].Position = thumpPosition + 4.0 * i;
    // }
    _performer.Play();
    _rootPitch = GameManager.Instance.GetPitch(rootDegree);
    SetState(State.FLOATER, floaterProps);
  }

  public void Stop() {
    floaterProps.instrument.SetAllNotesOff();
    followerProps.instrument.SetAllNotesOff();
    _performer.Stop();
    _performer.Position = 0.0;

    _followerCountdownElapsedBeats = 0;
    _followerTransitionProgress = 0.0f;

    _isThumperPlaying = false;

    _direction = Vector3.zero;
    _rigidBody.isKinematic = true;

    _settingToThumper = false;
    _transformToThumper = false;
    _state = State.FLOATER;
    _material.SetColor("_RimLightColor", floaterProps.rimLightColor);
    _material.SetColor("_1st_ShadeColor", floaterProps.shadeColor);
    haloLight.color = floaterProps.haloColor;
    body.localPosition = Vector3.zero;
    body.localScale = floaterProps.scale * Vector3.one;
    haloLight.range = 1.15f * transform.localScale.x * body.localScale.x;
    haloLight.intensity = 1.0f;
  }

  public void OnTriggerEnter(Collider collider) {
    if (collider.CompareTag("Player") && _isThumperPlaying) {
      float detune = Random.Range(-0.01f, 0.01f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 4.0f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 3.0f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 2.0f);
      followerProps.instrument.SetAllNotesOff();
      GameManager.Instance.GameOver();
      return;
    }

    if (!collider.CompareTag("Ground")) {
      return;
    }
    if (_state == State.THUMPER) {
      float detune = Random.Range(-0.01f, 0.01f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 3.0f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 2.0f);
      followerProps.instrument.SetAllNotesOff();
    } else if (!_rigidBody.isKinematic) {
      _transformToThumper = true;
      float detune = Random.Range(-0.01f, 0.01f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 2.0f);
      followerProps.instrument.SetNoteOn(_rootPitch + detune - 1.0f);
      followerProps.instrument.SetAllNotesOff();
    }
  }

  void Update() {
    // if (Input.GetKeyDown(KeyCode.Alpha1) && _state != State.FLOATER) {
    //   SetState(State.FLOATER, floaterProps);
    // } else if (Input.GetKeyDown(KeyCode.Alpha2) && _state != State.FOLLOWER) {
    //   SetState(State.FOLLOWER, followerProps);
    // } else if (Input.GetKeyDown(KeyCode.Alpha3) && _state != State.THUMPER) {
    //   SetState(State.THUMPER, thumperProps);
    // }

    _followerTransitionProgress =
        (_state == State.FLOATER)
            ? Mathf.Lerp(_followerTransitionProgress,
                         (float)_followerCountdownElapsedBeats / followerCountdownBeats,
                         Time.deltaTime * floaterCorruptSpeed)
            : 1.0f;
    _playerDistance = Vector3.Distance(body.transform.position, Camera.main.transform.position);

    UpdateProps();
  }

  private void SetState(State state, StateProps props) {
    _props = props;
    _state = state;

    haloLight.color = _props.haloColor;
    body.localScale = _props.scale * Vector3.one;

    _isThumperPlaying = false;

    _rigidBody.isKinematic = true;

    if (state == State.FLOATER) {
      _followerCountdownElapsedBeats = 0;
      _followerTransitionProgress = 0.0f;
      floaterProps.instrument.SetNoteOn(_rootPitch);
      _direction =
          (_direction + new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)))
              .normalized;
    }

    if (state != State.FOLLOWER) {
      followerProps.instrument.SetAllNotesOff();
    } else {
      floaterProps.instrument.SetAllNotesOff();
    }
  }

  private void UpdateProps() {
    if (!_performer.IsPlaying) {
      return;
    }

    Color rimLightColor = (_state == State.FLOATER)
                              ? Color.Lerp(floaterProps.rimLightColor, followerProps.rimLightColor,
                                           _followerTransitionProgress)
                              : _props.rimLightColor;
    Color shadeColor = (_state == State.FLOATER)
                           ? Color.Lerp(floaterProps.shadeColor, followerProps.shadeColor,
                                        _followerTransitionProgress)
                           : _props.shadeColor;
    _material.SetColor("_RimLightColor", rimLightColor);
    _material.SetColor("_1st_ShadeColor", shadeColor);

    haloLight.color = (_state == State.FLOATER)
                          ? Color.Lerp(floaterProps.haloColor, followerProps.haloColor,
                                       _followerTransitionProgress)
                          : _props.haloColor;

    body.localScale = Vector3.Lerp(
        body.localScale,
        ((_state == State.FLOATER)
             ? Mathf.Lerp(floaterProps.scale, followerProps.scale, _followerTransitionProgress)
             : _props.scale) *
            Vector3.one,
        Time.deltaTime * floaterScaleSpeed);

    var instrument = _props.instrument;

    if (_state != State.THUMPER) {
      transform.Translate(_direction * Time.deltaTime * _props.moveSpeed);
    }

    // Low-pass.
    instrument.FilterFrequency = Mathf.Exp(-_playerDistance / _props.lowPassDistance) * 48000.0f;

    // Player interaction.
    bool shouldInteract = (_playerDistance < _props.minInteractDistance);

    if (shouldInteract) {
      if (_state != State.FLOATER) {
        instrument.BitCrusherRate = Mathf.Pow(_playerDistance / _props.minInteractDistance, 2.0f);
      }
      if (_state == State.FOLLOWER) {
        transform.position =
            Vector3.Lerp(transform.position, Camera.main.transform.position,
                         0.75f * Time.deltaTime *
                             (_props.minInteractDistance /
                              (_playerDistance + 0.5f * _props.minInteractDistance)));
        if (!_settingToThumper && !_transformToThumper) {
          followerProps.instrument.SetNoteOn(-1.0f + _rootPitch, 1.0f);
          followerProps.instrument.SetNoteOn(2.0f + _rootPitch, 0.25f);
        }
      }
    } else {
      instrument.BitCrusherRate = (_state == State.FLOATER) ? 0.5f : 1.0f;
      followerProps.instrument.SetNoteOff(-1.0f + _rootPitch);
      followerProps.instrument.SetNoteOff(2.0f + _rootPitch);
    }

    if (_state == State.FLOATER) {
      instrument.PitchShift =
          Mathf.Lerp(instrument.PitchShift, (!_rigidBody.isKinematic) ? -2.0f : 0.0f,
                     Time.deltaTime * 0.5f * floaterCorruptSpeed);

      instrument.SetNoteControl(
          _rootPitch, NoteControlType.PITCH_SHIFT,
          shouldInteract ? (-1.0f + _playerDistance / _props.minInteractDistance) : 0.0f);

      instrument.BitCrusherDepth = 8.0f * (1.0f - 0.95f * _followerTransitionProgress);
      instrument.BitCrusherRate =
          0.5f * Mathf.Min(Mathf.Pow(1.75f - _followerTransitionProgress, 2.0f), 1.0f);
    }

    // Animate the thumper.
    if (_state == State.THUMPER) {
      _thumperAnimationPosition =
          Mathf.Lerp(_thumperAnimationPosition,
                     (float)(((_performer.Position - thumpPosition + 4.0) / 4.0) % 1.0),
                     thumperAnimationLerpSpeed * Time.deltaTime);
      Vector3 originalPosition = body.localPosition;
      thumperAnimation.SampleAnimation(body.gameObject, _thumperAnimationPosition);
      if (_settingToThumper) {
        body.localPosition =
            Vector3.Lerp(originalPosition, body.localPosition, Time.deltaTime * _props.moveSpeed);
        haloLight.range = 1.2f * transform.localScale.x * body.localScale.x;
      } else {
        haloLight.range *=
            1.15f * transform.localScale.x * body.localScale.x;  // should be uniform scale
      }
    } else {
      haloLight.range =
          1.2f * transform.localScale.x * body.localScale.x +
          ((_state == State.FLOATER) ? Mathf.Pow(_followerTransitionProgress, 2.0f) * 0.1f : 0.0f);
      haloLight.intensity = 1.0f;
    }

    // Hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.1f, 1.1f),
                                 Random.Range(-1.0f, 1.0f)) *
                         _props.scale,
                     Time.deltaTime * _props.hoveringNoiseSpeed);
  }

  private void OnBeat() {
    if (_transformToThumper && _state != State.THUMPER) {
      if (_performer.Position == thumpPosition) {
        _rigidBody.isKinematic = true;
        _transformToThumper = false;
        _settingToThumper = true;
        SetState(State.THUMPER, thumperProps);
      }
      return;
    }

    if (_state == State.FLOATER) {
      if (_playerDistance < floaterProps.minInteractDistance) {
        _followerCountdownElapsedBeats = Mathf.Max(_followerCountdownElapsedBeats - 8, 0);
      } else {
        ++_followerCountdownElapsedBeats;
        if (_followerCountdownElapsedBeats >= followerCountdownBeats) {
          SetState(State.FOLLOWER, followerProps);
        }
      }
    } else if (_state == State.THUMPER && _settingToThumper &&
               (_performer.Position % 4) == thumpPosition) {
      _settingToThumper = false;
      floaterProps.instrument.SetAllNotesOff();
    }
  }
}
