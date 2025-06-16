using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image _gameOverbackground;
    [SerializeField] private RectTransform _gameOverTransform;
    [SerializeField] private RectTransform _scoreTabletransform;
    [SerializeField] private TMP_Text _winnerText;
    [SerializeField] private Button _mainMenuButton;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;

    private RectTransform _mainMenuButtonTransform => _mainMenuButton.gameObject.GetComponent<RectTransform>();
    private RectTransform _winnerTransform => _winnerText.gameObject.GetComponent<RectTransform>();

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
    }
}
