using Barely;
using UnityEngine;

public class Summoner : MonoBehaviour {
  public FloorAutomaton floor;
  public Automaton automaton;

  public float automatonHeight = 1.0f;

  public Performer performer;

  void Start() {
    performer.Tasks.Add(new Task(8.0, 0.0, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        floor.Stop();
        automaton.Toggle();
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
