using Barely;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

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

  public float maxInteractionDistance = 5.0f;

  private Instrument _instrument = null;

  void Awake() {
    _idleScale = fork.localScale;
  }

  void Start() {
    _instrument = GetComponent<Instrument>();
    _instrument.Source.volume = idleNoiseVolume;
    _instrument.SetNoteOn(0.0f);
  }

  void Update() {
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
    if (Physics.Raycast(ray, out var hitInfo, maxInteractionDistance)) {
      if (hitInfo.collider.tag == "Automaton") {
        automaton = hitInfo.transform.parent.GetComponent<Automaton>();
        hasAutomatonOnTarget = automaton != null && automaton.CanInteract();
      }
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
}
