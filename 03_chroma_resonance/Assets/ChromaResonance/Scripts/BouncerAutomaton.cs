using System;
using Barely;
using UnityEngine;

public class BouncerController : MonoBehaviour {
  public Instrument instrument;

  public float sparkleSpeed = 8.0f;

  private Renderer _renderer = null;
  private Rigidbody _rigidBody = null;

  private Color _noteOffColor = Color.white;
  private Color _targetColor = Color.white;

  private void Awake() {
    _renderer = GetComponent<Renderer>();
    _rigidBody = GetComponent<Rigidbody>();
  }

  private void Start() {
    instrument.SetNoteOn(0.0f);
  }

  private void Update() {
    instrument.Source.volume = _rigidBody.linearVelocity.sqrMagnitude;

    // Render.
    _renderer.material.color =
        Color.Lerp(_renderer.material.color, _targetColor, sparkleSpeed * Time.deltaTime);
  }

  private void OnCollisionEnter(Collision collision) {
    if (collision.collider.tag == "Ground") {
      Sparkle(Color.red, 1.0f);
      instrument.SetNoteOff(0.0f);
    }
  }

  private void OnCollisionExit(Collision collision) {
    if (collision.collider.tag == "Ground") {
      instrument.SetNoteOn(0.0f);
    }
  }

  public void Sparkle(Color color, float intensity) {
    _renderer.material.color = Color.Lerp(_noteOffColor, color, intensity);
  }
}
