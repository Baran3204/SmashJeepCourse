using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUI : MonoBehaviour
{
    public static SkillsUI Instance;

    [Header("References")]
    [SerializeField] private TMP_Text _skillName;
    [SerializeField] private TMP_Text _timerCounter;
    [SerializeField] private Image _skillImage;
    [SerializeField] private Transform _timerCounterParent;
    [Header("Settings")]
    [SerializeField] private float _scaleDuration;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetSkillToNone();
        _timerCounterParent.transform.localScale = Vector3.zero;
        _timerCounterParent.gameObject.SetActive(false);
    }

    private void SetTimerCounterAnimations(int timerCounter)
    {
        if (_timerCounterParent.gameObject.activeInHierarchy) return;
        _timerCounterParent.gameObject.SetActive(true);
        _timerCounterParent.transform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
        _timerCounter.text = timerCounter.ToString();
    }
    public void SetSkillUI(MysteryBoxSkillsSO skill)
    {
        var skillUsageType = skill.SkillUsageType;
        var timerCounter = skill.SkillData.SpawnAmountOrTimer;
        _skillImage.gameObject.SetActive(true);
        _skillName.text = skill.SkillName;
        _skillImage.sprite = skill.SkillIcon;

        if (skillUsageType == SkillUsageType.Timer || skillUsageType == SkillUsageType.Amount)
        {
            SetTimerCounterAnimations(timerCounter);
        }
    }

    public void SetSkillToNone()
    {
        _skillImage.gameObject.SetActive(false);
        _skillName.text = string.Empty;

        if (_timerCounterParent.gameObject.activeInHierarchy) _timerCounterParent.gameObject.SetActive(false);
    }

    public void SetTimerCounterText(int timerCounter)
    {
        _timerCounter.text = timerCounter.ToString();
    }
}
