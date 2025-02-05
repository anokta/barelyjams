using UnityEngine;

public class AutomatonBody : MonoBehaviour {
  public Automaton2 automaton;

  private void OnTriggerEnter(Collider collider) {
    automaton.OnTriggerEnter(collider);
  }
}
