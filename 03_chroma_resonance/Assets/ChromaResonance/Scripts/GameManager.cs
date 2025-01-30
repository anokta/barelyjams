using Barely;
using UnityEngine;

public enum GameState {
  RUNNING,
  OVER,
}

public class GameManager : MonoBehaviour {
  public GameObject player;
  public Scale scale;

  public Instrument mainInstrument;
  public Performer mainPerformer;

  // Delta performer duration since the last Update call.
  public double DeltaDuration { get; private set; } = 0.0;
  public double Position { get; private set; } = 0.0;

  public Performer Performer {
    get { return mainPerformer; }
  }

  public GameState State { get; private set; } = GameState.OVER;

  public static GameManager Instance { get; private set; }

  public float GetPitch(int degree) {
    return scale.GetPitch(degree);
  }

  private void Awake() {
    Instance = this;
  }

  void Start() {
    player.SetActive(false);
    mainPerformer.Tasks.Add(new Task(0.0f, 1.0f, delegate(TaskState state) {
      float pitch = GetPitch(0);
      if (state == TaskState.BEGIN) {
        mainInstrument.SetNoteOn(pitch);
      } else if (state == TaskState.END) {
        mainInstrument.SetNoteOff(pitch);
      }
    }));
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (State != GameState.OVER) {
        State = GameState.OVER;
        mainPerformer.Stop();
        mainPerformer.Position = 0.0;
        player.SetActive(false);
      } else {
        Application.Quit();
      }
    }

    if (Input.GetButtonDown("Jump")) {
      if (State == GameState.OVER) {
        State = GameState.RUNNING;
        player.SetActive(true);
        mainPerformer.Play();
      }
    }

    // Update delta duration.
    DeltaDuration = mainPerformer.Position - Position;
    if (DeltaDuration < 0.0) {
      DeltaDuration += mainPerformer.LoopLength;
    }
    Position = mainPerformer.Position;
  }
}
