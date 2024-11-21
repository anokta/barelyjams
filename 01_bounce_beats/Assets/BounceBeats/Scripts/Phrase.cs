using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phrase : MonoBehaviour {
  // public float width = 1.0f;
  // public float triggerHeight = 10.0f;
  // public float destroyHeight = 20.0f;

  // public bool IsPointInside(Vector3 point) {
  //   return transform.position.y - point.y <= triggerHeight;
  // }

  public BoxCollider boxCollider;

  private Transform _bouncer;

  private const double Y_PADDING = 2.0f;

  private bool _canDestroy = false;

  public void Init(Transform bouncer) {
    _bouncer = bouncer;
    _canDestroy = true;
    gameObject.SetActive(false);
  }

  public void ResetState(Vector3 position) {
    _canDestroy = false;
    transform.localPosition = position;
    gameObject.SetActive(true);
  }

  private void Update() {
    if (_canDestroy &&
        transform.position.y - _bouncer.position.y > boxCollider.size.y + Y_PADDING) {
      gameObject.SetActive(false);
      return;
    }
  }

  private void OnTriggerExit(Collider collider) {
    if (_canDestroy || !collider.CompareTag("Bouncer")) {
      return;
    }
    _canDestroy = true;
    GameManager.Instance.GenerateNewPhrase();
  }
}
