using Barely;
using UnityEngine;

[RequireComponent(typeof(Instrument))]
[RequireComponent(typeof(Performer))]
public abstract class Automaton : MonoBehaviour {
  public Transform body;

  public float hoveringNoiseSpeed = 0.2f;
  public float lowPassDistance = 10.0f;
  public float minAttackDistance = 10.0f;

  public float PlayerDistance { get; private set; }

  protected Instrument _instrument;
  protected Performer _performer;

  public abstract void Toggle();

  protected virtual void Awake() {
    _instrument = GetComponent<Instrument>();
    _performer = GetComponent<Performer>();
  }

  protected virtual void Update() {
    PlayerDistance = Vector3.Distance(body.transform.position, Camera.main.transform.position);
    _instrument.FilterFrequency = Mathf.Exp(-PlayerDistance / lowPassDistance) * 48000.0f;

    // Body hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                 Random.Range(-1.0f, 1.0f)),
                     Time.deltaTime * hoveringNoiseSpeed);
  }
}
