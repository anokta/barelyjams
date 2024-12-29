
using UnityEngine;
using Barely;
using Barely.Examples;
using System;

public enum GameState {
  COUNTDOWN,
  RUNNING,
  PAUSED,
  OVER,
}

public class GameManager : MonoBehaviour {
  public GameObject title;
  public GameObject countdown;

  public LevelController[] levels;
  private int _currentLevel = 0;

  public SpriteRenderer countdownRenderer;
  public Sprite[] countdownSprites;

  public Metronome metronome;
  public Scale scale;

  public double initialTempo = 108.0;
  public double tempoIncrementAfterGameEnd = 12.0;
  private const double MaxTempo = 240.0;

  public GameState state = GameState.OVER;

  public static GameManager Instance { get; private set; }

  public void OnBeat(int bar, int beat) {
    if (state != GameState.COUNTDOWN) {
      return;
    }

    if (metronome.beatCount != countdownSprites.Length) {
      Debug.LogError("Invalid beat count for countdown");
      return;
    }

    if (bar == 0) {
      countdownRenderer.sprite = countdownSprites[beat];
    } else {
      metronome.isTicking = false;
      countdown.SetActive(false);
      levels[_currentLevel].gameObject.SetActive(true);
      levels[_currentLevel].character.OnBeat(bar, beat);  // TODO: handle the start case better

      state = GameState.RUNNING;
    }
  }

  public void FinishLevel() {
    levels[_currentLevel].gameObject.SetActive(false);

    ++_currentLevel;
    if (_currentLevel >= levels.Length) {
      Musician.Tempo = Math.Min(MaxTempo, Musician.Tempo + tempoIncrementAfterGameEnd);
      _currentLevel = 0;
    }

    metronome.Stop();  // TODO: This should keep ticking during level transitions for off-beat goals
    StartCountdown();
  }

  private void Awake() {
    Instance = this;
    title.SetActive(true);
    for (int i = 0; i < levels.Length; ++i) {
      levels[i].gameObject.SetActive(false);
    }
  }

  private void OnEnable() {
    Musician.Tempo = initialTempo;
    metronome.OnBeat += OnBeat;
  }

  private void OnDisable() {
    metronome.OnBeat += OnBeat;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (state == GameState.RUNNING || state == GameState.COUNTDOWN) {
        state = GameState.OVER;
        metronome.Stop();
        levels[_currentLevel].gameObject.SetActive(false);
        countdown.SetActive(false);
        title.SetActive(true);
      } else if (state == GameState.OVER) {
        Application.Quit();
      }
    }

    if (Input.GetButtonDown("Jump")) {
      if (state != GameState.RUNNING && state != GameState.COUNTDOWN) {
        StartCountdown();
      }
    }
  }

  private void StartCountdown() {
    title.SetActive(false);
    countdownRenderer.sprite = countdownSprites[0];
    countdown.SetActive(true);
    metronome.isTicking = true;
    metronome.Play();
    state = GameState.COUNTDOWN;
  }
}
