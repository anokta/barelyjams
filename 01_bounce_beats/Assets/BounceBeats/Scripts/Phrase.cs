using System.Collections;
using System.Collections.Generic;
using Barely;
using UnityEngine;

public class Phrase : MonoBehaviour {
  public BoxCollider boxCollider;
  public Instrument instrument;

  public PlaneController[] planes;

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

    instrument.SetNoteOn(GameManager.Instance.GetPitch(0));
    instrument.SetNoteOn(GameManager.Instance.GetPitch(2));
    instrument.SetNoteOn(GameManager.Instance.GetPitch(4));
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
    instrument.SetAllNotesOff();
    _canDestroy = true;
    GameManager.Instance.GenerateNewPhrase();
  }
}
