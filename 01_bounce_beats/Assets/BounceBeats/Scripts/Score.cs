using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
  public Transform parent;
  public GameObject planePrefab;

  public Color[] scaleColors;

  public Vector3 interval;

  [Multiline]
  public string score;

  public bool generateOnAwake = false;

  public void Awake() {
    if (generateOnAwake) {
      GenerateScore();
    }
  }

  public void GenerateScore() {
    string[] notes = score.Split(' ');
    Vector3 notePosition = Vector3.zero;
    for (int i = 0; i < notes.Length; ++i) {
      if (!int.TryParse(notes[i], out int degree)) {
        continue;
      }
      var plane = GameObject.Instantiate(planePrefab, parent).GetComponent<PlaneController>();
      plane.transform.localPosition = notePosition;
      plane.color = scaleColors[degree % scaleColors.Length];
      plane.scaleDegree = degree;
      notePosition += interval;
    }
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.S)) {
      GenerateScore();
    }
  }
}
