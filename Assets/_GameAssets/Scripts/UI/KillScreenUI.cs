using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class KillScreenUI : MonoBehaviour
{
    public static KillScreenUI Instance;
    public event Action OnRespawnTimerFinished;
    [Header("Smash UI")]
    [SerializeField] private RectTransform _smashUIRecttransform;
    [SerializeField] private TMP_Text _smashedPlayerText;

    [Header("Smashed UI")]
    [SerializeField] private RectTransform _smashedUIRecttransform;
    [SerializeField] private TMP_Text _smashedByPlayerText;
    [SerializeField] private TMP_Text _respawnTimerText;

    [Header("Settings")]
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _smashUIStayDuration;

    private float _timer;
    private bool _isTimerActive;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _smashedUIRecttransform.gameObject.SetActive(false);
        _smashUIRecttransform.gameObject.SetActive(false);

        _smashedUIRecttransform.localScale = Vector3.zero;
        _smashUIRecttransform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (_isTimerActive)
        {
            _timer -= Time.deltaTime;
            int timer = (int)_timer;
            _respawnTimerText.text = timer.ToString();

            if (_timer <= 0f)
            {
                _smashedUIRecttransform.localScale = Vector3.zero;
                _smashedUIRecttransform.gameObject.SetActive(false);
                _isTimerActive = false;
                _smashedByPlayerText.text = string.Empty;
                OnRespawnTimerFinished?.Invoke();
            }
        }    
    }

    private IEnumerator SetSmashUICourtine(string playerName)
    {
        _smashUIRecttransform.gameObject.SetActive(true);
        _smashUIRecttransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _smashedPlayerText.text = playerName;

        yield return new WaitForSeconds(_smashUIStayDuration);

        _smashUIRecttransform.gameObject.SetActive(false);
        _smashUIRecttransform.localScale = Vector3.zero;
        _smashedPlayerText.text = string.Empty;
    }

    public void SetSmashUI(string playerName)
    {
        StartCoroutine(SetSmashUICourtine(playerName));
    }

    public void SetSmashedUI(string playerName, int respawnTimerCounter)
    {
        _smashedUIRecttransform.gameObject.SetActive(true);
        _smashedUIRecttransform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);

        _smashedByPlayerText.text = playerName;
        _respawnTimerText.text = respawnTimerCounter.ToString();

        _isTimerActive = true;
        _timer = respawnTimerCounter;
    }
}
