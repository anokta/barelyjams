using Barely;
using UnityEngine;

public class FloorAutomaton : MonoBehaviour {
  public Transform body;

  public float hoveringNoiseSpeed = 0.2f;
  public float lowPassDistance = 10.0f;

  public double duration = 1.0;

  public float lightIntensity = 1000.0f;
  public float lightSpeed = 16.0f;

  public Instrument instrument;
  public Performer performer;

  private Light _spotLight;
  private float _targetIntensity = 0.0f;

  void Awake() {
    _spotLight = body.GetComponent<Light>();
    performer.Tasks.Add(new Task(0.0, duration, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        instrument.SetNoteOn(0.0f);
        instrument.SetNoteControl(0.0f, NoteControlType.PITCH_SHIFT, Random.Range(-0.001f, 0.001f));
        _targetIntensity = lightIntensity;
      } else if (state == TaskState.END) {
        instrument.SetNoteOff(0.0f);
        _targetIntensity = 0.0f;
      }
    }));
  }

  void Update() {
    float _playerDistance =
        Vector3.Distance(body.transform.position, Camera.main.transform.position);
    instrument.FilterFrequency = Mathf.Exp(-_playerDistance / lowPassDistance) * 48000.0f;

    // Body hovering noise.
    body.localPosition =
        Vector3.Lerp(body.localPosition,
                     new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f),
                                 Random.Range(-1.0f, 1.0f)),
                     Time.deltaTime * hoveringNoiseSpeed);

    _spotLight.intensity =
        Mathf.Lerp(_spotLight.intensity, _targetIntensity, Time.deltaTime * lightSpeed);
  }

  public void Play() {
    performer.Position = 0.0;
    performer.Play();
  }

  public void Stop() {
    performer.Stop();
  }
}
