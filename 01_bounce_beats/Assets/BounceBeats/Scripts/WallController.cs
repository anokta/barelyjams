using System.Collections;
using System.Collections.Generic;
using Barely;
using UnityEngine;

public class WallController : MonoBehaviour {
  public Instrument instrument;
  public Color color = Color.white;

  private Renderer _renderer;

  void Awake() {
    _renderer = GetComponent<Renderer>();
  }

  void Update() {
    _renderer.material.color = Color.Lerp(_renderer.material.color, color, Time.deltaTime);
  }

  private void OnCollisionEnter(Collision collision) {
    if (!collision.collider.CompareTag("Bouncer")) {
      return;
    }

    instrument.Source.pitch = Random.Range(0.99f, 1.01f);
    double pitch = GameManager.Instance.GetPitch(0);
    double intensity = (double)Mathf.Min(1.0f, 0.2f * collision.relativeVelocity.sqrMagnitude);
    instrument.SetNoteOn(pitch, intensity);
    instrument.SetNoteOff(pitch);

    collision.collider.GetComponent<BouncerController>().Sparkle(color, (float)intensity);
  }
}
