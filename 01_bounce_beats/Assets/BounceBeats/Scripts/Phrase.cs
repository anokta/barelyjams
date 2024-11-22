using System.Collections;
using System.Collections.Generic;
using Barely;
using UnityEngine;

public class Phrase : MonoBehaviour {
  public BoxCollider boxCollider;
  public Instrument instrument;

  private Transform _bouncer;

  public static readonly Vector2 PADDING = new Vector2(4.0f, 4.0f);

  private bool _canDestroy = false;
  private PlaneController[] _planes;

  private void Awake() {
    _planes = GetComponentsInChildren<PlaneController>();
  }

  public void Init(Transform bouncer) {
    Vector2 size = Vector2.zero;
    for (int i = 0; i < _planes.Length; ++i) {
      size.x = Mathf.Max(size.x, 2.0f * Mathf.Abs(_planes[i].transform.localPosition.x));
      size.y = Mathf.Max(size.y, Mathf.Abs(_planes[i].transform.localPosition.y));
    }
    size += PADDING;
    boxCollider.size = new Vector3(size.x, size.y, 1.0f);
    boxCollider.center = new Vector3(0.0f, -0.5f * (size.y - PADDING.y), 0.0f);

    _bouncer = bouncer;
    _canDestroy = true;
    gameObject.SetActive(false);
  }

  public void ResetState(Vector3 position) {
    _canDestroy = false;
    transform.localPosition = position;
    gameObject.SetActive(true);

    instrument.SetNoteOn(GameManager.Instance.GetPitch(0));
    instrument.SetNoteOn(GameManager.Instance.GetPitch(2));
    instrument.SetNoteOn(GameManager.Instance.GetPitch(4));
  }

  private void Update() {
    if (_canDestroy &&
        transform.position.y - _bouncer.position.y > boxCollider.size.y + PADDING.y) {
      gameObject.SetActive(false);
      return;
    }
  }

  private void OnTriggerExit(Collider collider) {
    if (_canDestroy || !collider.CompareTag("Bouncer")) {
      return;
    }
    instrument.SetAllNotesOff();
    _canDestroy = true;
    GameManager.Instance.GenerateNewPhrase();
  }
}
