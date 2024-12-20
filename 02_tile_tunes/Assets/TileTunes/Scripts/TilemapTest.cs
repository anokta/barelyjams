using UnityEngine;

public class TilemapTest : MonoBehaviour {
  public SpriteRenderer[] sprites;

  public Color activeColor = Color.black;
  public Color inactiveColor = Color.white;
  public float speed = 16.0f;

  private int _beat = 0;

  public void OnBeat(int bar, int beat) {
    _beat = beat;
  }

  private void Update() {
    for (int i = 0; i < sprites.Length; ++i) {
      sprites[i].color = Color.Lerp(sprites[i].color, (i == _beat) ? activeColor : inactiveColor,
                                    speed * Time.deltaTime);
    }
  }
}
