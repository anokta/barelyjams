using Barely;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
  public Transform fork;

  public Repeater repeater;

  public FollowerAutomaton automaton;

  public float clickScale = 0.8f;
  public float clickScaleSpeed = 1.0f;

  void Update() {
    if (GameManager.Instance.State == GameState.RUNNING && Input.GetMouseButtonDown(0)) {
      OnClick();
    }

    fork.localRotation =
        Quaternion.AngleAxis((float)GameManager.Instance.Position * 90.0f, Vector3.one);
    fork.localScale = Vector3.Lerp(fork.localScale, Vector3.one, Time.deltaTime * clickScaleSpeed);
  }

  void OnClick() {
    fork.localScale = clickScale * Vector3.one;

    // Toggle automaton.
    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    if (Physics.Raycast(ray, out var hitInfo)) {
      if (hitInfo.collider.tag == "Automaton") {
        var automaton = hitInfo.transform.parent.GetComponent<FollowerAutomaton>();
        if (automaton != null) {
          automaton.Toggle();
        }
      }
    }
  }
}
