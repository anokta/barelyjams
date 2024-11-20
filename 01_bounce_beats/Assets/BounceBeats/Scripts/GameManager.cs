
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

  public static GameManager Instance { get; private set; }

  void Awake() {
    Instance = this;
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.R)) {
      bouncer.Reset();
    }

    wallsParent.position =
        new Vector3(wallsParent.position.x, bouncer.transform.position.y, wallsParent.position.z);
  }
}
