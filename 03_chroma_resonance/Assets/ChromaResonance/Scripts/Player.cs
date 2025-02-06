using Barely;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class Player : MonoBehaviour {
  public CharacterController character;
  public FirstPersonController firstPerson;
  public Transform fork;

  public float clickScale = 0.8f;
  public float hoverScale = 1.2f;
  public float clickScaleSpeed = 1.0f;
  private Vector3 _idleScale = Vector3.one;

  public float idleNoiseVolume = 0.1f;
  public float noiseSpeed = 4.0f;

  public LayerMask interactionLayerMask;
  public float maxInteractionDistance = 5.0f;

  public int survivalBeatCount = 8;
  private int _elapsedSurvivalBeatCount = 0;

  public PostProcessProfile postProcessProfile;

  public float postProcessingRadius = 4.0f;
  public float postProcessingSpeed = 4.0f;
  private float _postProcessingIntensity = 0.0f;
  private float _postProcessingTargetIntensity = 0.0f;

  public float idleBloomIntensity = 2.76f;
  public float bloomIntensity = 5.0f;
  private Bloom _bloom;

  public float idleGrainIntensity = 0.28f;
  public float grainIntensity = 1.0f;
  public float idleGrainSize = 1.65f;
  public float grainSize = 4.0f;
  private Grain _grain;

  private Collider[] _overlapSphereColliders;

  private Instrument _instrument = null;

  void Awake() {
    _overlapSphereColliders = new Collider[1];
    _idleScale = fork.localScale;
    postProcessProfile.TryGetSettings(out _grain);
    postProcessProfile.TryGetSettings(out _bloom);
  }

  void OnEnable() {
    GameManager.Instance.Performer.OnBeat += OnBeat;
    _elapsedSurvivalBeatCount = 0;
  }

  void OnDisable() {
    GameManager.Instance.Performer.OnBeat -= OnBeat;
  }

  void Start() {
    _postProcessingIntensity = 0.0f;
    _instrument = GetComponent<Instrument>();
    _instrument.Source.volume = idleNoiseVolume;
    _instrument.SetNoteOn(0.0f);
  }

  void Update() {
    // Postprocessing update.
    if (!GameManager.Instance.IsDead()) {
      _postProcessingTargetIntensity = 0.0f;
      if (GameManager.Instance.State == GameState.RUNNING) {
        if (Physics.OverlapSphereNonAlloc(Camera.main.transform.position, postProcessingRadius,
                                          _overlapSphereColliders, interactionLayerMask) > 0) {
          if (_overlapSphereColliders[0].CompareTag("Automaton")) {
            var overlappedAutomaton =
                _overlapSphereColliders[0].transform.parent.GetComponent<Automaton>();
            if (overlappedAutomaton != null) {
              _postProcessingTargetIntensity = overlappedAutomaton.GetIntensity();
            }
          }
        }
      }
    }
    _postProcessingIntensity = Mathf.Lerp(_postProcessingIntensity, _postProcessingTargetIntensity,
                                          Time.deltaTime * postProcessingSpeed);

    _bloom.intensity.Override(idleBloomIntensity + (_postProcessingIntensity > 0.75f
                                                        ? bloomIntensity * _postProcessingIntensity
                                                        : 0.0f));
    _grain.intensity.Override(idleGrainIntensity + grainIntensity * _postProcessingIntensity);
    _grain.size.Override(idleGrainSize + grainSize * _postProcessingIntensity);

    if (GameManager.Instance.State != GameState.RUNNING) {
      return;
    }

    // Movement noise.
    _instrument.Source.volume =
        Mathf.Lerp(_instrument.Source.volume,
                   idleNoiseVolume + 0.5f * character.velocity.sqrMagnitude /
                                         (firstPerson.SprintSpeed * firstPerson.SprintSpeed),
                   Time.deltaTime * noiseSpeed);

    // Fork interaction.
    Automaton automaton = null;
    bool hasAutomatonOnTarget = false;
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out var hitInfo, maxInteractionDistance, interactionLayerMask)) {
      automaton = hitInfo.transform.parent.GetComponent<Automaton>();
      hasAutomatonOnTarget = automaton != null && automaton.CanInteract();
    }

    bool isHeld = Input.GetMouseButton(0);
    Vector3 scale = (isHeld ? clickScale : (hasAutomatonOnTarget ? hoverScale : 1.0f)) * _idleScale;

    if (Input.GetMouseButtonDown(0) && hasAutomatonOnTarget) {
      automaton.TransformToThumper();
    }

    fork.localRotation =
        Quaternion.AngleAxis((float)GameManager.Instance.Performer.Position * 90.0f, Vector3.one);
    fork.localScale = Vector3.Lerp(fork.localScale, scale, Time.deltaTime * clickScaleSpeed);
  }

  private void OnBeat() {
    if (GameManager.Instance.State == GameState.RUNNING && _postProcessingIntensity > 0.725f) {
      if (++_elapsedSurvivalBeatCount >= survivalBeatCount) {
        _elapsedSurvivalBeatCount = 0;
        GameManager.Instance.GameOver();
      }
    } else {
      _elapsedSurvivalBeatCount = 0;
    }
  }

  // private void OnCollisionEnter(Collision collision) {
  //   Debug.Log(collision.transform.parent.name);
  //   if (!collision.transform.CompareTag("Automaton")) {
  //     return;
  //   }
  //   var automaton = collision.transform.parent.GetComponent<Automaton>();
  //   if (automaton.IsThumperPlaying()) {
  //     GameManager.Instance.GameOver();
  //   }
  // }
}
