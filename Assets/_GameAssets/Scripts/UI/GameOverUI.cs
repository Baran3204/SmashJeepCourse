using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LeaderBoardUI _leaderBoardUI;
    [SerializeField] private Image _gameOverbackground;
    [SerializeField] private RectTransform _gameOverTransform;
    [SerializeField] private RectTransform _scoreTabletransform;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private ScoreTablePlayer _scoreTablePLayerPrefab;
    [SerializeField] private Transform _scoraTablePlayerParent;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;

    private RectTransform _mainMenuButtonTransform => _mainMenuButton.gameObject.GetComponent<RectTransform>();
    private RectTransform _winnerTransform => _winnerText.gameObject.GetComponent<RectTransform>();
    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsHost)
            {
                HostSingleton.Instance._hostGameManager.Shutdown();
            }

            ClientSingleton.Instance.GetClientGameManager().Disconnect();
        });
    }
    private void Start()
    {
        _scoreTabletransform.gameObject.SetActive(false);
        _scoreTabletransform.localScale = Vector3.zero;
        GameManager.Instance.OnGameStateChanged += (value) =>
        {
            if (value == GameState.GameOver)
            {
                AnimateGameOver();
            }
        };
    }

    private void AnimateGameOver()
    {
        _gameOverbackground.DOFade(0.8F, _animationDuration / 2);

        _gameOverTransform.DOAnchorPosY(0F, _animationDuration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _gameOverTransform.GetComponent<TMP_Text>().DOFade(0f, _animationDuration / 2).SetDelay(1f).OnComplete(() =>
            {
                AnimateLeaderBoardAndButtons();
            });
        });
    }

    private void AnimateLeaderBoardAndButtons()
    {
        _scoreTabletransform.gameObject.SetActive(true);
        _scoreTabletransform.DOScale(0.8f, _animationDuration / 2).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _mainMenuButtonTransform.DOScale(1F, _animationDuration / 2).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _winnerTransform.DOScale(1f, _animationDuration / 2).SetEase(Ease.OutBack);
            });
        });

        PopulateGameOverLeaderBoard();
    }

    private void PopulateGameOverLeaderBoard()
    {
        var leaderBoardData = _leaderBoardUI.GetLeaderBoard().OrderByDescending(X => X.Score).ToList();

        HashSet<ulong> existingClientIds = new();

        for (int i = 0; i < leaderBoardData.Count; i++)
        {
            var entry = leaderBoardData[i];

            if (existingClientIds.Contains(entry.ClientID))
            {
                continue;
            }

            var scoreTablePlayer = Instantiate(_scoreTablePLayerPrefab, _scoraTablePlayerParent);
            bool isOwner = entry.ClientID == NetworkManager.Singleton.LocalClientId;
            int rank = i + 1;

            scoreTablePlayer.SetScoreTableData(rank.ToString(), entry.PlayerName, entry.Score.ToString(), isOwner);

            existingClientIds.Add(entry.ClientID);
        }

        SetWinnerName();
    }

    private void SetWinnerName()
    {
        string winnersName = _leaderBoardUI.GetWinnersName();

        _winnerText.text = $"{winnersName} SMASHED Y'ALL!";
    }
}
