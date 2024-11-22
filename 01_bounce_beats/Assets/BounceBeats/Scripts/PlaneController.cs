using UnityEngine;
using Barely;

public class PlaneController : MonoBehaviour {
  public Color color = Color.white;

  public Instrument instrument = null;
  public int scaleDegree = 0;

  void OnEnable() {
    color = GameManager.Instance.GetColor(scaleDegree);
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
  }
}
