using Barely;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
  public Transform fork;

  public Repeater repeater;

  public FollowerAutomaton automaton;

  public float idleScale = 0.5f;
  public float clickScale = 0.4f;
  public float clickScaleSpeed = 1.0f;

  private float _playerOriginY = 0.0f;
  private float _referenceFrequency = 0.0f;

  void Start() {
    _playerOriginY = transform.position.y;
    _referenceFrequency = Musician.ReferenceFrequency;
  }

  void Update() {
    if (GameManager.Instance.State == GameState.RUNNING && Input.GetMouseButtonDown(0)) {
      OnClick();
    }

    // TODO: pitch shift could be cool
    // Musician.ReferenceFrequency =
    //     _referenceFrequency * (1.0f + _playerOriginY - transform.position.y);

    fork.localRotation =
        Quaternion.AngleAxis((float)GameManager.Instance.Position * 90.0f, Vector3.one);
    fork.localScale =
        Vector3.Lerp(fork.localScale, idleScale * Vector3.one, Time.deltaTime * clickScaleSpeed);
  }

  void OnClick() {
    fork.localScale = clickScale * Vector3.one;

    // Toggle automaton.
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out var hitInfo)) {
      if (hitInfo.collider.tag == "Automaton") {
        var automaton = hitInfo.transform.parent.GetComponent<Automaton>();
        if (automaton != null) {
          automaton.Toggle();
        }
      }
    }
  }
}
