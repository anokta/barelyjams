using Barely;
using UnityEngine;

[RequireComponent(typeof(Instrument))]
public class Automaton : MonoBehaviour {
  public Transform body;

  public float lowPassDistance = 10.0f;

  public float PlayerDistance { get; private set; }

  protected Instrument _instrument;

  protected virtual void Awake() {
    _instrument = GetComponent<Instrument>();
  }

  protected virtual void Update() {
    PlayerDistance = Vector3.Distance(body.transform.position, Camera.main.transform.position);
    _instrument.FilterFrequency = Mathf.Exp(-PlayerDistance / lowPassDistance) * 48000.0f;
  }
}
