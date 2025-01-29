using Barely;
using UnityEngine;

public class Player : MonoBehaviour {
  public Transform fork;

  public Performer performer;

  public float forkRotationSpeed = 1.0f;

  private double _lastPosition = 0.0;

  public float clickScale = 0.8f;
  public float clickScaleSpeed = 1.0f;
  // public Color clickColor = Color.white;
  // private Renderer[] _renderers;
  // private Color[] _rendererColors;

  void Start() {
    // _renderers = fork.gameObject.GetComponentsInChildren<Renderer>();
    // _rendererColors = new Color[_renderers.Length];
    // for (int i = 0; i < _rendererColors.Length; ++i) {
    //   _rendererColors[i] = _renderers[i].material.color;
    // }
  }

  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      OnClick();
    }

    double duration = performer.Position - _lastPosition;
    _lastPosition = performer.Position;

    fork.Rotate((float)duration * forkRotationSpeed * Vector3.one, Space.Self);
    fork.localScale = Vector3.Lerp(fork.localScale, Vector3.one, Time.deltaTime * clickScaleSpeed);
    // for (int i = 0; i < _rendererColors.Length; ++i) {
    //   _renderers[i].material.color =
    //       Color.Lerp(_renderers[i].material.color, _rendererColors[i], Time.deltaTime);
    // }
  }

  void OnClick() {
    fork.localScale = clickScale * Vector3.one;
    // for (int i = 0; i < _rendererColors.Length; ++i) {
    //   _renderers[i].material.color = clickColor;
    // }
  }
}
