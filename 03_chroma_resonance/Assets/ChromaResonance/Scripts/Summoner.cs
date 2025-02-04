using Barely;
using UnityEngine;

public class Summoner : MonoBehaviour {
  public FloorAutomaton floor;
  public Automaton2 automaton;

  public float automatonHeight = 1.0f;

  public Performer performer;

  void Start() {
    // TODO: Make sure this happens before other performer calls.
    performer.Tasks.Add(new Task(7.9999, 0.0, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        floor.Stop();
        automaton.Play();
      } else if (state == TaskState.END) {
        // TODO: This loops infinitely if `performer.Stop()` is called from `TaskState.BEGIN`.
        performer.Stop();
      }
    }));
  }

  void Update() {
    if (performer.IsPlaying) {
      automaton.transform.localPosition =
          Vector3.up * ((float)(0.25f * automatonHeight * performer.Position) - automatonHeight);
    }
  }

  public void Init() {
    performer.Position = 0.0;
    performer.Play();
    floor.Play();
  }
}
