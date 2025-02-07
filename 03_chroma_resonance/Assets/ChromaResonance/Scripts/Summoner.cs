using Barely;
using UnityEngine;

public class Summoner : MonoBehaviour {
  public FloorAutomaton floor;
  public Automaton automaton;

  public float automatonHeight = 1.0f;

  public Performer performer;

  private AudioSource[] _sources = null;

  void Awake() {
    _sources = GetComponentsInChildren<AudioSource>();
  }

  void OnEnable() {
    foreach (var source in _sources) {
      source.mute = false;
    }
    performer.OnBeat += OnBeat;
    performer.Position = 0.0;
    performer.Play();
    floor.Play();
  }

  void OnDisable() {
    floor.Stop();
    performer.OnBeat -= OnBeat;
    automaton.Stop();
    performer.Stop();
    performer.Position = 0.0;
    automaton.transform.localPosition = Vector3.down * automatonHeight;
    foreach (var source in _sources) {
      source.mute = true;
    }
  }

  void Update() {
    if (performer.IsPlaying) {
      automaton.transform.localPosition =
          Vector3.up * ((float)(0.25f * automatonHeight * performer.Position) - automatonHeight);
    }
  }

  private void OnBeat() {
    if (performer.Position == 8.0) {
      floor.Stop();
      automaton.Play();
      performer.Stop();
    }
  }
}
