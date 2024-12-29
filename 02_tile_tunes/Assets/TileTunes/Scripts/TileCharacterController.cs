using System;
using System.Collections.Generic;
using Barely;
using Barely.Examples;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCharacterController : MonoBehaviour {
  public Transform character;
  public SpriteRenderer characterSprite;
  public Tilemap tilemap;

  public Instrument instrument;
  public Metronome metronome;

  private readonly float POSITION_LERP_SPEED = 8.0f;
  private Vector3 _targetPosition = Vector3.zero;

  private Vector3 _initPosition = Vector3.zero;

  private bool _hasPressed = false;

  private Dictionary<Vector3Int, int> _scaleDegrees = null;
  private int _currentScaleDegree = 0;

  private enum TileType {
    Wall = 0,
    Arrow,
    InteractableArrow,
    Goal,
  }

  public void OnBeat(int bar, int beat) {
    // TODO: temp reset
    if (bar == 0 && beat == 0) {
      _targetPosition = _initPosition;
    }

    var cell = tilemap.WorldToCell(_targetPosition);
    // Debug.Log("Current cell = " + cell);

    TileType tileType = GetTileType(cell);

    // Interact with the tile.
    var tileTransform = tilemap.GetTransformMatrix(cell);
    if (_hasPressed) {
      _hasPressed = false;
      if (IsInteractable(tileType)) {
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
    float pitch = GameManager.Instance.scale.GetPitch(scaleDegree);
    instrument.SetNoteOn(pitch);
    instrument.SetNoteOff(pitch);

    characterSprite.color = Color.white;

    if (tileType == TileType.Arrow || tileType == TileType.InteractableArrow) {
      _targetPosition += tileTransform.rotation * Vector3.right;
    }

    if (tileType == TileType.Goal) {
      GameManager.Instance.FinishLevel();
      character.position = _initPosition;  // TODO: temp hack to test level restart
    }
  }

  private void OnEnable() {
    _initPosition = character.position;
    _targetPosition = character.position;
    _scaleDegrees = new Dictionary<Vector3Int, int>();
    _currentScaleDegree = 0;

    metronome.OnBeat += OnBeat;
  }

  private void OnDisable() {
    metronome.OnBeat -= OnBeat;
  }

  private void Update() {
    // TODO: Change input method and the position threshold.
    if (Input.GetButtonDown("Jump")) {
      if (Math.Abs(Math.Round(metronome.Position) - metronome.Position) < 0.5) {
        float pitch = GameManager.Instance.scale.GetPitch(
            IsInteractable(GetTileType(tilemap.WorldToCell(_targetPosition)))
                ? GameManager.Instance.scale.PitchCount
                : -GameManager.Instance.scale.PitchCount);
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

  private TileType GetTileType(Vector3Int cell) {
    string name = tilemap.GetTile(cell).name;
    if (name.Contains("interact")) {
      return TileType.InteractableArrow;
    } else if (name.Contains("goal")) {
      return TileType.Goal;
    } else if (name.Contains("arrow")) {
      return TileType.Arrow;
    }
    return TileType.Wall;
  }

  private bool IsInteractable(TileType type) {
    return type == TileType.InteractableArrow;
  }
}
