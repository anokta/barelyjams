using System.Collections.Generic;
using Barely;
using BarelyAPI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

public enum GameState {
  RUNNING,
  OVER,
  DIED,
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

  public int restartBeatCount = 4;
  private int _elapsedRestartBeatCount = 0;

  public int newSummonBeatCount = 32;
  private int _elapsedNewSummonBeatCount = 0;
  private float _randomEncounterChance = 0.0f;

  public AudioClip[] thumperSamples;
  private int _thumperSampleIndex = 0;
  private double _thumperPosition = 0.0;

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

  private MarkovChain _markovChain;

  public bool IsDead() {
    return State == GameState.DIED || _nextState == GameState.DIED;
  }

  public void GameOver() {
    _nextState = GameState.DIED;
    _elapsedRestartBeatCount = 0;
  }

  private void Awake() {
    Instance = this;

    _markovChain = new MarkovChain(8);
    _playerOrigin = player.transform.localPosition;

    _summonersParent = new GameObject("Summoners") { hideFlags = HideFlags.DontSave }.transform;
    _summonerPool = new GameObject[maxSummonerCount];
    for (int i = 0; i < maxSummonerCount; ++i) {
      _summonerPool[i] = GameObject.Instantiate(summonerPrefab, _summonersParent);
      _summonerPool[i].name = "Summoner " + (i + 1);
      _summonerPool[i].transform.position = -1000.0f * Vector3.up;
      _summonerPool[i].GetComponent<Summoner>().enabled = false;
    }

    mainPerformer.OnBeat += delegate() {
      if (State == GameState.RUNNING) {
        if (_elapsedNewSummonBeatCount++ >= newSummonBeatCount ||
            Random.Range(0.0f, 1.0f) < _randomEncounterChance) {
          _elapsedNewSummonBeatCount = 0;
          _instantiateNewSummoner = true;
        }
        _randomEncounterChance =
            Mathf.Min(0.075f, _randomEncounterChance + 0.075f / (6.0f * newSummonBeatCount));
      } else {
        _elapsedNewSummonBeatCount = 0;
      }

      if (mainPerformer.Position % 2 == 0) {
        if (State == GameState.DIED && ++_elapsedRestartBeatCount >= restartBeatCount) {
          _nextState = GameState.OVER;
        }

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
              _summonerPool[i].transform.position = -1000.0f * Vector3.up;
              _summonerPool[i].GetComponent<Summoner>().enabled = false;
            }
            _summonerPoolIndex = 0;

            _markovChain.Reset();
            _randomEncounterChance = 0.0f;
            _thumperSampleIndex = 0;

            player.SetActive(false);
            player.transform.localPosition = _playerOrigin;
            player.transform.localRotation = Quaternion.identity;

            fork.SetActive(false);
            menuFloor.Play();
          } else if (State == GameState.DIED) {
            player.SetActive(false);
            fork.SetActive(false);
          }
        }

        if (_instantiateNewSummoner) {
          _instantiateNewSummoner = false;
          if (State == GameState.RUNNING && _nextState == GameState.RUNNING) {
            InstantiateNewSummoner(player.transform.position +
                                   new Vector3(Random.Range(-20.0f, 20.0f),
                                               summonerPrefab.transform.position.y,
                                               Random.Range(-20.0f, 20.0f)));
          }
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
      if (State == GameState.RUNNING) {
        _nextState = GameState.OVER;
      } else {
        Application.Quit();
      }
    }

    // if (State == GameState.RUNNING && Input.GetKeyDown(KeyCode.G)) {
    //   _instantiateNewSummoner = true;
    // }

    if (Input.GetMouseButtonUp(0)) {
      if (State == GameState.OVER) {
        _nextState = GameState.RUNNING;
      }
    }
  }

  private void InstantiateNewSummoner(Vector3 position) {
    int summonerIndex = _summonerPoolIndex++;
    if (summonerIndex >= maxSummonerCount) {
      return;
    }
    var summonerObject = _summonerPool[summonerIndex];
    summonerObject.transform.position =
        new Vector3(position.x, summonerPrefab.transform.position.y, position.z);
    var summoner = summonerObject.GetComponent<Summoner>();
    summoner.automaton.rootDegree = _markovChain.CurrentState;
    // summoner.automaton.thumpPosition = _thumperPosition;
    summoner.automaton.thumperProps.instrument.Slices[0].Sample =
        thumperSamples[_thumperSampleIndex];
    summoner.enabled = true;

    _elapsedNewSummonBeatCount = 0;

    _markovChain.GenerateNextState();
    _thumperSampleIndex = Random.Range(0, thumperSamples.Length);
    _thumperPosition = Random.Range(0, 4);
  }
}
