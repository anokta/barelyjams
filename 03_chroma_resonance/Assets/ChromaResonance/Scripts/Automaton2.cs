using Barely;
using UnityEngine;
using UnityEngine.Animations;

// Instead of having separate types of Floater, Follower, and Thumper, this Automaton merges all
// into a single entity with a lifecycle continuum.
public class Automaton2 : MonoBehaviour {
  public Transform body;
  public Light haloLight;

  public Move[] followerMoves;

  private float floaterMoveSpeed = 0.4f;
  private float followerMoveSpeed = 2.0f;

  public AnimationClip thumperAnimation;
  public float thumperAnimationLerpSpeed = 24.0f;
  public double thumpPosition = 0.0;

  private float _thumperAnimationPosition = 0.0f;

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

  void Awake() {
    _props = floaterProps;
    _material = body.GetComponent<Renderer>().material;
    _performer = GetComponent<Performer>();
    _rootPitch = GameManager.Instance.GetPitch(rootDegree);
  }

  public void Play() {
    _performer.Play();
    SetState(State.FLOATER, floaterProps);
  }

  // void OnEnable() {
  //   GameManager.Instance.Performer.OnBeat += OnBeat;
  // }

  // void OnDisable() {
  //   GameManager.Instance.Performer.OnBeat -= OnBeat;
  // }

  void Start() {
    for (int i = 0; i < 2; ++i) {  // workaround 8 beat loop.
      _performer.Tasks.Add(new Task(thumpPosition + 4.0 * i, 0.25, delegate(TaskState state) {
        if (_state == State.THUMPER) {
          if (state == TaskState.BEGIN) {
            thumperProps.instrument.SetNoteOn(_rootPitch);
          } else if (state == TaskState.END) {
            thumperProps.instrument.SetNoteOff(_rootPitch);
          }
        }
      }));
    }
    foreach (var move in followerMoves) {
      _performer.Tasks.Add(new Task(move.position, move.duration, delegate(TaskState state) {
        if (_state == State.FOLLOWER) {
          float pitch = GameManager.Instance.GetPitch(move.degree);
          if (state == TaskState.BEGIN) {
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

  void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1) && _state != State.FLOATER) {
      SetState(State.FLOATER, floaterProps);
    } else if (Input.GetKeyDown(KeyCode.Alpha2) && _state != State.FOLLOWER) {
      SetState(State.FOLLOWER, followerProps);
    } else if (Input.GetKeyDown(KeyCode.Alpha3) && _state != State.THUMPER) {
      SetState(State.THUMPER, thumperProps);
    }

    _playerDistance = Vector3.Distance(body.transform.position, Camera.main.transform.position);

    UpdateProps();
  }

  private void SetState(State state, StateProps props) {
    _props = props;
    _state = state;

    haloLight.color = _props.haloColor;

    if (state == State.FLOATER) {
      floaterProps.instrument.SetNoteOn(_rootPitch);
      _direction =
          (_direction + new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)))
              .normalized;
    } else {
      floaterProps.instrument.SetAllNotesOff();
    }

    if (state != State.FOLLOWER) {
      followerProps.instrument.SetAllNotesOff();
    }

    switch (state) {
      case State.FLOATER:
        break;
      case State.FOLLOWER:
        break;
      case State.THUMPER:
        break;
    }
  }

  private void UpdateColor(string colorName, Color targetColor, float lerpSpeed) {
    _material.SetColor(colorName, targetColor);
    // Color currentColor = _material.GetColor(colorName);
    // _material.SetColor(colorName,
    //                    Color.Lerp(currentColor, targetColor, Time.deltaTime * lerpSpeed));
  }

  private void UpdateProps() {
    UpdateColor("_RimLightColor", _props.rimLightColor, 1.0f);
    UpdateColor("_1st_ShadeColor", _props.shadeColor, 1.0f);
    body.localScale = _props.scale * Vector3.one;

    var instrument = _props.instrument;

    if (_state != State.THUMPER) {
      transform.Translate(_direction * Time.deltaTime *
                          ((_state == State.FLOATER) ? floaterMoveSpeed : followerMoveSpeed));
    }

    // Low-pass.
    instrument.FilterFrequency = Mathf.Exp(-_playerDistance / _props.lowPassDistance) * 48000.0f;

    // Player interaction.
    bool shouldInteract = _performer.IsPlaying && (_playerDistance < _props.minInteractDistance);
    if (_state == State.FLOATER) {
      instrument.SetNoteControl(
          _rootPitch, NoteControlType.PITCH_SHIFT,
          shouldInteract ? (-1.0f + _playerDistance / _props.minInteractDistance) : 0.0f);
    }
    if (shouldInteract) {
      if (_state != State.FLOATER) {
        instrument.BitCrusherRate = Mathf.Pow(_playerDistance / _props.minInteractDistance, 2.0f);
      }
      if (_state == State.FOLLOWER) {
        transform.position =
            Vector3.Lerp(transform.position, Camera.main.transform.position,
                         Time.deltaTime * (_props.minInteractDistance /
                                           (_playerDistance + 0.5f * _props.minInteractDistance)));
      }
    } else {
      instrument.BitCrusherRate = (_state == State.FLOATER) ? 0.5f : 1.0f;
    }

    // Animate the thumper.
    if (_state == State.THUMPER) {
      _thumperAnimationPosition =
          Mathf.Lerp(_thumperAnimationPosition,
                     (float)(((_performer.Position - thumpPosition + 4.0) / 4.0) % 1.0),
                     thumperAnimationLerpSpeed * Time.deltaTime);
      thumperAnimation.SampleAnimation(body.gameObject, _thumperAnimationPosition);
      haloLight.range *=
          1.1f * transform.localScale.x * body.localScale.x;  // should be uniform scale
    } else {
      haloLight.range = 1.15f * transform.localScale.x * body.localScale.x;
      haloLight.intensity = 1.0f;
    }

    // Hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                 Random.Range(-1.0f, 1.0f)) *
                         _props.scale,
                     Time.deltaTime * _props.hoveringNoiseSpeed);
  }

  // private void OnBeat() {
  //   if (GameManager.Instance.Performer.Position == 2.0) {
  //     _performer.Play();
  //   }
  // }
}
