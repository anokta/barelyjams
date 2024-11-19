using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Barely;
using Unity.VisualScripting;
using UnityEngine;

public class BouncerController : MonoBehaviour {
  public float diveSpeed = 5.0f;
  public float moveSpeed = 5.0f;
  public float jumpSpeed = 10.0f;
  public float reactionSmoothness = 0.1f;
  public float reactionSmoothnessIdle = 0.01f;

  public Vector3 maxVelocity;
  public Vector3 minVelocity;

  public float sparkleSpeed = 8.0f;

  // TODO: temp
  public Instrument instrument = null;
  public Scale scale;
  private double _lastPitch = 0.0;

  private Renderer _renderer = null;
  private Rigidbody _rigidBody = null;

  private Color _noteOffColor = Color.white;
  private Color _targetColor = Color.white;

  private bool canDoubleJump = true;

  void Awake() {
    _renderer = GetComponent<Renderer>();
    _rigidBody = GetComponent<Rigidbody>();
  }

  void Update() {
    _renderer.material.color =
        Color.Lerp(_renderer.material.color, _targetColor, sparkleSpeed * Time.deltaTime);

    // Move.
    float moveInput = Input.GetAxis("Horizontal");
    float jumpInput = Input.GetAxis("Jump");
    float diveInput = Input.GetAxis("Fire1");

    Vector3 targetVelocity =
        new Vector3(moveInput * moveSpeed, _rigidBody.velocity.y - diveInput * diveSpeed,
                    _rigidBody.velocity.z);
    float smoothness =
        (Mathf.Abs(moveInput) > 0.1f || Mathf.Abs(diveInput) > 0.1f || Mathf.Abs(jumpInput) > 0.1f)
            ? reactionSmoothness
            : reactionSmoothnessIdle;
    _rigidBody.velocity = Vector3.Lerp(
        _rigidBody.velocity, Vector3.Min(Vector3.Max(targetVelocity, minVelocity), maxVelocity),
        smoothness);
    if (canDoubleJump && jumpInput > 0.1f) {
      _rigidBody.AddForce(Vector3.up * jumpInput * jumpSpeed, ForceMode.VelocityChange);
      canDoubleJump = false;
    }
    _rigidBody.velocity = Vector3.Min(Vector3.Max(_rigidBody.velocity, minVelocity), maxVelocity);
  }

  private void OnCollisionEnter(Collision collision) {
    double intensity = (double)Mathf.Min(1.0f, 0.1f * collision.relativeVelocity.sqrMagnitude);

    if (collision.transform.tag == "Plane") {
      _lastPitch = scale.GetPitch(collision.transform.GetComponent<PlaneController>().scaleDegree);
      _renderer.material.color =
          Color.Lerp(_noteOffColor, collision.transform.GetComponent<Renderer>().material.color,
                     (float)intensity);
    } else if (collision.transform.tag == "Wall") {
      _lastPitch = (_lastPitch == scale.GetPitch(scale.PitchCount))
                       ? scale.GetPitch(0)
                       : scale.GetPitch(scale.PitchCount);
    } else {
      _lastPitch = scale.GetPitch(Random.Range(0, scale.PitchCount));
    }

    instrument.SetNoteOn(_lastPitch, intensity);
    instrument.SetNoteOff(_lastPitch);
  }

  private void OnCollisionExit(Collision collision) {
    //   instrument.SetNoteOff(_lastPitch);
    canDoubleJump = true;
  }
}