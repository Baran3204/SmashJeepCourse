using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin _cineMachine;
    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingInsten;

    private void Awake()
    {
        _cineMachine = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private IEnumerator CameraShakeCorunite(float inten, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        _cineMachine.AmplitudeGain = inten;
        _shakeTimer = time;
        _shakeTimerTotal = time;
        _startingInsten = inten;
    }

    public void ShakeCamera(float inten, float time, float delay = 0)
    {
        StartCoroutine(CameraShakeCorunite(inten, time, delay));
    }
    private void Update()
    {
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;

            if (_shakeTimer < 0)
            {
                _cineMachine.AmplitudeGain = Mathf.Lerp(_startingInsten, 0f, 1 - (_shakeTimer / _shakeTimerTotal));
            }
        }
    }
}
