
using UnityEngine;

public class GameManager : MonoBehaviour {
  public BouncerController bouncer;
  public Transform wallsParent;

  void Update() {
    wallsParent.position =
        new Vector3(wallsParent.position.x, bouncer.transform.position.y, wallsParent.position.z);
  }
}
