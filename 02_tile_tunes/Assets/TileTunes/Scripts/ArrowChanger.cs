using UnityEngine;

public class ArrowChanger : MonoBehaviour {
  public int beatMod = 4;

  private Vector3 _position = Vector3.zero;

  private void Start() {
    _position = transform.position;
    GameManager.Instance.BeforeOnBeat += OnBeat;
  }

  private void OnDestroy() {
    GameManager.Instance.BeforeOnBeat -= OnBeat;
  }

  public void OnBeat(int bar, int beat) {
    if (!gameObject.activeInHierarchy) {
      return;
    }

    if (beat % beatMod == 0) {
      var tilemap = GameManager.Instance.CurrentLevel.tilemap;
      var cell = tilemap.WorldToCell(_position);

      var tileTransform = tilemap.GetTransformMatrix(cell);
      tileTransform *= Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 180.0f));
      tilemap.SetTransformMatrix(cell, tileTransform);
    }
  }
}
