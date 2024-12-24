using Barely;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileCharacterController : MonoBehaviour {
  public Transform character;
  public SpriteRenderer characterSprite;
  public Tilemap tilemap;

  public Instrument instrument;

  private readonly float POSITION_LERP_SPEED = 8.0f;
  private Vector3 _targetPosition = Vector3.zero;

  public void OnBeat(int bar, int beat) {
    var position = tilemap.WorldToCell(character.position);
    // Debug.Log("Current position = " + position);

    // TODO: assuming direction tile
    _targetPosition += tilemap.GetTransformMatrix(position).rotation * Vector3.right;

    var tile = tilemap.GetTile(position);
    // Debug.Log("Current tile = " + tile);

    instrument.SetNoteOn(0.0f);
    instrument.SetNoteOff(0.0f);

    characterSprite.color = Color.white;
  }

  private void OnEnable() {
    _targetPosition = character.position;
  }

  private void Update() {
    character.position =
        Vector3.Lerp(character.position, _targetPosition, Time.deltaTime * POSITION_LERP_SPEED);

    characterSprite.color =
        Color.Lerp(characterSprite.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), Time.deltaTime);
  }
}
