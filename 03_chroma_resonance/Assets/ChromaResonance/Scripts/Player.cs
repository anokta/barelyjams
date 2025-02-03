using Barely;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
  public CharacterController character;
  public Transform fork;

  public Repeater repeater;

  public FollowerAutomaton automaton;

  public float clickScale = 0.8f;
  public float hoverScale = 1.2f;
  public float clickScaleSpeed = 1.0f;
  private Vector3 _idleScale = Vector3.one;

  public float maxInteractionDistance = 5.0f;

  private Instrument _instrument = null;

  void Awake() {
    _idleScale = fork.localScale;
    _instrument = GetComponent<Instrument>();
  }

  void Start() {
    _instrument.SetNoteOn(0.0f);
  }

  // private float _playerOriginY = 0.0f;
  // private float _referenceFrequency = 0.0f;

  // void Start() {
  //   _playerOriginY = transform.position.y;
  //   _referenceFrequency = Musician.ReferenceFrequency;
  // }

  void Update() {
    if (GameManager.Instance.State != GameState.RUNNING) {
      return;
    }

    // Movement noise.
    _instrument.Source.volume = character.velocity.sqrMagnitude;

    // Fork interaction.
    bool hasAutomatonOnTarget = false;
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out var hitInfo, maxInteractionDistance)) {
      hasAutomatonOnTarget = (hitInfo.collider.tag == "Automaton");
    }

    bool isHeld = Input.GetMouseButton(0);
    Vector3 scale = (isHeld ? clickScale : (hasAutomatonOnTarget ? hoverScale : 1.0f)) * _idleScale;

    if (Input.GetMouseButtonDown(0) && hasAutomatonOnTarget) {
      // fork.localScale = scale;
      OnClick(hitInfo.transform.parent.GetComponent<Automaton>());
    }

    fork.localRotation =
        Quaternion.AngleAxis((float)GameManager.Instance.Position * 90.0f, Vector3.one);
    fork.localScale = Vector3.Lerp(fork.localScale, scale, Time.deltaTime * clickScaleSpeed);
  }

  void OnClick(Automaton automaton) {
    // Toggle automaton.
    if (automaton != null) {
      automaton.Toggle();
    }
  }
}
