using Barely;
using UnityEngine;

public class Summoner : MonoBehaviour {
  public FloorAutomaton floor;
  public Automaton2 automaton;

  public float automatonHeight = 1.0f;

  public Performer performer;

  void OnEnable() {
    performer.OnBeat += OnBeat;
  }

  void OnDisable() {
    performer.OnBeat -= OnBeat;
  }

  void Update() {
    if (performer.IsPlaying) {
      automaton.transform.localPosition =
          Vector3.up * ((float)(0.25f * automatonHeight * performer.Position) - automatonHeight);
    }
  }

  public void Init() {
    performer.Position = 0.0;
    performer.Play();
    floor.Play();
  }

  private void OnBeat() {
    if (performer.Position == 8.0) {
      floor.Stop();
      automaton.Play();
      performer.Stop();
    }
  }
}
