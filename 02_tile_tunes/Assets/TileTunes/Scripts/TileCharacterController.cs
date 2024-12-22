using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCharacterController : MonoBehaviour {
  public Transform character;
  public Tilemap tilemap;

  public void OnBeat(int bar, int beat) {
    var position = tilemap.WorldToCell(character.position);
    // Debug.Log("Current position = " + position);

    // TODO: assuming direction tile
    character.position += tilemap.GetTransformMatrix(position).rotation * Vector3.right;

    var tile = tilemap.GetTile(position);
    // Debug.Log("Current tile = " + tile);
  }

  private void Update() {}
}
