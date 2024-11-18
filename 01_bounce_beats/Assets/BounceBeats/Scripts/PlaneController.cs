using UnityEngine;

public class PlaneController : MonoBehaviour {
  public Color color = Color.white;

  public int scaleDegree = 0;

  void Start() {
    GetComponent<Renderer>().material.color = color;
  }
}
