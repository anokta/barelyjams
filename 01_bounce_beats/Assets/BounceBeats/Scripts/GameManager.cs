
using UnityEngine;
using Barely;

public enum GameState {
  RUNNING,
  PAUSED,
  OVER,
}

public class GameManager : MonoBehaviour {
  public BouncerController bouncer;
  public Transform wallsParent;

  public Scale scale;

  public GameObject[] phrasePrefabs;

  public static GameManager Instance { get; private set; }

  private Transform _phrasesParent = null;
  private Phrase[] _phrases = null;

  private float _positionY = 0.0f;

  void Awake() {
    Instance = this;

    _phrasesParent = new GameObject("Phrases") { hideFlags = HideFlags.DontSave }.transform;
    _phrases = new Phrase[phrasePrefabs.Length];
    for (int i = 0; i < _phrases.Length; ++i) {
      _phrases[i] = GameObject.Instantiate(phrasePrefabs[i], _phrasesParent).GetComponent<Phrase>();
      _phrases[i].Init(bouncer.transform);
    gameObject.SetActive(true);
    }
    GenerateNewPhrase();
  }

  void OnDestroy() {
    GameObject.Destroy(_phrasesParent.gameObject);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.R)) {
      bouncer.Reset();

      _positionY = 0.0f;
      GenerateNewPhrase();
    }

    wallsParent.position =
        new Vector3(wallsParent.position.x, bouncer.transform.position.y, wallsParent.position.z);
  }

  static int index = 0;
  public void GenerateNewPhrase() {
    int nextPhraseIndex = ++index % phrasePrefabs.Length;  // Random.Range(0, phrasePrefabs.Length);
    var phrase = _phrases[nextPhraseIndex];
    phrase.ResetState(_positionY * Vector3.down);
    _positionY += phrase.boxCollider.size.y;
  }
}
