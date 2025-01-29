using Barely;
using UnityEngine;

public class Player : MonoBehaviour {
  public Transform fork;

  public Performer performer;

  public float forkRotationSpeed = 1.0f;

  private double _lastPosition = 0.0;

  void Update() {
    double duration = performer.Position - _lastPosition;
    _lastPosition = performer.Position;

    fork.Rotate((float)duration * forkRotationSpeed * Vector3.one, Space.Self);
  }
}
