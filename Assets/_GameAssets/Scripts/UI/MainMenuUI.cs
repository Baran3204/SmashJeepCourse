using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private LobbiesParentUI _lobbyParentUI;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _lobbiesButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private RectTransform _lobbiesTransform;
    [SerializeField] private float _animDuration;
    [SerializeField] private GameObject _lobbiesParent;
    [SerializeField] private TMP_InputField _joinCode;
    [SerializeField] private TMP_Text _welcomeText;

    private void Awake()
    {
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);

        _lobbiesButton.onClick.AddListener(() =>
        {
            _lobbiesParent.SetActive(true);
            _lobbiesTransform.DOAnchorPosX(-650F, _animDuration).SetEase(Ease.OutBack);
            _lobbyParentUI.RefreshList();
        });

        _closeButton.onClick.AddListener(() =>
        {
            _lobbiesTransform.DOAnchorPosX(900f, _animDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                _lobbiesParent.SetActive(false);
            });
        });
    }

    private async void StartClient()
    {
        await ClientSingleton.Instance.GetClientGameManager().StartClientAsycn(_joinCode.text);
    }
    private void OnEnable()
    {
        var playerName = PlayerPrefs.GetString("PlayerName");

        _welcomeText.text = $"Welcome, <color=yellow>{playerName}</color>";
    }
    private async void StartHost()
    {
        await HostSingleton.Instance._hostGameManager.StartHostAsync();
    }
}
