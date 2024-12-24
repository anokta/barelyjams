using System;
using Barely;
using Barely.Examples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TileCharacterController : MonoBehaviour {
  public Transform character;
  public SpriteRenderer characterSprite;
  public Tilemap tilemap;

  public Instrument instrument;
  public Metronome metronome;

  private readonly float POSITION_LERP_SPEED = 8.0f;
  private Vector3 _targetPosition = Vector3.zero;

  private bool _hasPressed = false;

  public void OnBeat(int bar, int beat) {
    var position = tilemap.WorldToCell(_targetPosition);
    // Debug.Log("Current position = " + position);

    var tileTransform = tilemap.GetTransformMatrix(position);
    // TODO: only flip interactable arrows?
    if (_hasPressed) {
      _hasPressed = false;
      tileTransform *= Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 180.0f));
      tilemap.SetTransformMatrix(position, tileTransform);
    }

    // TODO: assuming direction tile
    _targetPosition += tileTransform.rotation * Vector3.right;

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
    // TODO: Change input method and the position threshold.
    if (Input.GetKeyDown(KeyCode.S)) {
      if (Math.Abs(Math.Round(metronome.Position) - metronome.Position) < 0.5) {
        instrument.SetNoteOn(1.0f);
        instrument.SetNoteOff(1.0f);
        _hasPressed = true;
      }
    }

    character.position =
        Vector3.Lerp(character.position, _targetPosition, Time.deltaTime * POSITION_LERP_SPEED);

    characterSprite.color =
        Color.Lerp(characterSprite.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), Time.deltaTime);
  }
}
