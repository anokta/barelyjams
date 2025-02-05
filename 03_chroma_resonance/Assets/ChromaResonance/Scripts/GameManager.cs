using System.Collections.Generic;
using Barely;
using Unity.VisualScripting;
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

  public GameObject summonerPrefab;
  private Transform _summonersParent;

  public int maxSummonerCount = 100;
  private bool _instantiateNewSummoner = false;
  private GameObject[] _summonerPool = null;
  private int _summonerPoolIndex = 0;

  public Performer Performer {
    get { return mainPerformer; }
  }

  public GameState State { get; private set; } = GameState.OVER;
  private GameState _nextState = GameState.OVER;

  public static GameManager Instance { get; private set; }

  public float GetPitch(int degree) {
    return scale.GetPitch(degree);
  }

  // private List<Summoner> _activeSummoners;
  private Vector3 _playerOrigin;

  private void Awake() {
    Instance = this;

    _playerOrigin = player.transform.localPosition;

    _summonersParent = new GameObject("Summoners") { hideFlags = HideFlags.DontSave }.transform;
    _summonerPool = new GameObject[maxSummonerCount];
    for (int i = 0; i < maxSummonerCount; ++i) {
      _summonerPool[i] = GameObject.Instantiate(summonerPrefab, _summonersParent);
      _summonerPool[i].name = "Summoner " + (i + 1);
      _summonerPool[i].GetComponent<Summoner>().enabled = false;
    }

    mainPerformer.OnBeat += delegate() {
      if (mainPerformer.Position % 2 == 0) {
        if (_nextState != State) {
          State = _nextState;

          if (State == GameState.RUNNING) {
            menuFloor.Stop();
            title.SetActive(false);
            player.SetActive(true);
            fork.SetActive(true);

            InstantiateNewSummoner(Vector3.zero);
          } else if (State == GameState.OVER) {
            for (int i = 0; i < _summonerPoolIndex; ++i) {
              _summonerPool[i].transform.position =
                  Vector3.up * summonerPrefab.transform.position.y;
              _summonerPool[i].GetComponent<Summoner>().enabled = false;
            }
            _summonerPoolIndex = 0;

            player.SetActive(false);
            player.transform.localPosition = _playerOrigin;
            player.transform.localRotation = Quaternion.identity;

            fork.SetActive(false);
            menuFloor.Play();
          }
        }

        if (_instantiateNewSummoner) {
          _instantiateNewSummoner = false;
          InstantiateNewSummoner(new Vector3(Random.Range(-20.0f, 20.0f),
                                             summonerPrefab.transform.position.y,
                                             Random.Range(-20.0f, 20.0f)));
        }
      }

      if (State == GameState.OVER) {
        title.SetActive(mainPerformer.Position % 2 == 0);
      }
    };
  }

  void OnDestroy() {
    _summonerPool = null;
    GameObject.Destroy(_summonersParent.gameObject);
  }

  void Start() {
    Musician.ScheduleTask(delegate() {
      mainPerformer.Play();
      menuFloor.Play();
    }, Musician.Timestamp + 1.0);
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

    if (State == GameState.RUNNING && Input.GetKeyDown(KeyCode.G)) {
      _instantiateNewSummoner = true;
    }

    if (Input.GetMouseButtonUp(0)) {
      if (State == GameState.OVER) {
        _nextState = GameState.RUNNING;
      }
    }
  }

  private void InstantiateNewSummoner(Vector3 position) {
    int summonerIndex = _summonerPoolIndex++;
    if (summonerIndex >= maxSummonerCount) {
      Debug.LogError("Summoner pool exceeded");
      return;
    }
    var summoner = _summonerPool[summonerIndex];
    summoner.transform.position =
        new Vector3(position.x, summonerPrefab.transform.position.y, position.z);
    summoner.GetComponent<Summoner>().enabled = true;
  }
}
