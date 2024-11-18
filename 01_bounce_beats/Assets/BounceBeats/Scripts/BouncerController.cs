using System.Collections;
using System.Collections.Generic;
using Barely;
using UnityEngine;

public class BouncerController : MonoBehaviour {
  public float moveSpeed = 5.0f;
  public float reactionSmoothness = 0.1f;
  public float reactionSmoothnessIdle = 0.01f;

  // TODO: temp
  public Instrument instrument = null;
  public Scale scale;
  private double _lastPitch = 0.0;

  private Rigidbody _rigidBody = null;

  void Awake() {
    _rigidBody = GetComponent<Rigidbody>();
  }

  void Update() {
    float moveInput = Input.GetAxis("Horizontal");
    Vector3 targetVelocity =
        new Vector3(moveInput * moveSpeed, _rigidBody.velocity.y, _rigidBody.velocity.z);
    float smoothness = (Mathf.Abs(moveInput) > 0.1f) ? reactionSmoothness : reactionSmoothnessIdle;
    _rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, targetVelocity, smoothness);
  }

  private void OnCollisionEnter(Collision collision) {
    _lastPitch = scale.GetPitch(Random.Range(0, scale.PitchCount));
    double intensity = (double)Mathf.Min(1.0f, 0.1f * collision.relativeVelocity.sqrMagnitude);
    instrument.SetNoteOn(_lastPitch, intensity);
    instrument.SetNoteOff(_lastPitch);
  }

  // private void OnCollisionExit(Collision collision) {
  //   instrument.SetNoteOff(_lastPitch);
  // }
}
