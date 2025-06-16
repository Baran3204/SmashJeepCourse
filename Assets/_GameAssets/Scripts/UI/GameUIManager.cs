using DG.Tweening;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] _canvasGroups;
    [SerializeField] private float _fadeDuration;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += (value) =>
        {
            if (value == GameState.GameOver)
            {
                foreach (var c in _canvasGroups)
                {
                    c.DOFade(0f, _fadeDuration);
                }
            }
        };
    }
}
