using DG.Tweening;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public static HealthUI Instance;
    [SerializeField] private RectTransform _healthBar;
    [SerializeField] private float _animDur;

    private void Awake()
    {
        Instance = this;
    }
    public void SetHealth(int health, int maxHealth)
    {
        _healthBar.transform.DOScaleX(health / (float)maxHealth, _animDur).SetEase(Ease.Linear);
    }
}
