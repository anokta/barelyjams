using Barely;
using UnityEngine;

public class FloaterThumperController : MonoBehaviour {
  public FloorAutomaton floor;
  public FloaterAutomaton floater;
  public ThumperAutomaton thumper;

  public float floaterHeight = 1.0f;


  public Performer performer;

  void Start() {
    performer.Tasks.Add(new Task(8.0, 0.0, delegate(TaskState state) {
      if (state == TaskState.BEGIN) {
        floor.Stop();
        floater.Toggle();
      } else if (state == TaskState.END) {
        // TODO: This loops infinitely if `performer.Stop()` is called from `TaskState.BEGIN`.
        performer.Stop();
      }
    }));
  }

  void Update() {
    if (performer.IsPlaying) {
      floater.transform.position =
          Vector3.up * ((float)(0.25f * floaterHeight * performer.Position) - floaterHeight);
    }
  }

  public void Init() {
    performer.Position = 0.0;
    performer.Play();
    floor.Play();
  }
}
