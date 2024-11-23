using UnityEngine;
using Barely;

public class PlaneController : MonoBehaviour {
  public Color color = Color.white;

  public Instrument instrument = null;
  public int scaleDegree = 0;

  public ParticleSystem particles;

  void OnEnable() {
    // scaleDegree = Random.Range(0, 8);
    color = GameManager.Instance.GetColor(scaleDegree);

    var colorOverLifetime = particles.colorOverLifetime;
    colorOverLifetime.enabled = true;
    Gradient gradient = new Gradient();
    gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 0.0f),
                                              new GradientColorKey(color, 1.0f) },
                     new GradientAlphaKey[] { new GradientAlphaKey(0.75f, 0.0f),
                                              new GradientAlphaKey(0.0f, 1.0f) });
    colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

    GetComponent<Renderer>().material.color = color;
  }

  private void OnCollisionEnter(Collision collision) {
    if (!collision.collider.CompareTag("Bouncer")) {
      return;
    }

    double pitch = GameManager.Instance.GetPitch(scaleDegree);
    double intensity = (double)Mathf.Min(1.0f, 0.1f * collision.relativeVelocity.sqrMagnitude);
    instrument.SetNoteOn(pitch, intensity);
    instrument.SetNoteOff(pitch);

    collision.collider.GetComponent<BouncerController>().Sparkle(color, (float)intensity);
    particles.Play();
  }
}
