using UnityEngine;

public class AutomatonBody : MonoBehaviour {
  public Automaton automaton;

  private void OnTriggerEnter(Collider collider) {
    automaton.OnTriggerEnter(collider);
  }
}
