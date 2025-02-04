using Barely;
using UnityEngine;

public enum GameState {
  RUNNING,
  OVER,
}

public class GameManager : MonoBehaviour {
  public GameObject player;
  public GameObject fork;
  public Scale scale;

  public Performer mainPerformer;

  public FloorAutomaton menuFloor;
  public GameObject title;
  public Summoner introSummoner;

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
      // Debug.Log("Debug Beat Position: " + mainPerformer.Position);
      if (mainPerformer.Position % 2 == 0 && _nextState != State) {
        State = _nextState;
        if (State == GameState.RUNNING) {
          title.SetActive(false);
          player.SetActive(true);
          fork.SetActive(true);
          menuFloor.Stop();
          introSummoner.Init();
        } else if (State == GameState.OVER) {
          mainPerformer.Position = 0.0;
          player.SetActive(false);
          fork.SetActive(false);
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
    fork.SetActive(false);
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
        _nextState = GameState.RUNNING;
      }
    }
  }
}
