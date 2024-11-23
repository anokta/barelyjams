using System.Collections;
using System.Collections.Generic;
using Barely;
using UnityEngine;

public class WallController : MonoBehaviour {
  public Instrument instrument;
  public ParticleSystem particles;

  private Color _color = Color.white;
  private Renderer _renderer;

  void Awake() {
    _renderer = GetComponent<Renderer>();
  }

  void Update() {
    _renderer.material.color = Color.Lerp(_renderer.material.color, _color, Time.deltaTime);
  }

  public void SetColor(Color color) {
    var colorOverLifetime = particles.colorOverLifetime;
    colorOverLifetime.enabled = true;
    Gradient gradient = new Gradient();
    gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 0.0f),
                                              new GradientColorKey(color, 1.0f) },
                     new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f),
                                              new GradientAlphaKey(0.0f, 1.0f) });
    colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
    _color = color;
  }

  private void OnCollisionEnter(Collision collision) {
    if (!collision.collider.CompareTag("Bouncer")) {
      return;
    }

    instrument.Source.pitch = Random.Range(0.995f, 1.005f);
    double pitch = GameManager.Instance.GetPitch(0);
    double intensity = (double)Mathf.Min(1.0f, 0.2f * collision.relativeVelocity.sqrMagnitude);
    instrument.SetNoteOn(pitch, intensity);
    instrument.SetNoteOff(pitch);

    collision.collider.GetComponent<BouncerController>().Sparkle(_color, (float)intensity);
    particles.Play();
  }
}
