using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance;
    [SerializeField] private TMP_Text _timerText;

    public void SetTimerText(int timer)
    {
        _timerText.text = timer.ToString();
    }

    private void Awake()
    {
        Instance = this;    
    }
}
