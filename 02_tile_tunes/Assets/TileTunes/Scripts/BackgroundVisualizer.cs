using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundVisualizer : MonoBehaviour {
  public GameObject visualizerPrefab;
  public Transform parent;

  public Color peakColor;

  public float zOffset = 10.0f;
  public float width = 2.0f;
  public float maxHeight = 10.0f;
  public float minHeight = 2.0f;
  public float multiplier = 10.0f;

  public float restSpeed = 4.0f;

  private const int BAR_COUNT = 24;
  private const int FFT_SAMPLE_COUNT = 64;
  private const float REST_THRESHOLD = 0.5f;

  private GameObject[] _bars = new GameObject[BAR_COUNT];
  private float[] _fftSamples = new float[FFT_SAMPLE_COUNT];

  private float _targetX = 0.0f;
  private float _currentX = 0.0f;
  private readonly float TARGET_RANGE = 1.0f;

  public void OnBeat(int bar, int beat) {
    if (beat == 0) {
      _targetX = Random.Range(-TARGET_RANGE, TARGET_RANGE);
    }
  }

  void Start() {
    for (int i = 0; i < BAR_COUNT; ++i) {
      _bars[i] = GameObject.Instantiate(visualizerPrefab, parent);
      _bars[i].transform.localPosition =
          new Vector3((-BAR_COUNT / 2 + i) * 1.75f * width, minHeight / 2, zOffset);
      _bars[i].GetComponent<Renderer>().material.color = Color.white - Camera.main.backgroundColor;
    }
    GameManager.Instance.metronome.OnBeat += OnBeat;
  }

  void OnDestroy() {
    GameManager.Instance.metronome.OnBeat -= OnBeat;
  }

  void Update() {
    AudioListener.GetSpectrumData(_fftSamples, 0, FFTWindow.Hamming);
    float newX = Mathf.Lerp(_currentX, _targetX, 0.5f * Time.deltaTime);
    float deltaX = newX - _currentX;
    _currentX = newX;

    for (int bar = 0; bar < BAR_COUNT; ++bar) {
      // float barValue = 0.0f;
      // for (int i = 0; i < FFT_SAMPLE_COUNT / BAR_COUNT; ++i) {
      // barValue += _fftSamples[bar * FFT_SAMPLE_COUNT / BAR_COUNT + i];
      // }
      // barValue /= FFT_SAMPLE_COUNT / BAR_COUNT;
      float barValue = _fftSamples[bar];
      barValue = minHeight + barValue * (maxHeight - minHeight) * multiplier;
      // if (barValue < REST_THRESHOLD) {
      barValue =
          Mathf.Lerp(_bars[bar].transform.localScale.y, barValue, Time.deltaTime * restSpeed);
      // }
      Color baseColor = Color.white - Camera.main.backgroundColor;
      baseColor.a = 0.1f;
      Color targetColor = Color.Lerp(Color.white - Camera.main.backgroundColor, peakColor,
                                     multiplier * (barValue - minHeight) / (maxHeight - minHeight));
      _bars[bar].GetComponent<Renderer>().material.color = Color.Lerp(
          _bars[bar].GetComponent<Renderer>().material.color, targetColor, Time.deltaTime * 4.0f);

      _bars[bar].transform.localScale = new Vector3(width, barValue, 1.0f);
      _bars[bar].transform.localPosition =
          new Vector3(_bars[bar].transform.localPosition.x + deltaX, 0.5f * barValue,
                      _bars[bar].transform.localPosition.z);
    }
  }
}
