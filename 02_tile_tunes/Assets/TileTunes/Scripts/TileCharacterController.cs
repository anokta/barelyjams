using System;
using System.Collections.Generic;
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

  public Scale scale;

  private readonly float POSITION_LERP_SPEED = 8.0f;
  private Vector3 _targetPosition = Vector3.zero;

  private Vector3 _initPosition = Vector3.zero;

  private bool _hasPressed = false;

  private Dictionary<Vector3Int, int> _scaleDegrees = null;
  private int _currentScaleDegree = 0;

  public void OnBeat(int bar, int beat) {
    // TODO: temp reset
    if (bar == 0 && beat == 0) {
      _targetPosition = _initPosition;
    }

    var cell = tilemap.WorldToCell(_targetPosition);
    // Debug.Log("Current cell = " + cell);

    // Interact with the tile.
    var tileTransform = tilemap.GetTransformMatrix(cell);
    if (_hasPressed) {
      _hasPressed = false;
      if (IsInteractable(cell)) {
        tileTransform *= Matrix4x4.Rotate(Quaternion.Euler(0.0f, 0.0f, 180.0f));
        tilemap.SetTransformMatrix(cell, tileTransform);
      }
    }

    // Play note.
    int scaleDegree = 0;
    if (_scaleDegrees.TryGetValue(cell, out scaleDegree)) {
      _currentScaleDegree = scaleDegree + 1;
    } else {
      scaleDegree = _currentScaleDegree++;
      _scaleDegrees.Add(cell, scaleDegree);
    }
    float pitch = scale.GetPitch(scaleDegree);
    instrument.SetNoteOn(pitch);
    instrument.SetNoteOff(pitch);

    characterSprite.color = Color.white;

    // TODO: assuming direction tile
    _targetPosition += tileTransform.rotation * Vector3.right;
  }

  private void OnEnable() {
    _initPosition = character.position;
    _targetPosition = character.position;
    _scaleDegrees = new Dictionary<Vector3Int, int>();
  }

  private void Update() {
    // TODO: Change input method and the position threshold.
    if (Input.GetKeyDown(KeyCode.S)) {
      if (Math.Abs(Math.Round(metronome.Position) - metronome.Position) < 0.5) {
        float pitch = scale.GetPitch(IsInteractable(tilemap.WorldToCell(_targetPosition))
                                         ? scale.PitchCount
                                         : -scale.PitchCount);
        instrument.SetNoteOn(pitch);
        instrument.SetNoteOff(pitch);
        _hasPressed = true;
      }
    }

    character.position =
        Vector3.Lerp(character.position, _targetPosition, Time.deltaTime * POSITION_LERP_SPEED);

    characterSprite.color =
        Color.Lerp(characterSprite.color, new Color(0.0f, 0.0f, 0.0f, 0.0f), Time.deltaTime);
  }

  private bool IsInteractable(Vector3Int cell) {
    return tilemap.GetTile(cell).name.Contains("interact");
  }
}
