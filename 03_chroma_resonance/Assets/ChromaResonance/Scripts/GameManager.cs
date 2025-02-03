using Barely;
using UnityEngine;

public enum GameState {
  RUNNING,
  OVER,
}

public class GameManager : MonoBehaviour {
  public GameObject player;
  public Scale scale;

  // public Instrument mainInstrument;
  public Performer mainPerformer;

  public FloorAutomaton menuFloor;
  public GameObject title;
  public Summoner introSummoner;

  // public Summoner firstSummoner;
  // public Summoner secondSummoner;
  // public FloorAutomaton floorAutomaton;

  // Delta performer duration since the last Update call.
  public double DeltaDuration { get; private set; } = 0.0;
  public double Position { get; private set; } = 0.0;

  public Performer Performer {
    get { return mainPerformer; }
  }

  public GameState State { get; private set; } = GameState.OVER;
  private GameState _nextState = GameState.OVER;

  public static GameManager Instance { get; private set; }

  public float GetPitch(int degree) {
    return scale.GetPitch(degree);
  }

  private void Awake() {
    Instance = this;
    mainPerformer.OnBeat += delegate() {
      Debug.Log("Debug Beat Position: " + mainPerformer.Position);
      if (mainPerformer.Position % 2 == 0 && _nextState != State) {
        State = _nextState;
        if (State == GameState.RUNNING) {
          title.SetActive(false);
          player.SetActive(true);
          menuFloor.Stop();
          introSummoner.Init();
        } else if (State == GameState.OVER) {
          mainPerformer.Position = 0.0;
          player.SetActive(false);
        }
      }
      if (State == GameState.OVER) {
        title.SetActive(mainPerformer.Position % 2 == 0);
        if (mainPerformer.Position == 2.0) {
          menuFloor.Play();
        }
      }
    };
  }

  void Start() {
    mainPerformer.Play();
    player.SetActive(false);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (State != GameState.OVER) {
        _nextState = GameState.OVER;
      } else {
        Application.Quit();
      }
    }

    if (Input.GetMouseButtonUp(0)) {
      if (State == GameState.OVER) {
        menuFloor.Stop();  // TODO: Beat callback needs to be triggered before task callbacks.
        _nextState = GameState.RUNNING;
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
