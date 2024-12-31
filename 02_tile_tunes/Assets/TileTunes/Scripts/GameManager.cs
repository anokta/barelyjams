
using UnityEngine;
using Barely;
using Barely.Examples;
using System;

public enum GameState {
  OVER = 0,
  COUNTDOWN,
  RUNNING,
  FINISHED,
}

public class GameManager : MonoBehaviour {
  public GameObject title;
  public GameObject countdown;

  public LevelController[] levels;
  private int _currentLevel = 0;

  public LevelController CurrentLevel {
    get { return levels[_currentLevel]; }
  }

  public SpriteRenderer countdownRenderer;
  public Sprite[] countdownSprites;
  public Sprite[] thanksSprites;

  public Metronome metronome;
  public Scale scale;

  public double initialTempo = 108.0;
  public double tempoIncrementAfterGameEnd = 12.0;
  private const double MaxTempo = 240.0;

  public GameState state = GameState.OVER;

  public static GameManager Instance { get; private set; }

  public event Metronome.BeatEventCallback BeforeOnBeat;

  public void OnBeat(int bar, int beat) {
    if (state == GameState.RUNNING) {
      BeforeOnBeat?.Invoke(bar, beat);
    }

    if (state != GameState.COUNTDOWN && state != GameState.FINISHED) {
      return;
    }

    if (metronome.beatCount != countdownSprites.Length ||
        metronome.beatCount != thanksSprites.Length) {
      Debug.LogError("Invalid beat count for countdown");
      return;
    }

    if (bar == 0) {
      countdownRenderer.sprite =
          (state == GameState.FINISHED) ? thanksSprites[beat] : countdownSprites[beat];
    } else if (state == GameState.FINISHED) {
      state = GameState.OVER;
      metronome.isTicking = false;
      metronome.OnBeat -= OnBeat;
      metronome.Stop();
      countdown.SetActive(false);
      title.SetActive(true);

      Musician.Tempo = Math.Min(MaxTempo, Musician.Tempo + tempoIncrementAfterGameEnd);
    } else {
      countdown.SetActive(false);
      metronome.isTicking = false;
      levels[_currentLevel].gameObject.SetActive(true);
      metronome.OnBeat += levels[_currentLevel].character.OnBeat;
      levels[_currentLevel].character.OnBeat(bar, beat);

      state = GameState.RUNNING;
    }
  }

  public void FinishLevel() {
    levels[_currentLevel].character.ResetMap();
    levels[_currentLevel].gameObject.SetActive(false);
    metronome.OnBeat -= levels[_currentLevel].character.OnBeat;

    metronome.OnBeat -= OnBeat;
    metronome.Stop();

    ++_currentLevel;
    if (_currentLevel >= levels.Length) {
      state = GameState.FINISHED;
      _currentLevel = 0;
    } else {
      state = GameState.COUNTDOWN;
    }
    StartCountdown();
  }

  private void Awake() {
    Instance = this;
    title.SetActive(true);
    metronome = GetComponent<Metronome>();
    for (int i = 0; i < levels.Length; ++i) {
      levels[i].gameObject.SetActive(false);
    }
  }

  private void OnEnable() {
    Musician.Tempo = initialTempo;
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (state != GameState.OVER) {
        state = GameState.OVER;
        metronome.OnBeat -= OnBeat;
        metronome.Stop();
        levels[_currentLevel].character.ResetMap();
        levels[_currentLevel].gameObject.SetActive(false);
        metronome.OnBeat -= levels[_currentLevel].character.OnBeat;
        countdown.SetActive(false);
        title.SetActive(true);
      } else {
        Application.Quit();
      }
    }

    if (Input.GetButtonDown("Jump")) {
      if (state == GameState.OVER) {
        state = GameState.COUNTDOWN;
        StartCountdown();
      }
    }
  }

  private void StartCountdown() {
    title.SetActive(false);
    countdownRenderer.sprite =
        (state == GameState.FINISHED) ? thanksSprites[0] : countdownSprites[0];
    metronome.Stop();
    metronome.OnBeat += OnBeat;
    countdown.SetActive(true);
    metronome.isTicking = true;
    metronome.Play();
  }
}
