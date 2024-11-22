
using UnityEngine;
using Barely;
using Unity.VisualScripting;

public enum GameState {
  RUNNING,
  PAUSED,
  OVER,
}

public class GameManager : MonoBehaviour {
  public BouncerController bouncer;
  public Transform wallsParent;

  public GameObject title;

  public Scale scale;

  public GameObject[] phrasePrefabs;

  public static GameManager Instance { get; private set; }

  private Transform _phrasesParent = null;
  private Phrase[] _phrases = null;

  public WallController[] walls;
  public BackgroundVisualizer[] visualizers;

  public Color[] scaleColors;

  private float _positionY = 0.0f;

  private int _harmonic = 0;

  public GameState state = GameState.OVER;

  void Awake() {
    Instance = this;

    _phrasesParent = new GameObject("Phrases") { hideFlags = HideFlags.DontSave }.transform;
    _phrasesParent.transform.localScale =
        new Vector3(_phrasesParent.transform.localScale.x * 1.2f,
                    _phrasesParent.transform.localScale.y, _phrasesParent.transform.localScale.z);
    _phrases = new Phrase[phrasePrefabs.Length];
    for (int i = 0; i < _phrases.Length; ++i) {
      _phrases[i] = GameObject.Instantiate(phrasePrefabs[i], _phrasesParent).GetComponent<Phrase>();
      _phrases[i].Init(bouncer.transform);
      gameObject.SetActive(true);
    }
    // GenerateNewPhrase();
  }

  void OnDestroy() {
    GameObject.Destroy(_phrasesParent.gameObject);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (state == GameState.RUNNING) {
        state = GameState.OVER;
        title.SetActive(true);
        bouncer.Reset();
        for (int i = 0; i < _phrases.Length; ++i) {
          _phrases[i].Init(bouncer.transform);
        }
      }
    }

    if (Input.GetKeyDown(KeyCode.Space)) {
      if (state != GameState.RUNNING) {
        title.SetActive(false);
        state = GameState.RUNNING;
        _positionY = bouncer.transform.position.y - 2.0f * Phrase.PADDING.y;
        GenerateNewPhrase();
      }
    }

    // if (Input.GetKeyDown(KeyCode.R)) {
    //   bouncer.Reset();

    //   _positionY = 0.0f;
    //   // GenerateNewPhrase();
    // }

    wallsParent.position =
        new Vector3(wallsParent.position.x, bouncer.transform.position.y, wallsParent.position.z);
  }

  public void GenerateNewPhrase() {
    _harmonic = Random.Range(0, scale.PitchCount);
    for (int i = 0; i < walls.Length; ++i) {
      walls[i].color = Random.ColorHSV();
      visualizers[i].peakColor = Color.white - walls[i].color;
    }
    int nextPhraseIndex = Random.Range(0, phrasePrefabs.Length);
    var phrase = _phrases[nextPhraseIndex];
    phrase.ResetState(_positionY * Vector3.up);
    _positionY -= Phrase.PADDING.y + phrase.boxCollider.size.y;
  }

  public Color GetColor(int degree) {
    return scaleColors[(_harmonic + degree) % scaleColors.Length];
  }

  public double GetPitch(int degree) {
    return scale.GetPitch(_harmonic + degree);
  }
}
